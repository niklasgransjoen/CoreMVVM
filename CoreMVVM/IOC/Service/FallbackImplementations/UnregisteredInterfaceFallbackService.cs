﻿using System;
using System.Reflection;

namespace CoreMVVM.IOC.FallbackImplementations
{
    /// <summary>
    /// Implementation of <see cref="IResolveUnregisteredInterfaceService"/>,
    /// uses <see cref="FallbackImplementationAttribute"/> to attempt resolve.
    /// </summary>
    public sealed class UnregisteredInterfaceFallbackService : IResolveUnregisteredInterfaceService
    {
        /// <summary>
        /// Handles the resolving of an unregistered interface.
        /// </summary>
        public void Handle(ResolveUnregisteredInterfaceContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var attribute = context.InterfaceType.GetCustomAttribute<FallbackImplementationAttribute>();
            if (attribute != null)
            {
                context.SetInterfaceImplementationType(attribute.Type);
                context.CacheImplementation = true;
                context.CacheScope = ComponentScope.Singleton;
            }
        }
    }
}