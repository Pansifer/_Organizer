using PersonalOrganizer.Core;
using PersonalOrganizer.MVVM.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace PersonalOrganizer.MVVM.Model
{
    //it is a generic item
    public class GenericItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        
        public int ID;
        public bool cheched = false;
     
        //using this for initialize the class,
        //because the costructor if is not empty generate an error from xmlserializer
        public void Initialize(string itemText, string description, DateTime date, int id)
        {
            Title = itemText;
            Description = description;
            Date = date;
            ID = id;
        }
        public void Initialize(string description, DateTime date, int id)
        {
            Description = description;
            Date = date;
            ID = id;
        }
    }
}
