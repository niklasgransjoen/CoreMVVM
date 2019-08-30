using System;
using System.Collections.Generic;

namespace CoreMVVM.IOC.Builder
{
    /// <summary>
    /// A collection of registrations mapped to types.
    /// </summary>
    internal class RegistrationCollection : Dictionary<Type, IRegistration>
    {
    }
}