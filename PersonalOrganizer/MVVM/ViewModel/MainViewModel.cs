using PersonalOrganizer.Core;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace PersonalOrganizer.MVVM.ViewModel
{
    class MainViewModel : ObjectWithNotification
    {

        //Create ref to command, it is use in menu button
        public CommandClass HomeViewCommand { get; set; }
        public CommandClass DailyRoutineViewCommand { get; set; }
        public CommandClass ToDoListViewCommand { get; set; }
        public CommandClass DaysThinksViewCommand { get; set; }

        //Create ref of view for setting current view
        public HomeViewModel HomeVM { get; set; }
        public DailyRoutineViewModel DailyRoutineVM { get; set; }
        public static ToDoListViewModel ToDoListVM { get; set; }
        public DaysThinksViewModel DaysThinksVM { get; set; }

        //this is the current view
        private object _currentView;

        //Current view notify the change to the client when it is set
        public object CurrentView
        {
            get { return _currentView; }
            set {
                _currentView = value;
                OnPropertyChanged();
                }
        }

        public MainViewModel()
        {
            //Create all view and set homeView as current view
            CurrentView = HomeVM = new HomeViewModel();
            DailyRoutineVM = new DailyRoutineViewModel();
            ToDoListVM= new ToDoListViewModel();
            DaysThinksVM= new DaysThinksViewModel();

            //define the command
            #region command
            HomeViewCommand = new CommandClass(o =>
            {
                CurrentView = HomeVM;
            }, o => { return true; });

            DailyRoutineViewCommand = new CommandClass(
                o => { CurrentView = DailyRoutineVM; },
                o => { return true; });

            ToDoListViewCommand = new CommandClass(
              o => { CurrentView = ToDoListVM; },
              o => { return true; });

            DaysThinksViewCommand = new CommandClass(
              o => { CurrentView = DaysThinksVM; },
              o => { return true; });
            #endregion
        }
    }
}
