using System;
using System.Collections.Generic;
using System.Text;

namespace AlignNamespacesExtension.XUnit.Mocks
{
    public class FolderNode
    {
        public string Name { get; set; }
        public IList<FolderNode> Folders { get; set; }
        public IDictionary<string, string> Files { get; set; }
    }
}
