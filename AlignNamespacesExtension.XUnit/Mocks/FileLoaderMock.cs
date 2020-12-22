using AlignNamespacesExtension.Interfaces.Loaders;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using System.IO;
using AlignNamespacesExtension.XUnit.MockData;

namespace AlignNamespacesExtension.XUnit.Mocks
{
    public class FileLoaderMock : IFileLoader
    {
        public FolderNode Root { get; protected set; }
        public string SolutionFolder { get; set; }

        public FileLoaderMock(
            FolderNode root,
            string solutionFolder
            )
        {
            Root = root;
            SolutionFolder = solutionFolder;
        }

        public async Task<string[]> GetDirectories(string diectory)
            => NavigateToFolder(diectory.Split(Path.DirectorySeparatorChar))
                .Folders
                .Select(x => $"{diectory}{Path.DirectorySeparatorChar}{x.Name}")
                .ToArray();

        internal FolderNode NavigateToFolder(params string[] path)
        {
            string start = string.Empty;

            for (int i = 0; i < path.Count(); i++)
            {
                //if(path)
            }
            FolderNode result = Root.Folders.FirstOrDefault(x => x.Name == path.LastOrDefault());
            
            if (path.Count() == 1) return result;

            for (int i = 1; i < path.Count(); i++)
            {
                result = Root.Folders.FirstOrDefault(x => x.Name == path[i]);
                if (result == null) return null;
            }
            return result;
        }

        public async Task<string[]> GetFiles(string diectory)
            => NavigateToFolder(diectory.Split(Path.DirectorySeparatorChar))
                .Files
                .Select(x => $"{diectory}{Path.DirectorySeparatorChar}{x.Key}")
                .ToArray();

        public async Task<string> Read(string file)
        {
            var parts = file.Split(Path.DirectorySeparatorChar);
            var folder = NavigateToFolder(parts);
            return folder.Files[parts.Last()];
        }

        public async Task Save(string path, string text)
        {
            var parts = path.Split(Path.DirectorySeparatorChar);
            var folder = NavigateToFolder(parts);
            folder.Files[parts.Last()] = text;
        }
    }


    public class FileLoaderMockTests
    {
        [Fact]
        public void GetDirectories_BasicInstance_NotNull()
        {
            var fileLoader = new FileLoaderMock(FolderNodeMockData.Instance, string.Empty);
            string[] paths = new string[] { "three", "three" };
            var folder = fileLoader.NavigateToFolder(paths);
            Assert.NotNull(folder);
        }

        [Fact]
        public async Task LoadFromSystem()
        {
            var fileLoader = await FolderNodeMockData.LoadFromSystem(@"C:\Users\mattl\source\repos\Lab");

        }
    }
}
