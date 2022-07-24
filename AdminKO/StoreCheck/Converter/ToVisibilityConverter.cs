using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ZPF
{
   public class ToVisibilityConverter : IValueConverter
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
#if WPF || WINDOWS_PHONE  || PCL
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object Convert(object value, Type targetType, object parameter, string language)
#endif   
      {
         Visibility Result = Visibility.Collapsed;

         if (value != null)
         {
            Result = Visibility.Visible;
         }

         if (value is bool)
         {
            Result = (((bool)value == true ? Visibility.Visible : Visibility.Collapsed));
         }

         if (value is int)
         {
            int p = 0;
            string st = "";

            if (parameter is string)
            {
               st = parameter as string;
            };

            if (st.ToUpper() == "ZERO")
            {
               Result = (((int)value > 0 ? Visibility.Visible : Visibility.Collapsed));
            }
            else if (int.TryParse(st, out p))
            {
               Result = (((int)value == p ? Visibility.Visible : Visibility.Collapsed));
            }
            else
            {
               Result = (((int)value > -1 ? Visibility.Visible : Visibility.Collapsed));
            };
         }

         if (value is string)
         {
            if (String.IsNullOrEmpty((string)value))
            {
               Result = Visibility.Collapsed;
            }
            else
            {
               if ((string)value == "0")
               {
                  Result = Visibility.Collapsed;
               }
               else
               {
                  Result = Visibility.Visible;
               };
            };
         }

         if (value is ICollection)
         {
            Result = ((value as ICollection).Count == 0 ? Visibility.Collapsed : Visibility.Visible);
         };

         if (parameter is string)
         {
            if ((parameter as string) == "!")
            {
               Result = (Result == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed);
            };
         };

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
#if WPF || WINDOWS_PHONE || PCL
      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#endif
#if NETFX_CORE
      public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
      {
         throw new NotImplementedException();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
