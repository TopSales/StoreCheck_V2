using System;
using System.Globalization;

#if PCL
using Xamarin.Forms;
#endif

#if WINDOWS_PHONE
using System.Windows.Data;
using System.Windows;
#endif

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

#if  WPF
using System.Windows.Data;
using System.Windows;
#endif


namespace ZPF.Conv
{
   // <!--<TextBlock Text="{Binding Path=ReleaseDate, Mode=OneWay, Converter={StaticResource FormatConverter}, ConverterParameter=\{0:datePicker\}}" />-->

   public class Dot2CommaConverter : IValueConverter
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

#if WPF || WINDOWS_PHONE  || PCL
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object Convert(object value, Type targetType, object parameter, string language)
#endif
      {
         if (value == null)
         {
            return "";
         };

         if (value is string)
         {
            string st = (string)value;

            return st.Replace(".", ",");
         }

         return value.ToString();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

#if WPF || WINDOWS_PHONE || PCL
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
      {
         if (value == null)
         {
            return "";
         };

         if (value is string)
         {
            string st = (string)value;

            return st.Replace(",", ".");
         }

         return value.ToString();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
