using AlignNamespacesExtension.BLL.Loaders;
using AlignNamespacesExtension.BLL.Mappers;
using AlignNamespacesExtension.BLL.NetFramework.Loaders;
using AlignNamespacesExtension.BLL.Services;
using AlignNamespacesExtension.Interfaces.Loaders;
using AlignNamespacesExtension.Interfaces.Services;
using AlignNamespacesExtension.Models.DTO.NamespaceAlignment;
using AlignNamespacesExtension.Services;
using CommonCore.Interfaces.Utilities;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Unity;
using Task = System.Threading.Tasks.Task;

namespace AlignNamespacesExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(AlignNamespacesExtensionPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class AlignNamespacesExtensionPackage : AsyncPackage
    {
        public static IUnityContainer UnityContainer = new UnityContainer();
        /// <summary>
        /// AlignNamespacesExtensionPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "45df6dbd-4cfc-4f82-ae77-f788eee81358";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            UnityContainer.RegisterType<IMapper<Match, string, string, NamespaceReplacement>, NamespaceReplacementMapper>();
            UnityContainer.RegisterType<INamespaceReplacementLoader, NamespaceReplacementLoader>();
            UnityContainer.RegisterType<IFileLoader, FileLoader>();
            UnityContainer.RegisterType<IAlignNamespacesService, AlignNamespacesService>();
            UnityContainer.RegisterType<IAlignNamespacesCommandService, AlignNamespacesCommandService>();
            
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await AlignNamespacesInProjectCommand.InitializeAsync(this);
            await AlignNamespacesInFolderCommand.InitializeAsync(this);
            await AlignNamespacesInSolutuionCommand.InitializeAsync(this);
        }

        #endregion
    }
}
