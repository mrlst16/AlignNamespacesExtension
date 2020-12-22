using AlignNamespacesExtension.BLL.Loaders;
using AlignNamespacesExtension.BLL.Mappers;
using AlignNamespacesExtension.BLL.Services;
using AlignNamespacesExtension.Interfaces.Loaders;
using AlignNamespacesExtension.Models.DTO.Requests;
using AlignNamespacesExtension.XUnit.MockData;
using AlignNamespacesExtension.XUnit.Mocks;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AlignNamespacesExtension.XUnit.Services
{
    public class AlignNamespacesServicePITests
    {
        private readonly FolderNode Root;
        private readonly FileLoaderMock _fileLoader;
        private readonly AlignNamespacesService _service;
        private readonly NamespaceReplacementLoader _namespaceReplacementLoader;
        private const string SolutionFolder = @"C:\Users\mattl\source\repos\Lab";
        private const string SolutionFile = @"C:\Users\mattl\source\repos\Lab\Lab.sln";

        public AlignNamespacesServicePITests()
        {
            var rootTask = FolderNodeMockData.LoadFromSystem(@"C:\Users\mattl\source\repos\Lab");
            rootTask.Wait();
            Root = rootTask.Result;

            _fileLoader = new FileLoaderMock(Root, SolutionFolder);
            _namespaceReplacementLoader = new NamespaceReplacementLoader(new NamespaceReplacementMapper());

            _service = new AlignNamespacesService(
                _namespaceReplacementLoader,
                _fileLoader
                );
        }

        [Fact]
        public async Task AlignNamepacesInSolution()
        {
            var result = await _service.AlignNamespces(new AlignNamespacesRequest()
            {
                PathClicked = SolutionFile,
                SolutionDirectory = SolutionFolder,
                SolutionName = "Lab",
                SolutionPath = SolutionFile
            });


        }
    }
}
