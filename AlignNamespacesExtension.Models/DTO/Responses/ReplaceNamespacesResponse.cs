using AlignNamespacesExtension.Models.DTO.NamespaceAlignment;
using CommonCore.Models.Responses;
using System.Collections.Generic;

namespace AlignNamespacesExtension.Models.DTO.Responses
{
    public class ReplaceNamespacesResponse : MethodResponse<string>
    {
        public IEnumerable<NamespaceReplacement> Replacements { get; set; }
    }
}
