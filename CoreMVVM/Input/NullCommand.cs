﻿using System;
using System.Windows.Input;

namespace CoreMVVM
{
    /// <summary>
    /// An empty command.
    /// </summary>
    public sealed class NullCommand : ICommand
    {
        public static readonly ICommand Instance = new NullCommand();

        private NullCommand()
        {
        }

        event EventHandler ICommand.CanExecuteChanged { add { } remove { } }

        bool ICommand.CanExecute(object parameter) => true;

        void ICommand.Execute(object parameter)
        { }
    }
}