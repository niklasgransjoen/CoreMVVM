using System.ComponentModel;
using System.Windows;

namespace CoreMVVM.Windows
{
    /// <summary>
    /// Logic for checking if software is currently in design mode.
    /// </summary>
    public static class DesignHelper
    {
        /// <summary>
        /// Gets a bool indicating if the application is currently in design mode.
        /// </summary>
        public static bool IsDesignMode => (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
    }
}