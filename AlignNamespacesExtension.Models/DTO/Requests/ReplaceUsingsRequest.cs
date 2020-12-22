using AlignNamespacesExtension.Models.DTO.NamespaceAlignment;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlignNamespacesExtension.Models.DTO.Requests
{
    public class ReplaceUsingsRequest
    {
        public IEnumerable<NamespaceReplacement> Replacements { get; set; }
        public string Text { get; set; }
    }
}
