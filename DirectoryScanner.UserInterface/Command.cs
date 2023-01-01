using System;
using System.Windows.Input;

namespace DirectoryScanner.UserInterface
{
    public class Command : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object param)
        {
            return _canExecute(param);
        }

        public void Execute(object param)
        {
            _execute(param);
        }
    }
}
