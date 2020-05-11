using System;
using System.Windows.Input;

namespace QAHelper.WPF
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _executeMethod;
        private readonly Func<object, bool> _canExecuteMethod;

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod = null)
            : this((obj) =>
            {
                if (executeMethod == null)
                {
                    return;
                }
                executeMethod();
            },
            (obj) => canExecuteMethod == null ? true : canExecuteMethod())
        { }

        public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod = null)
        {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;

            if (_executeMethod == null)
            {
                _executeMethod = (obj) => { };
            }

            if (_canExecuteMethod == null)
            {
                _canExecuteMethod = (obj) => true;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteMethod(parameter);
        }

        public void Execute(object parameter)
        {
            _executeMethod(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}