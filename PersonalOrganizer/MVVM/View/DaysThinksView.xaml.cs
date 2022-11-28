using PersonalOrganizer.MVVM.Class;
using PersonalOrganizer.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PersonalOrganizer.MVVM.View
{
    public partial class DaysThinksView : UserControl
    {

        List<GenericItem> DayThinksListItems = new List<GenericItem>();
        SolidColorBrush CoolBlueColor;

        XmlSaver dt_DataStorage;
        DateTime SelectedDate;

        GenericItem currentItem;
        public DaysThinksView()
        {
            InitializeComponent();
            //crate a instance class for can save data in xml file.
            dt_DataStorage = new XmlSaver("DaysThinksItemsStorage.xml");
           
            //check if file exist, if file not exist create a empty list of generic item
            DayThinksListItems = (List<GenericItem>)dt_DataStorage.ReadData(DayThinksListItems);
            if(DayThinksListItems==null)DayThinksListItems = new List<GenericItem>();

            CoolBlueColor = _Color.CoolBlueColor;
           
            //selected today date
            SelectedDate = new DateTime();
            SelectedDate = DateTime.Today;
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");
            //Set not selectable future date
            DataPickerObject.BlackoutDates.Add(new CalendarDateRange(DateTime.Now.AddDays(1), DateTime.MaxValue));
           
            CreateView();

        }

        private void saveData()
        {
            //Before saving data, the method clean file path for eliminate the risk of the crash of the application
            //clean
            dt_DataStorage.CleanData();
            //create
            dt_DataStorage = new XmlSaver("DaysThinksItemsStorage.xml");
            //write
            dt_DataStorage.WriteData(DayThinksListItems);
        }


        #region CreateViewUtils
        public void CreateView()
        {
            //if selected day is today it is not possible selecte future day, right arrow is'nt necessary
            if (SelectedDate == DateTime.Today) RightArrow.Visibility = Visibility.Hidden;
            else RightArrow.Visibility = Visibility.Visible;
          
            //Searching item with today date in storage,
            //if this exist create view with this.
            foreach (var item in DayThinksListItems)
            {
                if (item.Date.ToString("dd/MM/yyyy") == SelectedDate.ToString("dd/MM/yyyy"))
                {
                    CreateItemView(item,false);
                    return;
                }
            }

            //If the control has a negative result,
            //I create view with new item, with default value
                DayThinksListItems.Add(new GenericItem());
                DayThinksListItems.Last().Initialize("Title", "Description", SelectedDate, DayThinksListItems.Count);
                CreateItemView(DayThinksListItems.Last(),true);
        }

        public void CreateItemView(GenericItem item,bool isNew)
        {
            //save current item for update value when user change text in textbox
            currentItem = item;
            
            //definitionRow
            RowDefinition row1 = new RowDefinition();
            RowDefinition row2 = new RowDefinition();
            
            //create grid for wrapping the item
            Grid _Grid = new Grid();
            daysthinksWrapper.Children.Add(_Grid);
            _Grid.RowDefinitions.Add(row1);
            _Grid.RowDefinitions.Add(row2);

            //Create title form
            Border TitleWrapper = new Border()
            {
                Margin = new Thickness(0, 0, 0, 15),
                CornerRadius = new CornerRadius(5, 5, 5, 5),
                Background = CoolBlueColor
            };
            Grid.SetRow(TitleWrapper, 0);
            TextBox TitleText = new TextBox()
            {
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                VerticalAlignment = VerticalAlignment.Center,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Margin = new Thickness(15, 0, 15, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                Height = 75,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 24,
                Text = item.Title,
                Foreground = Brushes.White
            };

            TitleText.TextChanged += TitleTextChanged;
            TitleText.GotFocus += titleFocus;
            TitleText.LostFocus += titleLostFocus;
            TitleWrapper.Child = TitleText;
           
            _Grid.Children.Add(TitleWrapper);

            //Create contetForm
            Border ContentWrapper = new Border()
            {
                CornerRadius = new CornerRadius(5, 5, 5, 5),
                Background = CoolBlueColor
            };
            Grid.SetRow(ContentWrapper, 1);
            TextBox ContentText = new TextBox()
            {
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
                VerticalAlignment = VerticalAlignment.Top,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Margin = new Thickness(15, 15, 15, 15),
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                Height = 375,
                FontSize = 20,
                Text = item.Description,
                Foreground = Brushes.White

            };
           
            
            ContentText.TextChanged += ContentTextChanged;
            ContentText.GotFocus += ContentFocus;
            ContentText.LostFocus += ContentLostFocus;


            ContentWrapper.Child = ContentText;
            _Grid.Children.Add(ContentWrapper);
        }

        private void ClearView()
        {
            //Clear all child of content wrapper 
            daysthinksWrapper.Children.Clear();
        }
        #endregion

        #region uiUtils
        private void SelectedViewFromDataPicker(object sender, SelectionChangedEventArgs e)
        {
            //Select day from data picker
            SelectedDate = (DateTime)DataPickerObject.SelectedDate;
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");

            ClearView();
            CreateView();
        }
        private void LeftArrowButton_Click(object sender, RoutedEventArgs e)
        {
            //select previous day date
            SelectedDate = SelectedDate.AddDays(-1);
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");

            ClearView();
            CreateView();
        }
        private void RightArrowButton_Click(object sender, RoutedEventArgs e)
        {
            //select next day date
            SelectedDate = SelectedDate.AddDays(1);
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");

            ClearView();
            CreateView();
        }
        private void ContentFocus(object sender, RoutedEventArgs e)
        {
            //remove placeholder text
            var text = e.Source as TextBox;
            if (text.Text == "Description")
                text.Text = "";
        }

        private void ContentLostFocus(object sender, RoutedEventArgs e)
        {
            //add placeholder text
            var text = e.Source as TextBox;
            if (text.Text == "")
                text.Text = "Description";
        }

        private void titleLostFocus(object sender, RoutedEventArgs e)
        {
            //add placeholder text
            var text = e.Source as TextBox;
            if (text.Text == "")
                text.Text = "Title";
        }

        private void titleFocus(object sender, RoutedEventArgs e)
        {
            //remove placeholder text
            var text = e.Source as TextBox;
            if (text.Text == "Title")
                text.Text = "";
        }

        private void TitleTextChanged(object sender, TextChangedEventArgs e)
        {
            //if title text change save this value in item and in storage
            var t = e.Source as TextBox;
            currentItem.Title = t.Text;
            saveData();
        }
        private void ContentTextChanged(object sender, TextChangedEventArgs e)
        {
            //if content text change save this value in item and in storage
            var t = e.Source as TextBox;
            currentItem.Description = t.Text;
            saveData();
        }

        #endregion
    }


}
