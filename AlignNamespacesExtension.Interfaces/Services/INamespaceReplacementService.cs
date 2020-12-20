using AlignNamespacesExtension.Models.DTO.Responses;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Services
{
    public interface INamespaceReplacementService
    {
        Task<ReplaceNamespacesResponse> ReplaceNamespaces(string text, string newNamspace);
    }
}