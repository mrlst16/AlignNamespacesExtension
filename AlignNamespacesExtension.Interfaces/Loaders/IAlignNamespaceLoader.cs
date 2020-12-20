using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Models.Responses;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Loaders
{
    public interface IAlignNamespaceLoader
    {
        Task<MethodResponse> AlignNamespces(AlignNamespacesRequest request);
    }
}
