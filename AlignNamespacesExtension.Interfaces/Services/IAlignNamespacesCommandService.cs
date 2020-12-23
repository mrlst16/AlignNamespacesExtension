using CommonCore.Models.Responses;
using System;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Interfaces.Services
{
    public interface IAlignNamespacesCommandService
    {
        Task<MethodResponse> Execute(object sender, EventArgs e);
    }
}
