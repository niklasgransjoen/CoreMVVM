using System;
using System.Linq;

namespace CoreMVVM.IOC.Core
{
    internal static class RegistrationCollectionExtensions
    {
        public static void AssertNoScopingConflicts<TComponent>(this RegistrationCollection registrations, ComponentScope scope)
        {
            AssertNoScopingConflicts(registrations, typeof(TComponent), scope);
        }

        public static void AssertNoScopingConflicts(this RegistrationCollection registrations, Type component, ComponentScope scope)
        {
            // Verify that all registrations have the same scope.
            var previousReg = registrations
                .Where(r => r.Value.Type == component && r.Value.Scope != scope)
                .FirstOrDefault();

            if (previousReg.Key != null)
                throw new ScopingConflictException(component, previousReg.Key, scope, previousReg.Value.Scope);
        }
    }
}