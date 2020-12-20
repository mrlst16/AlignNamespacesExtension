using AlignNamespacesExtension.Interfaces.Loaders;
using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Models.Responses;
using CommonCore.Standard.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.BLL.Loaders
{
    public class AlignNamespacesLoader : IAlignNamespaceLoader
    {
        private readonly INamespaceReplacementLoader _namespaceReplacementLoader;

        public AlignNamespacesLoader(
            INamespaceReplacementLoader namespaceReplacementLoader
            )
        {
            _namespaceReplacementLoader = namespaceReplacementLoader;
        }

        public async Task<MethodResponse> AlignNamespces(AlignNamespacesRequest request)
        {
            var extension = Path.GetExtension(request.PathClicked);
            string path = null;

            switch (extension)
            {
                case ".vcxproj":
                    return await AlignNamespcesInProject(request);
                case string x when string.IsNullOrWhiteSpace(x):
                    return await AlignNamespcesInFolder(request);
                case ".cs":
                    return await AlignNamespcesInFile(request);
                default:
                    return new MethodResponse()
                        .AsFailure()
                        .AddError("Extension not supported");
            }
        }

        private async Task<MethodResponse> AlignNamespcesInProject(AlignNamespacesRequest request)
        {
            var parentFolderName = request.PathClicked.ParentFolderName();
            var pathBetweenFolders = request.PathClicked.PathBetweenFoldersToDeepestOccurrances(request.SolutionName, parentFolderName);
            var folderPath = request.PathClicked.PathToDeepestFolder(request.PathClicked.Filename());
            var ns = string.Join(".", pathBetweenFolders.Split(Path.DirectorySeparatorChar));

            await ReplaceNamespacesInFolderRecursively(folderPath, ns);

            return new MethodResponse()
            {
                Sucess = true
            };
        }

        private async Task ReplaceNamespacesInFolderRecursively(string folder, string parentNamespace)
        {
            var fileName = folder.Filename();
            var files = Directory.GetFiles(folder);
            var diectories = Directory.GetDirectories(folder);
            string ns = $"{parentNamespace}.{fileName}";

            List<Task> tasks = new List<Task>();

            foreach (var file in files)
            {
                var task = _namespaceReplacementLoader
                    .ReplaceNamespaces(new ReplaceNamespaceRequest()
                    {
                        FileName = file,
                        NewNamespace = ns,
                        Text = File.ReadAllText(file)
                    });
                tasks.Add(task);
            }

            foreach (var dir in diectories)
            {
                var task = ReplaceNamespacesInFolderRecursively(dir, ns);
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
        }

        private async Task<MethodResponse> AlignNamespcesInFolder(AlignNamespacesRequest request)
        {

            return new MethodResponse()
            {
                Sucess = true
            };
        }

        private async Task<MethodResponse> AlignNamespcesInFile(AlignNamespacesRequest request)
        {

            return new MethodResponse()
            {
                Sucess = true
            };
        }
    }
}
