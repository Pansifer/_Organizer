using PersonalOrganizer.MVVM.Class;
using PersonalOrganizer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PersonalOrganizer.MVVM.View
{
    public partial class DailyRoutineView : UserControl
    {
        List<GenericItem> DailyRoutineItems = new List<GenericItem>();
        XmlSaver DailyRoutineSaver;

        SolidColorBrush CoolBlueColor;
       
        DateTime todayRef;
       
        public DailyRoutineView()
        {
            InitializeComponent();

            //Get day reference for normalize data
            //Every day reset checked param
            todayRef = DateTime.Today;

            //crate a instance class for can save data in xml file.
            DailyRoutineSaver = new XmlSaver("DailyRoutineStorage.xml");
           
            //check if file exist, if file not exist create a empty list of generic item
            DailyRoutineItems = (List<GenericItem>)DailyRoutineSaver.ReadData(DailyRoutineItems);
            if (DailyRoutineItems == null) DailyRoutineItems = new List<GenericItem>();
           
            CoolBlueColor = _Color.CoolBlueColor;
           
            CreateView();
        }

        private void saveData()
        {
            //Before saving data, the method clean file path for eliminate the risk of the crash of the application
            //clean
            DailyRoutineSaver.CleanData();
            //create
            DailyRoutineSaver = new XmlSaver("DailyRoutineStorage.xml");
            //write
            DailyRoutineSaver.WriteData(DailyRoutineItems);
        }

        #region CreateViewUtils
        private void CreateView()
        {
            //Clear view
            GridContentWrapper.Children.Clear();
          
            //Order element from date
            var templist=new List<GenericItem>();
            while(DailyRoutineItems.Count > 0)
            {   
                var maxTime = DateTime.MaxValue;
                var beforItem = new GenericItem();
                foreach(var item in DailyRoutineItems)
                {
                    if(item.Date<maxTime) {
                        beforItem = item;
                        maxTime = item.Date;
                    }
                }
                DailyRoutineItems.Remove(beforItem);
                templist.Add(beforItem);
            }
            DailyRoutineItems.Clear();
         
            //Normalize other day daily routine
            foreach (var t in templist)
            {
                if(t.Date.ToString("dd/MM/yyyy") != todayRef.ToString("dd/MM/yyyy"))
                {
                    t.cheched = false;
                    t.Date = new DateTime(todayRef.Year, todayRef.Month, todayRef.Day, t.Date.Hour, t.Date.Minute,0);
                }
            }

            DailyRoutineItems.AddRange(templist);
            int count = 0;
            foreach(var i in DailyRoutineItems)
            {
                //set item id by order date
                i.ID = count;
                count++;
                GenerateItem(i);
            }

            //Save the new ordered list
            saveData();
        }
        private void GenerateItem(GenericItem item)
        {
            //Create content wrapper
            var ItemWrapper = new Border()
            {
                MinHeight = 50,
                MaxHeight = 300,
                Margin = new Thickness(0, 0, 0, 25),
                Background = item.cheched? _Color.DarkGreen : CoolBlueColor
            };
          
            //Set row by order index and add this on the grid container
            Grid.SetRow(ItemWrapper,item.ID);
            GridContentWrapper.Children.Add(ItemWrapper);

            //Add new row to the next Item
            GridContentWrapper.RowDefinitions.Add(new RowDefinition() { });

            //Create a contantainer for item data
            var wrapperGrid = new Grid()
            {
                Margin = new Thickness(0, 0, 0, 25),
                MinHeight = 50,
                MaxHeight = 300,
            };
            wrapperGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(150) });
            wrapperGrid.ColumnDefinitions.Add(new ColumnDefinition());
            wrapperGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            wrapperGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });
            //Add item data grid container to the item wrapper
            ItemWrapper.Child = wrapperGrid;

            //Create hour text block and add this to the first col
            var hourText = new TextBlock()
            {
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                Text = item.Date.Hour.ToString("00") + ":" + item.Date.Minute.ToString("00"),
                FontSize = 24,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 25, 0, 0),
            };
            Grid.SetColumn(hourText, 0);
            wrapperGrid.Children.Add(hourText);
            //Create content text block and add this to the second col
            var contentText = new TextBlock()
            {
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                Text = item.Description,
                FontSize = 24,
                Margin = new Thickness(0,25,0,0),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            wrapperGrid.Children.Add(contentText);
            Grid.SetColumn(contentText, 1);

            //Create trash button and add this to the tirth col
            var trashButton = new Button()
            {
                Background = Brushes.Transparent,
                Width= 25,
                VerticalAlignment= VerticalAlignment.Top,
                Margin = new Thickness(0,25,0,0)
            };
            Grid.SetColumn(trashButton, 3);
            trashButton.Click += trashButtonItem;
            wrapperGrid.Children.Add(trashButton);

            var TrashButtonImage = new Image()
            {
                Source = new BitmapImage(new Uri(@"/Images/trash.png", UriKind.RelativeOrAbsolute))
            };
            trashButton.Content = TrashButtonImage;
           
            
            //Create trash button and add this to the tirth col
            var checkButton = new Button()
            {
                Background = Brushes.Transparent,
                Width = 25,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 25, 0, 0)
            };
            Grid.SetColumn(checkButton, 2);
            checkButton.Click += checkButtonItem;
            wrapperGrid.Children.Add(checkButton);

            var CheckButtonImage = new Image()
            {
                Source = new BitmapImage(new Uri(@"/Images/confirm.png", UriKind.RelativeOrAbsolute))
            };
            checkButton.Content = CheckButtonImage;
        }
 
        #endregion
  
        #region UIEvents
        private void trashButtonItem(object sender, RoutedEventArgs e)
        {
            //get ItemView parent reference
            var button = e.Source as Button;//this is the button
            var gridParent = button.Parent as Grid;//this is the button wrapper
            var WrapperReference = gridParent.Parent as Border;//this is the item view wrapper
            
            //Get row index
            var indexRow = Grid.GetRow(WrapperReference);
            //Delete item from row index
            DailyRoutineItems.RemoveAt(indexRow);
            //Regenerate view
            CreateView();

        }
        private void checkButtonItem(object sender, RoutedEventArgs e)
        {
            //get ItemView parent reference
            var button = e.Source as Button;//this is the button
            var gridParent = button.Parent as Grid;//this is the button wrapper
            var WrapperReference = gridParent.Parent as Border;//this is the item view wrapper

            //Get row index
            var indexRow = Grid.GetRow(WrapperReference);
            //Set checked or dechecked  item from row index
            DailyRoutineItems[indexRow].cheched = !DailyRoutineItems[indexRow].cheched;
            //Regenerate view
            CreateView();

        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddDailyRoutineWrapper.Visibility = Visibility.Visible;
            AddButtonWrapper.Visibility = Visibility.Collapsed;
        }
        private void AddDailyRoutineButton_Click(object sender, RoutedEventArgs e)
        {
            if (DailyText.Text == "" || DailyText.Text == "Insert daily routine...(max 1000 words)")
            {
                //if user not set the content text, the input is null
                RemoveDailyRoutineButton_Click(sender, e);
                return;
            }

            //get the hour and minute value and parse them to the datetime type
            var hval = int.Parse(HourText.Header.ToString());
            var mval = int.Parse(MinuteText.Header.ToString());
            DateTime time = new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day,hval,mval,0);

            //create new item
            var item = new GenericItem();
            //initialize item
            item.Initialize(DailyText.Text, time, DailyRoutineItems.Count);
          
            //add item to items list
            DailyRoutineItems.Add(item);
           
            //Create view
            CreateView();

            //Close form
            RemoveDailyRoutineButton_Click(sender,e);
        }
        private void RemoveDailyRoutineButton_Click(object sender, RoutedEventArgs e)
        {
            //before closing form this is normalized with start value
            HourText.Header = "00";
            MinuteText.Header = "00";
            DailyText.Text = "Insert daily routine...(max 1000 words)";
            AddDailyRoutineWrapper.Visibility = Visibility.Collapsed;
            AddButtonWrapper.Visibility = Visibility.Visible;
        }
        private void SelectedMinute(object sender, RoutedEventArgs e)
        {
            //change text of the hour's menu 
            var text = e.Source as MenuItem;
            MinuteText.Header = text.Header;
        }
        private void SelectedHour(object sender, RoutedEventArgs e)
        {
            //change text of the minute's menu
            var text = e.Source as MenuItem;
            HourText.Header = text.Header;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //When user focus the content text form this remove the placeholder
            var text = e.Source as TextBox;
            if (text.Text == "Insert daily routine...(max 1000 words)")
                text.Text = "";
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //When user unfocus the content text without add other word the placeholder returned
            var text = e.Source as TextBox;
            if (text.Text == "")
                text.Text = "Insert daily routine...(max 1000 words)";
        }
        #endregion

    }
}
