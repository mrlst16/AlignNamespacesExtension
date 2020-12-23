using AlignNamespacesExtension.Interfaces.Loaders;
using AlignNamespacesExtension.Interfaces.Services;
using AlignNamespacesExtension.Models.DTO.Requests;
using AlignNamespacesExtension.Models.DTO.Responses;
using CommonCore.Models.Responses;
using Microsoft.Build.Construction;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommonCore.Standard.Extensions;
using AlignNamespacesExtension.Models.DTO.NamespaceAlignment;

namespace AlignNamespacesExtension.BLL.Services
{
    public class AlignNamespacesService : IAlignNamespacesService
    {
        private readonly INamespaceReplacementLoader _namespaceReplacementLoader;
        private readonly IFileLoader _fileLoader;

        public AlignNamespacesService(
            INamespaceReplacementLoader namespaceReplacementLoader,
            IFileLoader fileLoader
            )
        {
            _namespaceReplacementLoader = namespaceReplacementLoader;
            _fileLoader = fileLoader;
        }

        public async Task<MethodResponse> AlignNamespces(AlignNamespacesRequest request)
        {
            var result = new MethodResponse<List<ReplaceNamespacesResponse>>();
            var extension = Path.GetExtension(request.PathClicked);

            switch (extension)
            {
                case ".csproj":
                    result.Data = await AlignNamespcesInProject(request);
                    break;
                case string x when string.IsNullOrWhiteSpace(x):
                    result.Data = await AlignNamespcesInFolder(request);
                    break;
                case ".sln":
                    result.Data = await AlignNamespcesInSoltion(request);
                    break;
                default:
                    return new MethodResponse()
                        .AsFailure()
                        .AddError("Extension not supported");
            }

            var namespaceReplacements = result.Data
                .SelectMany(x => x.Replacements)
                .Distinct(x => x.OriginalNamespace)
                .ToList();

            await ReplaceUsingsInFolder(request.SolutionDirectory, namespaceReplacements);

            return result;
        }

        private async Task ReplaceUsingsInFolder(string folder, List<NamespaceReplacement> namespaceReplacements)
        {
            var files = (await _fileLoader.GetFiles(folder));
            var directories = await _fileLoader.GetDirectories(folder);

            for (int i = 0; i < files.Length; i++)
            {
                var replaceResult = await _namespaceReplacementLoader.ReplaceUsings(new ReplaceUsingsRequest()
                {
                    Replacements = namespaceReplacements,
                    Text = await _fileLoader.Read(files[i])
                });

                if (replaceResult)
                    await _fileLoader.Save(files[i], replaceResult.Data);
            }

            for (int i = 0; i < directories.Length; i++)
            {
                await ReplaceUsingsInFolder(directories[i], namespaceReplacements);
            }
        }

        private ProjectInSolution GetProject(AlignNamespacesRequest request)
        {
            SolutionFile solution = SolutionFile.Parse(request.SolutionPath);

            return solution
                .ProjectsInOrder
                .FirstOrDefault(x => x.AbsolutePath == request.PathClicked);
        }

        private async Task<MethodResponse<List<ReplaceNamespacesResponse>>> AlignNamespcesInProject(AlignNamespacesRequest request)
        {
            var project = GetProject(request);

            var folder = project.RelativePath.Split(Path.DirectorySeparatorChar).FirstOrDefault();
            var projectFolderPath = project.AbsolutePath.Replace(project.RelativePath, "");
            projectFolderPath = Path.Combine(projectFolderPath, folder);
            var nsReplacementResult = await ReplaceNamespacesInFolderRecursively(projectFolderPath, project.ProjectName);
            nsReplacementResult = nsReplacementResult
                .Select(x =>
                {
                    x.Replacements = x.Replacements.Where(r => r.OriginalNamespace != r.NewNamespace);
                    return x;
                })
                .Where(x => x.Replacements.Any())
                .ToList();

            return new MethodResponse<List<ReplaceNamespacesResponse>>()
            {
                Sucess = true,
                Data = nsReplacementResult
            };
        }

        private async Task<List<ReplaceNamespacesResponse>> ReplaceNamespacesInFolderRecursively(string folder, string nameSpace)
        {
            var result = new List<ReplaceNamespacesResponse>();
            await ReplaceNamespacesInFolderRecursively(folder, nameSpace, result);
            return result;
        }

        private async Task ReplaceNamespacesInFolderRecursively(string folder, string nameSpace, List<ReplaceNamespacesResponse> responses)
        {
            var files = await _fileLoader.GetFiles(folder);
            var diectories = await _fileLoader.GetDirectories(folder);

            foreach (var file in files)
            {
                if (Path.GetExtension(file) != ".cs") continue;

                var response = await _namespaceReplacementLoader
                    .ReplaceNamespaces(new ReplaceNamespaceRequest()
                    {
                        FileName = file,
                        NewNamespace = nameSpace,
                        Text = await _fileLoader.Read(file)
                    });
                responses.Add(response);
                await _fileLoader.Save(file, response.Data);
            }

            foreach (var dir in diectories)
            {
                var folderName = dir.FolderName();
                await ReplaceNamespacesInFolderRecursively(dir, $"{nameSpace}.{folderName}", responses);
            }
        }

        private async Task<MethodResponse<List<ReplaceNamespacesResponse>>> AlignNamespcesInFolder(AlignNamespacesRequest request)
        {
            var project = GetProject(request);
            var folder = project.RelativePath.Split(Path.DirectorySeparatorChar).FirstOrDefault();
            var projectFolderPath = project.AbsolutePath.Replace(project.RelativePath, "");
            projectFolderPath = Path.Combine(projectFolderPath, folder);

            var ns = folder.Replace(Path.DirectorySeparatorChar, '.');

            var data = await ReplaceNamespacesInFolderRecursively(folder, ns);
            return new MethodResponse<List<ReplaceNamespacesResponse>>()
            {
                Sucess = true,
                Data = data
            };
        }

        private async Task<MethodResponse<List<ReplaceNamespacesResponse>>> AlignNamespcesInSoltion(AlignNamespacesRequest request)
        {
            SolutionFile solution = SolutionFile.Parse(request.SolutionPath);
            Parallel.For(0, solution.ProjectsInOrder.Count, async (int i) =>
            {
                request.PathClicked = solution.ProjectsInOrder[i].AbsolutePath;
                await AlignNamespcesInProject(request);
            });

            return new MethodResponse<List<ReplaceNamespacesResponse>>()
            {
                Sucess = true
            };
        }
    }
}
