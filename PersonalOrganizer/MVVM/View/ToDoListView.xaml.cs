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
    public partial class ToDoListView : UserControl
    {
        List<GenericItem> ToDoListItems = new List<GenericItem>();
        XmlSaver ToDoSaveStorage;

        SolidColorBrush PrimaryColor;

        DateTime SelectedDate;

        int columIndex = 0;
        int rowIndex = 0;
        public ToDoListView()
        {
            InitializeComponent();

            PrimaryColor = _Color.CoolBlueColor;

            //crate a instance class for can save data in xml file.
            ToDoSaveStorage = new XmlSaver("ToDoSaveStorage.xml");

            //check if file exist, if file not exist create a empty list of generic item
            ToDoListItems = (List<GenericItem>)ToDoSaveStorage.ReadData(ToDoListItems);
            if (ToDoListItems == null) ToDoListItems = new List<GenericItem>();

            //selected today date
            SelectedDate = new DateTime();
            SelectedDate = DateTime.Today;
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy"); ;

            //Set not selectable future date
            DataPickerObject.BlackoutDates.Add(new CalendarDateRange(DateTime.Now.AddDays(1), DateTime.MaxValue));

            CreateView();
        }
        private void saveData()
        {
            //Before saving data, the method clean file path for eliminate the risk of the crash of the application
            //clean
            ToDoSaveStorage.CleanData();
            //create
            ToDoSaveStorage = new XmlSaver("ToDoSaveStorage.xml");
            //write
            ToDoSaveStorage.WriteData(ToDoListItems);
        }

        #region CreateToDoItems
        private void CreateView()
        {
            //if selected day is today it is not possible selecte future day, right arrow is'nt necessary
            if (SelectedDate == DateTime.Today) RightArrow.Visibility = Visibility.Hidden;
            else RightArrow.Visibility = Visibility.Visible;

            //clearView and reset index of cols and rows
            ClearView();

            //get ordinate list of item
            var todayList = UpdateList();

            //add new row in content grid
            ToDoTaskContentGrid.RowDefinitions.Add(new RowDefinition()
            {
                Height = new GridLength(400)
            }) ;

            //Create view for each item in today list
            for (int i = 0; i < todayList.Count; i++)
            {
                //if the last column is fill create a new row
                //and set new index for col and row
                if (columIndex == ToDoTaskContentGrid.ColumnDefinitions.Count)
                {
                    columIndex = 0; //start to first column in left position
                    rowIndex++;//create new row bottom the existent row

                    //add the new row to the grid
                    ToDoTaskContentGrid.RowDefinitions.Add(new RowDefinition()
                    {
                        Height = new GridLength(400)
                    });
                }
                //create item view and set grid position with the index
                var uiElement = createBorderElement(todayList[i]);
                Grid.SetRow(uiElement, rowIndex);
                Grid.SetColumn(uiElement, columIndex);

                //Add item into the grid container
                ToDoTaskContentGrid.Children.Add(uiElement);
                //move column index to right
                columIndex++;
            }

            //when all item is istantiated save data to the storage
            saveData();
        }
        private void AddToDoTask(object sender, RoutedEventArgs e)
        {
            if (ToDoTaskTextContainer.Text != "Insert to do content...                       (max 250 words)")
            {
                //create new generic item and initialize this
                ToDoListItems.Add(new GenericItem());
                ToDoListItems.Last().Initialize(ToDoTaskTextContainer.Text, SelectedDate, ToDoListItems.Count);

                //Create new view and close the form
                CreateView();
                AddFormIsNotVisible(sender, e);
            }
            else InsertToDoForm.Visibility = Visibility.Collapsed;

        }
        private void ClearView()
        {
            //Clear all child of content wrapper 
            ToDoTaskContentGrid.Children.Clear();

            //Clear existent row and reset index
            ToDoTaskContentGrid.RowDefinitions.Clear();
            columIndex = 0;
            rowIndex = 0;
        }
        private List<GenericItem> UpdateList()
        {
            //create temp list of today to do task
            var temp_todayToDoItems = ToDoListItems.Where(x => x.Date == SelectedDate).ToList();
            //get all checked todo task item and remove of the temp list
            var temp_todayToDoItemsChecked = temp_todayToDoItems.Where(x => x.cheched == true).ToList();
            temp_todayToDoItems.RemoveAll(x => temp_todayToDoItemsChecked.Contains(x));
            //Add now the checked item at the bottom of the temp list
            temp_todayToDoItems.AddRange(temp_todayToDoItemsChecked);

            //remove disordinate temp item in itemlist
            ToDoListItems.RemoveAll(x => temp_todayToDoItems.Contains(x));
            //add ordinate today item in main todo task items list
            ToDoListItems.AddRange(temp_todayToDoItems);

            //for each items in task list reassign id based on their position in the list
            for (int i = 0; i < ToDoListItems.Count; i++)
            {
                ToDoListItems[i].ID = i;
            }
            return temp_todayToDoItems;
        }
        private Border createBorderElement(GenericItem item)
        {
            //Create the content wrapper
            var BorderContainer = new Border()
            {
                Padding = new Thickness(10),
                Margin = new Thickness(0, 25, 0, 25),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 300,
                Background = item.cheched == false ? PrimaryColor : _Color.DarkGreen,
                CornerRadius = new CornerRadius(5),
            };
            //create the grid insiade the content wrapper
            //Create and Add two row to the grid
            var GridContainer = new Grid();

            RowDefinition row1 = new RowDefinition() { Height = new GridLength(300) };
            RowDefinition row2 = new RowDefinition() { Height = new GridLength(25) };

            GridContainer.RowDefinitions.Add(row1);
            GridContainer.RowDefinitions.Add(row2);

            //Add Grid inside the content wrapper 
            BorderContainer.Child = GridContainer;

            //Text container
            TextBlock TextContainer = new TextBlock()
            {
                Background = Brushes.Transparent,
                Foreground = Brushes.White,
                Text = item.Description,
                FontSize = 18,
                Padding = new Thickness(10),
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Top,
            };
            //set text container row position and add this to the grids
            Grid.SetRow(TextContainer, 0);
            GridContainer.Children.Add(TextContainer);

            //Create new Grid to wrap image button for check and delete task
            var GridIconWrapper = new Grid();

            //Add icon wrapper to content wrapper
            Grid.SetRow(GridIconWrapper, 1);
            GridContainer.Children.Add(GridIconWrapper);

            //define two columns and add to the icon grid
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            GridIconWrapper.ColumnDefinitions.Add(col1);
            GridIconWrapper.ColumnDefinitions.Add(col2);

            //Create check and delete button image and add these
            //inside the the icon grid
            var confirmButton = GenerateButtonImage("/Images/confirm.png");
            var trashButton = GenerateButtonImage("/Images/trash.png");
            Grid.SetColumn(confirmButton, 0);
            Grid.SetColumn(trashButton, 1);
            GridIconWrapper.Children.Add(confirmButton);
            GridIconWrapper.Children.Add(trashButton);

            //Add event listener to the button
            confirmButton.Click += ToDoItemCheckButton;
            trashButton.Click += ToDoItemTrashButton;

            //return view of the item
            return BorderContainer;
        }
        private Button GenerateButtonImage(string ImageSource)
        {
            //Create button
            var _button = new Button()
            {
                Background = Brushes.Transparent,
                Width = 50,
            };
            //create image
            var ButtonImage = new Image()
            {
                Source = new BitmapImage(new Uri(@"" + ImageSource, UriKind.RelativeOrAbsolute))
            };
            //add image inside the button
            _button.Content = ButtonImage;
            return _button;
        }
        #endregion
        #region UIEvents
        private void LeftArrowButton_Click(object sender, RoutedEventArgs e)
        {
            //select previous day date
            SelectedDate = SelectedDate.AddDays(-1);
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy"); ;
            CreateView();
        }
        private void RightArrowButton_Click(object sender, RoutedEventArgs e)
        {
            //select next day date
            SelectedDate = SelectedDate.AddDays(1);
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");
            CreateView();
        }
        private void SelectedViewFromDP(object sender, SelectionChangedEventArgs e)
        {
            //Select day from data picker
            SelectedDate = (DateTime)DataPickerObject.SelectedDate;
            DateString.Text = SelectedDate.ToString("dd/MM/yyyy");
            CreateView();
        }
        private void AddFormIsVisible(object sender, RoutedEventArgs e)
        {
            //when plus button is clicked the form become visible
            InsertToDoForm.Visibility = Visibility.Visible;
        }

        private void AddFormIsNotVisible(object sender, RoutedEventArgs e)
        {
            //if clicked trash button on form this closed him and reset form.
            InsertToDoForm.Visibility = Visibility.Collapsed;
            ToDoTaskTextContainer.Text = "Insert to do content...                       (max 250 words)";
        }

        private void ToDoItemCheckButton(object sender, RoutedEventArgs e)
        {
            //get ItemView parent reference
            var button = e.Source as Button; //this is the button
            var gridParent = button.Parent as Grid; //this is the button wrapper
            var secontGridParent = gridParent.Parent as Grid;//this is the grid container
            var WrapperReference = secontGridParent.Parent as Border; //this is the item view wrapper

            //get row and col index
            var indexRow = Grid.GetRow(WrapperReference);
            var IndexCol = Grid.GetColumn(WrapperReference);
            //calc the index by item position in the grid
            int IndexList = (IndexCol + (indexRow * ToDoTaskContentGrid.ColumnDefinitions.Count));

            //get the item of the selected date
            var tempList = ToDoListItems.Where(x => x.Date == SelectedDate).ToList();
            //get the element at the grid position 
            var tempItem = tempList[IndexList];

            //Checked this element 
            ToDoListItems[tempItem.ID].cheched = !ToDoListItems[tempItem.ID].cheched;

            //Regenarate the view for reorder element
            CreateView();
        }
        private void ToDoItemTrashButton(object sender, RoutedEventArgs e)
        {
            //get ItemView parent reference
            var button = e.Source as Button;//this is the button
            var gridParent = button.Parent as Grid;//this is the button wrapper
            var secontGridParent = gridParent.Parent as Grid;//this is the grid container
            var WrapperReference = secontGridParent.Parent as Border;//this is the item view wrapper

            //get row and col index
            var indexRow = Grid.GetRow(WrapperReference);
            var IndexCol = Grid.GetColumn(WrapperReference);
            //calc the index by item position in the grid
            int IndexList = IndexCol + (indexRow * ToDoTaskContentGrid.ColumnDefinitions.Count);

            //get the item of the selected date
            var tempList = ToDoListItems.Where(x => x.Date == SelectedDate).ToList();
            //get the element at the grid position 
            var tempItem = tempList[IndexList];

            //Remove this element
            ToDoListItems.RemoveAt(tempItem.ID);

            //Regenare view
            CreateView();
        }
        private void AddPlaceHolderText(object sender, RoutedEventArgs e)
        {
            //if losting focus inside a form without insert text 
            //this returned with start state
            if (ToDoTaskTextContainer.Text == "")
            {
                //adding a placeholder
                ToDoTaskTextContainer.Text = "Insert to do content...                       (max 250 words)";
            }
        }
        private void RemovePlaceHolderText(object sender, RoutedEventArgs e)
        {
            //if i focused the content form remove the placeholder
            if (ToDoTaskTextContainer.Text == "Insert to do content...                       (max 250 words)")
            {
                ToDoTaskTextContainer.Text = "";
            }
        }

        #endregion
    }
}
