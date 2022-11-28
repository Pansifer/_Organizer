using PersonalOrganizer.MVVM.Class;
using PersonalOrganizer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PersonalOrganizer.MVVM.View
{
    public partial class HomeView : UserControl
    {

        GenericItem NoteItem;
        List<GenericItem> NoteItems = new List<GenericItem>();
        List<GenericItem> TaskList = new List<GenericItem>();
        List<GenericItem> DailyRoutineList = new List<GenericItem>();
        XmlSaver note_DataStorage;

        DateTime SelectedDate;

        public HomeView()
        {
            InitializeComponent();
            //Create a Time visualizer
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) }; //DispatcherTimer executes internal code every 'x' seconds
           
            //Set date and time text
            DateDisplay.Text = DateTime.Now.ToString("dd/MM/yyyy");
            TimerDisplay.Text = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00");
            
            //create and add a event handler that update time view text every tick time (1 sec) we have set this in timespan.fromsecond(x)
            timer.Tick += (sender, args) =>
            {
                TimerDisplay.Text = DateTime.Now.Hour.ToString("00") +":"+ DateTime.Now.Minute.ToString("00") +":"+ DateTime.Now.Second.ToString("00");
            };
            //start the tick event
            timer.Start();


            //Create a storage for the note
            note_DataStorage = new XmlSaver("NoteItemStorage.xml");
            //check if file exist, if file not exist create a empty list of generic item
            NoteItems = (List<GenericItem>)note_DataStorage.ReadData(NoteItems);
            if (NoteItems == null) NoteItems = new List<GenericItem>();

            //check if routineStorage exist
            var routineStorage = new XmlSaver("DailyRoutineStorage.xml");
            DailyRoutineList = (List<GenericItem>)routineStorage.ReadData(DailyRoutineList);
            if (DailyRoutineList == null) {
                DailyRoutineContainer.Text = "You haven't set a daily routine.";
            }
            else
            {
                if(DailyRoutineList.Count > 0)
                {
                    var checkCount = DailyRoutineList.Where(o => o.cheched).ToList().Count;
                   
                    if (checkCount != DailyRoutineList.Count)
                    {
                        DailyRoutineContainer.Text = checkCount + " of " + DailyRoutineList.Count + " completed";
                        DailyRoutineContainer.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    else
                    {
                        DailyRoutineContainer.Text = "Congratulation,\nyou have complete your daily routine!";
                        DailyRoutineContainer.HorizontalAlignment = HorizontalAlignment.Left;
                    }                       
                                            
                }
                else
                {
                    DailyRoutineContainer.Text = "You haven't set a daily routine.";
                }
            }

            //check if stackstorage exist
            var taskStorage = new XmlSaver("ToDoSaveStorage.xml");
            TaskList = (List<GenericItem>)taskStorage.ReadData(TaskList);
            if (TaskList == null)
            {
                todoContainer.Text = "You haven't any task today.";
            }
            else
            {
                var templist = TaskList.Where(x=>x.Date == DateTime.Today).ToList();
                if(templist.Count > 0)
                {
                    var checkCount = templist.Where(o => o.cheched).ToList().Count;

                    if (checkCount != templist.Count)
                    {
                        todoContainer.Text = checkCount + " of " + templist.Count + " completed";
                        todoContainer.HorizontalAlignment = HorizontalAlignment.Center;


                    }
                    else
                    {
                        todoContainer.Text = "Congratulation,\nyou have complete your daily task!";
                        DailyRoutineContainer.HorizontalAlignment = HorizontalAlignment.Left;

                    }

                }
                else
                {
                    todoContainer.Text = "You haven't any task today.";
                }
            }
              
            //selected today date
            SelectedDate = new DateTime();
            SelectedDate = DateTime.Today;
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");
            //Set not selectable future date
            DataPickerObject.BlackoutDates.Add(new CalendarDateRange(DateTime.Now.AddDays(1), DateTime.MaxValue));
            arrowRightCheck();

          
            CreateNote();
        }

        private void saveData()
        {
            //Before saving data, the method clean file path for eliminate the risk of the crash of the application
            //clean
            note_DataStorage.CleanData();
            //create
            note_DataStorage = new XmlSaver("NoteItemStorage.xml");
            //write
            note_DataStorage.WriteData(NoteItems);
        }
       
        #region NoteManager
        private void CreateNote()
        {
            //Search selected day's note
            foreach (var item in NoteItems)
            {
                if (item.Date == SelectedDate)
                {
                    NoteItem = item;
                    NoteContainer.Text = NoteItem.Description;
                    saveData();
                    return;
                }
            }

            //if not found that, create new 
            NoteItem = new GenericItem();
            NoteItem.Initialize("", SelectedDate, NoteItems.Count);
            NoteItems.Add(NoteItem);
            NoteContainer.Text = NoteItem.Description;
            saveData();

        }
#endregion
        #region UIUtils
        private void arrowRightCheck()
        {
            //check if right arrow must be visible
            if (SelectedDate == DateTime.Today) RightArrow.Visibility = Visibility.Hidden;
            else RightArrow.Visibility = Visibility.Visible;
        }
        private void SelectedViewFromDataPicker(object sender, SelectionChangedEventArgs e)
        {
            //Select day from data picker
            SelectedDate = (DateTime)DataPickerObject.SelectedDate;
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");
        }
        private void LeftArrowButton_Click(object sender, RoutedEventArgs e)
        {
            //select previous day date
            SelectedDate = SelectedDate.AddDays(-1);
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");
            arrowRightCheck();
            CreateNote();
        }
        private void RightArrowButton_Click(object sender, RoutedEventArgs e)
        {
            //select next day date
            SelectedDate = SelectedDate.AddDays(1);
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");
            arrowRightCheck();
            CreateNote();
        }
        #endregion

        private void NoteContainer_TextChanged(object sender, TextChangedEventArgs e)
        {
            //while user write save new text in storage
            NoteItem.Description = (e.Source as TextBox).Text;
            saveData();
        }
    }
}
