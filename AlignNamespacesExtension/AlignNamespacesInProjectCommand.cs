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
using Unity;
using Task = System.Threading.Tasks.Task;

namespace AlignNamespacesExtension
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AlignNamespacesInProjectCommand
    {
        private readonly IAlignNamespacesCommandService _service;

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
        /// Initializes a new instance of the <see cref="AlignNamespacesInProjectCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AlignNamespacesInProjectCommand(
            AsyncPackage package,
            OleMenuCommandService commandService,
            IAlignNamespacesCommandService service
            )
        {
            _service = service;

            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);

            commandService.AddCommand(menuItem);
        }

        void Execute(object sender, EventArgs e)
        {
            var task = _service.Execute(sender, e);
            task.Wait();
        }

        /// <summary>
        //Gets the instance othe command.
        /// </summary>
        public static AlignNamespacesInProjectCommand Instance
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

            var service = AlignNamespacesExtensionPackage.UnityContainer.Resolve<IAlignNamespacesCommandService>();
            Instance = new AlignNamespacesInProjectCommand(package, commandService, service);
        }
    }
}
