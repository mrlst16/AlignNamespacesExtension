using AlignNamespacesExtension.Interfaces.Loaders;
using AlignNamespacesExtension.Interfaces.Services;
using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Models.Responses;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.BLL.Services
{
    public class AlignNamespacesService : IAlignNamespacesService
    {
        private readonly IAlignNamespaceLoader _loader;

        public AlignNamespacesService(
            IAlignNamespaceLoader loader
            )
        {
            _loader = loader;
        }

        public async Task<MethodResponse> AlignNamespces(AlignNamespacesRequest request)
            => await _loader.AlignNamespces(request);
    }
}
