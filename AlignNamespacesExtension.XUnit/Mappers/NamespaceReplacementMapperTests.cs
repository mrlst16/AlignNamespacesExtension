using AlignNamespacesExtension.BLL.Mappers;
using System.Threading.Tasks;
using Xunit;

namespace AlignNamespacesExtension.XUnit.Mappers
{
    public class NamespaceReplacementMapperTests
    {

        private readonly NamespaceReplacementMapper _mapper;

        public NamespaceReplacementMapperTests()
        {
            _mapper = new NamespaceReplacementMapper();
        }

        [Fact]
        public async Task Map_ProperlyParsedMatch()
        {
            var data = "namespace Lab.Goodies { class F { } }  " + System.Environment.NewLine + "  namespace Lab.Goodies.Sweets { }";

            var filename = "someFileName";
            var newNamespace = "Lab.Replacement";

            var match = CommonCore.Standard.CommonRegex.NamespaceRegex.Match(data);

            var result = await _mapper.Map(match, filename, newNamespace);

            Assert.Equal(filename, result.FileReplacedIn);
            Assert.Equal(newNamespace, result.NewNamespace);
            Assert.Equal("Lab.Goodies", result.OriginalNamespace);
        }
    }
}
