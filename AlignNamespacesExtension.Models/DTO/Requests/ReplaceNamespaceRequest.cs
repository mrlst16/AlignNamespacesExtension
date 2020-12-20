using System;
using System.Collections.Generic;
using System.Text;

namespace AlignNamespacesExtension.Models.DTO.Requests
{
    public class ReplaceNamespaceRequest
    {
        public string FileName { get; set; }
        public string Text { get; set; }
        public string NewNamespace { get; set; }
    }
}
