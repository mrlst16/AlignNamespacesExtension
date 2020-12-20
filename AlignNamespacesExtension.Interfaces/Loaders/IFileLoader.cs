using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Loaders
{
    public interface IFileLoader
    {
        Task<string[]> GetFiles(string diectory);
        Task<string[]> GetDirectories(string diectory);
        Task<string> Read(string file);
        Task Save(string path, string text);
    }
}
