using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PersonalOrganizer.MVVM.Class
{
    public static class _Color
    {
        public static SolidColorBrush CoolBlueColor;
        public static SolidColorBrush LightGreenColor;
        public static SolidColorBrush DarkGreen;

        public static void getResources()
        {
            CoolBlueColor = Application.Current.Resources["CoolBlue"] as SolidColorBrush;
            LightGreenColor = Application.Current.Resources["LightGreen"] as SolidColorBrush;
            DarkGreen = Application.Current.Resources["DarkGreen"] as SolidColorBrush;
        }
    }
}
