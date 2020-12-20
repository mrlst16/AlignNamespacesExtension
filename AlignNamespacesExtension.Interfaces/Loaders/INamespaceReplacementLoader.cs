using AlignNamespacesExtension.Models.DTO.Requests;
using AlignNamespacesExtension.Models.DTO.Responses;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Loaders
{
    public interface INamespaceReplacementLoader
    {
        Task<ReplaceNamespacesResponse> ReplaceNamespaces(ReplaceNamespaceRequest request);
    }
}
