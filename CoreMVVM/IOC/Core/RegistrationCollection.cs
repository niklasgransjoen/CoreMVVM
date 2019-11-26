using CoreMVVM.IOC.Builder;
using System;
using System.Collections.Generic;

namespace CoreMVVM.IOC.Core
{
    /// <summary>
    /// A collection of registrations mapped to types.
    /// </summary>
    internal class RegistrationCollection : Dictionary<Type, IRegistration>
    {
    }
}