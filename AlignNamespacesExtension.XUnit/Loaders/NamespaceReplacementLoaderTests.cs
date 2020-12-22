using AlignNamespacesExtension.BLL.Loaders;
using AlignNamespacesExtension.Models.DTO.NamespaceAlignment;
using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Interfaces.Utilities;
using NSubstitute;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace AlignNamespacesExtension.XUnit.Loaders
{
    public class NamespaceReplacementLoaderTests
    {
        private readonly NamespaceReplacementLoader _loader;
        private readonly IMapper<Match, string, string, NamespaceReplacement> _mapper;

        public NamespaceReplacementLoaderTests()
        {
            _mapper = Substitute.For<IMapper<Match, string, string, NamespaceReplacement>>();
            _mapper
                .Map(Arg.Any<Match>(), Arg.Any<string>(), Arg.Any<string>())
                .ReturnsForAnyArgs(new NamespaceReplacement());
            _loader = new NamespaceReplacementLoader(_mapper);
        }

        [Fact]
        public async Task Replace_DoesNotContainOldNamepace()
        {
            var data = "namespace Lab.Goodies { class F { } }  " + System.Environment.NewLine + "  namespace Lab.Sweets { }";

            var result = await _loader.ReplaceNamespaces(new ReplaceNamespaceRequest()
            {
                FileName = "somefile",
                NewNamespace = "Lab.Replacement",
                Text = data
            });

            Assert.DoesNotContain("Lab.Goodies", result.Data);
            Assert.DoesNotContain("Lab.Sweets", result.Data);
        }

        [Fact]
        public async Task Replace_ContainsNewNamepace()
        {
            var data = "namespace Lab.Goodies { class F { } }  " + System.Environment.NewLine + "  namespace Lab.Sweets { }";

            var result = await _loader.ReplaceNamespaces(new ReplaceNamespaceRequest()
            {
                FileName = "somefile",
                NewNamespace = "Lab.Replacement",
                Text = data
            });

        }

        [Fact]
        public async Task ReplaceUsing()
        {
            var data = "using namespace Lab.Goodies ;" + System.Environment.NewLine + "using namespace Lab.Sweets; namespace Lab.Goodies.Balls{}";

            var result = await _loader.ReplaceUsings(new ReplaceUsingsRequest()
            {
                Text = data,
                Replacements = new List<NamespaceReplacement>()
                {
                    new NamespaceReplacement(){
                        OriginalNamespace = "Lab.Goodies",
                        NewNamespace = "Lab.Donkeys"
                    },
                    new NamespaceReplacement(){
                        OriginalNamespace = "Lab.Sweets",
                        NewNamespace = "Lab.Donkeys2"
                    }
                }
            });
            Assert.Equal("using namespace Lab.Donkeys;" + System.Environment.NewLine + "using namespace Lab.Donkeys2; namespace Lab.Goodies.Balls{}", result.Data);
        }

    }
}
