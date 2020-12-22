using AlignNamespacesExtension.Models.DTO.Requests;
using AlignNamespacesExtension.Models.DTO.Responses;
using CommonCore.Models.Responses;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Loaders
{
    public interface INamespaceReplacementLoader
    {
        Task<ReplaceNamespacesResponse> ReplaceNamespaces(ReplaceNamespaceRequest request);
        Task<MethodResponse<string>> ReplaceUsings(ReplaceUsingsRequest request);
    }
}
