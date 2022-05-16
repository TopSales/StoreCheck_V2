using Android.App;
using Android.Content;
using Android.Net;
using System;
using System.Diagnostics;
using ZPF.XF;

[assembly: Xamarin.Forms.Dependency(typeof(Scanner))]
namespace ZPF.XF
{
   /// <summary>
   /// </summary>
   public class Scanner : IScanner
   {
      bool _IsOpen = false;

      /// <summary>
      /// Turn on the power for the bar code reader. 
      /// </summary>
      /// <returns></returns>
      public  bool OpenScanner()
      {
         _IsOpen = true;

         // Turn on the power for the bar code reader. 
         return UnitechHelper.OpenScanner();
      }

      /// <summary>
      /// Turn off the power for the bar code reader. 
      /// </summary>
      /// <returns></returns>
      public  bool CloseScanner()
      {
         _IsOpen = false;

         // Turn on the power for the bar code reader. 
         return UnitechHelper.CloseScanner();
      }

      public bool EnableAllSymbologies()
      {
         return UnitechHelper.EnableAllSymbologies();
      }

      public bool EnableEAN13Only()
      {
         return UnitechHelper.EnableEAN13Only();
      }
      public bool EnableAllEANs()
      {
         return UnitechHelper.EnableAllEANs();
      }

      public bool IsOpen()
      {
         return _IsOpen;
      }
   }
}
