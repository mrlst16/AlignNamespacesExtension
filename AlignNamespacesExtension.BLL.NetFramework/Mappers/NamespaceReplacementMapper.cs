using AlignNamespacesExtension.Models.DTO.NamespaceAlignment;
using CommonCore.Interfaces.Utilities;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.BLL.Mappers
{
    public class NamespaceReplacementMapper : IMapper<Match, string, string, NamespaceReplacement>
    {
        public async Task<NamespaceReplacement> Map(Match source, string fileName, string newNamespace)
        {
            var oldNamespace = source?.Value?.Trim().Split()[1]?.Trim();
            return new NamespaceReplacement()
            {
                FileReplacedIn = fileName,
                NewNamespace = newNamespace,
                OriginalNamespace = oldNamespace
            };
        }
    }
}
