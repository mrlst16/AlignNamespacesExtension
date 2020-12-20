using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Models.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Services
{
    public interface IAlignNamespacesService
    {
        Task<MethodResponse> AlignNamespces(AlignNamespacesRequest request);
    }
}
