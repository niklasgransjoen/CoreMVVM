using CoreMVVM.Windows;
using System;

namespace CoreMVVM.IOC.Builder
{
    public static class CoreMVVMWindowsContainerBuilderExtensions
    {
        /// <summary>
        /// Registers CoreMVVM.Windows' services with a container builder.
        /// </summary>
        public static ContainerBuilder AddWindowsServices(this ContainerBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.RegisterTransient<ViewSetServiceProvider>().As<IViewInitializer>();
            builder.RegisterTransient<ViewDataContextInitializer>().As<IViewInitializer>();
            builder.RegisterTransient<ViewComponentInitializer>().As<IViewInitializer>();

            builder.OnBuild -= OnContainerBuilt;
            builder.OnBuild += OnContainerBuilt;

            return builder;
        }

        private static void OnContainerBuilt(IContainer container)
        {
            container.ConfigureWindowsServices();
        }
    }
}