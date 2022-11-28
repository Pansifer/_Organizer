using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PersonalOrganizer.Core
{
    class CommandClass : ICommand
    {
        //action to exectute when command is launch
        private Action<object> _execute;
        //function to verify if the command can be execute
        private Func<object, bool> _canExecute;

        public event EventHandler? CanExecuteChanged; //check if _canExtecute change
      
        //Get an action with object param and a func with object param that return a bool value
        public CommandClass(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }


        public bool CanExecute(object parameter)
        {
           return _canExecute == null || _canExecute(parameter); //the action can execute when _canexecute is null or if the function parsed return true
        }

        public void Execute(object parameter)
        {
            _execute(parameter); //Execute the action parsed
        }
    }
}
