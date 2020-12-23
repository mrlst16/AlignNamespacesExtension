using AlignNamespacesExtension.Interfaces.Services;
using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Models.Responses;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AlignNamespacesExtension.Services
{
    public class AlignNamespacesCommandService : IAlignNamespacesCommandService
    {
        private readonly IAlignNamespacesService _alignNamespacesService;
        protected readonly List<string> AcceptableFileTypes = new List<string>() {
            ".cs", ".vcxproj", ".sln" , ".csproj"
        };

        public AlignNamespacesCommandService(
            IAlignNamespacesService alignNamespacesService
            )
        {
            _alignNamespacesService = alignNamespacesService;
        }

        public async Task<MethodResponse> Execute(object sender, EventArgs e)
        {
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                AlignNamespacesRequest request = new AlignNamespacesRequest();

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;
                var gathered = Gather(out hierarchy, out itemid, ref request);

                if (!gathered) 
                    return new MethodResponse()
                        .AsFailure();

                string itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);

                if (!AcceptableFileTypes.Contains(Path.GetExtension(itemFullPath)))
                    return new MethodResponse()
                       .AsFailure();

                request.PathClicked = itemFullPath;

                return await _alignNamespacesService.AlignNamespces(request);
            }
            return new MethodResponse().AsFailure();
        }

        private static bool Gather(out IVsHierarchy hierarchy, out uint itemid, ref AlignNamespacesRequest request)
        {
            hierarchy = null;
            itemid = VSConstants.VSITEMID_NIL;
            int hr = VSConstants.S_OK;

            var monitorSelection = Package.GetGlobalService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;
            var solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;

            if (monitorSelection == null || solution == null)
            {
                return false;
            }

            solution.GetProperty(((int)__VSPROPID.VSPROPID_SolutionBaseName), out object solutionName);
            solution.GetProperty(((int)__VSPROPID.VSPROPID_SolutionDirectory), out object solutionDirectory);
            solution.GetProperty(((int)__VSPROPID.VSPROPID_SolutionFileName), out object solutionFileName);

            request.SolutionName = solutionName.ToString();
            request.SolutionDirectory = solutionDirectory.ToString();
            request.SolutionPath = solutionFileName.ToString();

            IVsMultiItemSelect multiItemSelect = null;
            IntPtr hierarchyPtr = IntPtr.Zero;
            IntPtr selectionContainerPtr = IntPtr.Zero;

            try
            {
                hr = monitorSelection.GetCurrentSelection(out hierarchyPtr, out itemid, out multiItemSelect, out selectionContainerPtr);

                if (ErrorHandler.Failed(hr) || hierarchyPtr == IntPtr.Zero || itemid == VSConstants.VSITEMID_NIL)
                {
                    // there is no selection
                    return false;
                }

                // multiple items are selected
                if (multiItemSelect != null) return false;

                hierarchy = Marshal.GetObjectForIUnknown(hierarchyPtr) as IVsHierarchy;
                if (hierarchy == null) return false;

                Guid guidProjectID = Guid.Empty;

                if (ErrorHandler.Failed(solution.GetGuidOfProject(hierarchy, out guidProjectID)))
                {
                }

                return true;
            }
            finally
            {
                if (selectionContainerPtr != IntPtr.Zero)
                {
                    Marshal.Release(selectionContainerPtr);
                }

                if (hierarchyPtr != IntPtr.Zero)
                {
                    Marshal.Release(hierarchyPtr);
                }
            }
        }

    }
}
