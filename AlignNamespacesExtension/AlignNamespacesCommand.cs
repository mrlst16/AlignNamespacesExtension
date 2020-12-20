using AlignNamespacesExtension.BLL.Loaders;
using AlignNamespacesExtension.BLL.Mappers;
using AlignNamespacesExtension.BLL.NetFramework.Loaders;
using AlignNamespacesExtension.BLL.Services;
using AlignNamespacesExtension.Interfaces.Services;
using AlignNamespacesExtension.Models.DTO.Requests;
using CommonCore.Models.Responses;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;
using Task = System.Threading.Tasks.Task;

namespace AlignNamespacesExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AlignNamespacesCommand
    {
        private readonly IAlignNamespacesService _alignNamespacesService;
        protected readonly List<string> AcceptableFileTypes = new List<string>() {
            ".cs", ".vcxproj", ".sln" , ".csproj"
        };

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("3e98dcc5-55f7-472c-837e-8dd127a4b6ba");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlignNamespacesCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AlignNamespacesCommand(
            AsyncPackage package,
            OleMenuCommandService commandService,
            IAlignNamespacesService alignNamespacesService
            )
        {
            _alignNamespacesService = alignNamespacesService;
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);

            commandService.AddCommand(menuItem);
        }

        void Execute(object sender, EventArgs e)
        {
            // get the menu that fired the event
            var menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                AlignNamespacesRequest request = new AlignNamespacesRequest();

                IVsHierarchy hierarchy = null;
                uint itemid = VSConstants.VSITEMID_NIL;
                var gathered = Gather(out hierarchy, out itemid, ref request);

                if (!gathered) return;
                // Get the file path
                string itemFullPath = null;
                ((IVsProject)hierarchy).GetMkDocument(itemid, out itemFullPath);
                var transformFileInfo = new FileInfo(itemFullPath);
                if (!AcceptableFileTypes.Contains(Path.GetExtension(itemFullPath))) return;
                request.PathClicked = itemFullPath;

                var resultTask = _alignNamespacesService.AlignNamespces(request);
                Task.WaitAll(resultTask);

                MethodResponse result = resultTask.Result;

            }
        }

        public static bool Gather(out IVsHierarchy hierarchy, out uint itemid, ref AlignNamespacesRequest request)
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

                // if we got this far then there is a single project item selected
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

        /// <summary>
        //Gets the instance othe command.
        /// </summary>
        public static AlignNamespacesCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in AlignNamespacesCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            var mapper = new NamespaceReplacementMapper();
            var namespaceReplacementLoader = new NamespaceReplacementLoader(mapper);
            FileLoader fileLoader = new FileLoader();
            var loader = new AlignNamespacesLoader(namespaceReplacementLoader, fileLoader);
            var service = new AlignNamespacesService(loader);

            Instance = new AlignNamespacesCommand(package, commandService, service);
        }
    }
}
