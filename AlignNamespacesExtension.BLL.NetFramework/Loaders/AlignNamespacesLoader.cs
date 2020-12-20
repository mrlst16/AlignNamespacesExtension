using AlignNamespacesExtension.Interfaces.Loaders;
using AlignNamespacesExtension.Models.DTO.Requests;
using AlignNamespacesExtension.Models.DTO.Responses;
using CommonCore.Models.Responses;
using CommonCore.Standard.Extensions;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.BLL.Loaders
{
    public class AlignNamespacesLoader : IAlignNamespaceLoader
    {
        private readonly INamespaceReplacementLoader _namespaceReplacementLoader;
        private readonly IFileLoader _fileLoader;

        public AlignNamespacesLoader(
            INamespaceReplacementLoader namespaceReplacementLoader,
            IFileLoader fileLoader
            )
        {
            _namespaceReplacementLoader = namespaceReplacementLoader;
            _fileLoader = fileLoader;
        }

        public async Task<MethodResponse> AlignNamespces(AlignNamespacesRequest request)
        {
            var extension = Path.GetExtension(request.PathClicked);

            switch (extension)
            {
                case ".csproj":
                    return await AlignNamespcesInProject(request);
                case string x when string.IsNullOrWhiteSpace(x):
                    return await AlignNamespcesInFolder(request);
                case ".sln":
                    return await AlignNamespcesInSoltion(request);
                default:
                    return new MethodResponse()
                        .AsFailure()
                        .AddError("Extension not supported");
            }
        }

        private async Task<MethodResponse> AlignNamespcesInProject(AlignNamespacesRequest request)
        {

            SolutionFile solution = SolutionFile.Parse(request.SolutionPath);

            var project = solution
                .ProjectsInOrder
                .FirstOrDefault(x => x.AbsolutePath == request.PathClicked);

            var folder = project.RelativePath.Split(Path.DirectorySeparatorChar).FirstOrDefault();
            var projectFolderPath = project.AbsolutePath.Replace(project.RelativePath, "");
            projectFolderPath = Path.Combine(projectFolderPath, folder);
            await ReplaceNamespacesInFolderRecursively(projectFolderPath, project.ProjectName);

            return new MethodResponse()
            {
                Sucess = true
            };
        }

        private async Task ReplaceNamespacesInFolderRecursively(string folder, string nameSpace)
            => await ReplaceNamespacesInFolderRecursively(folder, nameSpace, new List<ReplaceNamespacesResponse>());

        private async Task ReplaceNamespacesInFolderRecursively(string folder, string nameSpace, List<ReplaceNamespacesResponse> responses)
        {
            var files = await _fileLoader.GetFiles(folder);
            var diectories = await _fileLoader.GetDirectories(folder);

            List<Task> tasks = new List<Task>();

            Parallel.ForEach(files, async (file) =>
            {
                if (Path.GetExtension(file) != ".cs") return;
                var task = new Task(async () =>
                {
                    var response = await _namespaceReplacementLoader
                        .ReplaceNamespaces(new ReplaceNamespaceRequest()
                        {
                            FileName = file,
                            NewNamespace = nameSpace,
                            Text = await _fileLoader.Read(file)
                        });
                    responses.Add(response);
                    await _fileLoader.Save(file, response.Data);
                });
                tasks.Add(task);
            });

            Parallel.ForEach(diectories, (dir) =>
            {
                var folderName = dir.FolderName();
                var task = ReplaceNamespacesInFolderRecursively(dir, $"{nameSpace}.{folderName}");
                tasks.Add(task);
            });

            Task.WaitAll(tasks.ToArray());
        }

        private async Task<MethodResponse> AlignNamespcesInFolder(AlignNamespacesRequest request)
        {
            SolutionFile solution = SolutionFile.Parse(request.SolutionPath);
            var project = solution
                .ProjectsInOrder
                .FirstOrDefault(x => x.AbsolutePath == request.PathClicked);
            var folder = project.RelativePath.Split(Path.DirectorySeparatorChar).FirstOrDefault();
            var projectFolderPath = project.AbsolutePath.Replace(project.RelativePath, "");
            projectFolderPath = Path.Combine(projectFolderPath, folder);

            var ns = folder.Replace(Path.DirectorySeparatorChar, '.');

            await ReplaceNamespacesInFolderRecursively(folder, ns);
            return new MethodResponse()
            {
                Sucess = true
            };
        }

        private async Task<MethodResponse> AlignNamespcesInSoltion(AlignNamespacesRequest request)
        {
            SolutionFile solution = SolutionFile.Parse(request.SolutionPath);
            Parallel.For(0, solution.ProjectsInOrder.Count, async (int i) =>
            {
                await AlignNamespcesInProject(request);
            });

            return new MethodResponse()
            {
                Sucess = true
            };
        }
    }
}
