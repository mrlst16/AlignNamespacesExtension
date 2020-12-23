using AlignNamespacesExtension.Interfaces.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.BLL.NetFramework.Loaders
{
    public class FileLoader : IFileLoader
    {
        public async Task<string[]> GetDirectories(string diectory)
            => Directory.GetDirectories(diectory)
                .Where(x=> Directory.GetFiles(x, "*.cs", SearchOption.AllDirectories).Length > 0)
                .ToArray();

        public async Task<string[]> GetFiles(string diectory)
            => Directory.GetFiles(diectory, "*.cs");

        public async Task<string> Read(string file)
            => File.ReadAllText(file);

        public async Task Save(string path, string text)
            => File.WriteAllText(path, text);
    }
}
