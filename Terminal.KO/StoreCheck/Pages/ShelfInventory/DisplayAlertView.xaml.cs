using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF.XF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class DisplayAlertView : ContentView
   {
      public DisplayAlertView(string title, string message)
      {
         InitializeComponent();

         lTitle.Text = title;
         lMessage.Text = message;
      }

      public static Task<string> Display(ContentPage parent, Grid mainGrid, string title, string message, string accept, string cancel = "")
      {
         var view = new DisplayAlertView(title, message);

         var tiles = new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01 ), accept),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete), cancel),
         });

         if (cancel == "") tiles.RemoveAt(1);

         var h = ZPF.XF.Basics.Current.Display.Height;
         var sc = ZPF.XF.Basics.Current.Display.Scale;
         var h2 = h / sc;

         GridDlgOnTop.CustomPadding = new Thickness(30, h2 - 300, 30, 30);

         return GridDlgOnTop.DlgOnTop(mainGrid, view, tiles, 500, GridDlgOnTop.MarginWidths.custom);

         //return GridDlgOnTop.DlgOnTop(mainGrid, view, tiles, 500, GridDlgOnTop.MarginWidths.narrow);
      }
   }
}
