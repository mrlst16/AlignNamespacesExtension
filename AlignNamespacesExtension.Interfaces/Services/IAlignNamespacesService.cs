using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Models.Responses;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Services
{
    public interface IAlignNamespacesService
    {
        Task<MethodResponse> AlignNamespces(AlignNamespacesRequest request);
    }
}
