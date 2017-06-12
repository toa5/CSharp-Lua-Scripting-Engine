using System;
using System.Windows.Input;

namespace WpfExample
{
    public class Command : ICommand
    {
        public Action Action { get; set; }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action?.Invoke();
        }
    }
}
