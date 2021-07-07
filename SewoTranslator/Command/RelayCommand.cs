using System;
using System.Windows.Input;

namespace SewoTranslator.Command
{
    public class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;
        private Action<string> showEnglishDictionary;

        public RelayCommand(Action execute)
            : this(execute, null)
        {

        }

        public RelayCommand(Action<string> showEnglishDictionary)
        {
            this.showEnglishDictionary = showEnglishDictionary;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;
            return canExecute();
        }

        public void Execute(object parameter)
        {
            execute();
        }

        #endregion
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Func<T, bool> canExecute;

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {

        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            this.canExecute = canExecute;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute == null)
                return true;
            return canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException("Parameter cannot be null.");
            if (parameter is T)
            {
                T param = (T)parameter;

                execute(param);
            }
            else
            {
                throw new ArgumentException($"Invalid parameter type, type required: {typeof(T)}, type given: {parameter.GetType()}");
            }
        }

        #endregion
    }
}
