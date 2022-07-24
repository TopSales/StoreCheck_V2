using System;
using System.Globalization;
using System.Diagnostics;

#if WPF
using System.Windows.Data;
#endif

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

#if PCL
using Xamarin.Forms;
#endif

namespace ZPF.USR
{
   // <!--<TextBlock Text="{Binding Path=ReleaseDate, Mode=OneWay, Converter={StaticResource FormatConverter}, ConverterParameter=Yes|Non}}" />-->

   public class CheckUserLevelConverter : IValueConverter
   {
      // This converts the DateTime object to the string to display.
#if WPF || WINDOWS_PHONE || PCL
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object Convert(object value, Type targetType, object parameter, string language)
#endif
      {
         return Check(value, targetType, parameter, culture);
      }

      public static bool Check(object value, Type targetType, object parameter, CultureInfo culture)
      {
         var tStrings = value as TStrings;
         if (tStrings == null)
         {
            return false;
         }

         if (parameter == null)
         {
            return false;
         }

         String s = (parameter as String);

         Debug.WriteLine(s + (tStrings.IndexOf(s) > -1));

         return (tStrings.IndexOf(s) > -1);
      }

      // No need to implement converting back on a one-way binding
#if WPF || WINDOWS_PHONE || PCL
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
      {
         return null;
      }
   }

   // - - -  - - -

   public class VisibilityUserLevelConverter : IValueConverter
   {
      // This converts the DateTime object to the string to display.
#if WPF || WINDOWS_PHONE || PCL
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object Convert(object value, Type targetType, object parameter, string language)
#endif
      {
         if (parameter == null)
         {
            return false;
         }

         String s = (parameter as String);


         //ToDo: bof!!! VisibilityUserLevelConverter --> UserViewModel.Current.CheckRights(s) 
         return (UserViewModel.Current.CheckRights(s) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden);
      }

      // No need to implement converting back on a one-way binding
#if WPF || WINDOWS_PHONE || PCL
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
      {
         return null;
      }
   }

   // - - -  - - -

   public class HasUserLevelConverter : IValueConverter
   {
      // This converts the DateTime object to the string to display.
#if WPF || WINDOWS_PHONE || PCL
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object Convert(object value, Type targetType, object parameter, string language)
#endif
      {
         if (parameter == null)
         {
            return false;
         }

         String s = (parameter as String);

         //ToDo: bof!!! HasUserLevelConverter --> UserViewModel.Current.CheckRights(s) 
         return UserViewModel.Current.CheckRights(s);
      }

      // No need to implement converting back on a one-way binding
#if WPF || WINDOWS_PHONE || PCL
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
      {
         return null;
      }
   }
}


