using System;
using System.Collections.Generic;
using System.Text;

namespace AlignNamespacesExtension.Models.DTO.NamespaceAlignment
{
    public class NamespaceReplacement
    {
        public string OriginalNamespace { get; set; }
        public string NewNamespace { get; set; }
        public string FileReplacedIn { get; set; }
    }
}
