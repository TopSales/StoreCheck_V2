using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.XF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ScanView : ContentView
   {
      public ScanView()
      {
         InitializeComponent();

         DoIt.Delay(100, () => 
         {
            DoIt.OnMainThread(() => 
            {
               entry.Focus();
            });
         });
      }

      public static string Data { get; set; } = "";

      public static async void Display(Page parent, Grid mainGrid)
      {
         var view = new ScanView();

         var tiles = new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete ), "cancel"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Barcode_01), "scan"),
         });

         tiles[0].Clicked += (object s, EventArgs ea) =>
         {
            ScanView.Data = "";
         };
         tiles[1].Clicked += (object s, EventArgs ea) =>
         {
            if (!string.IsNullOrEmpty(view.entry.Text))
            {
               var st = view.entry.Text.Trim();

               if (!string.IsNullOrEmpty(st))
               {
                  ScanView.Data = st;
               };
            };
         };

         var h = ZPF.XF.Basics.Current.Display.Height;
         var sc = ZPF.XF.Basics.Current.Display.Scale;
         var h2 = h / sc;

         GridDlgOnTop.CustomPadding = new Thickness(30, h2 - 30, 30, 30);

         await GridDlgOnTop.DlgOnTop(mainGrid, view, tiles, 500, GridDlgOnTop.MarginWidths.custom, view.PostInit);

         if (!string.IsNullOrEmpty(ScanView.Data))
         {
            UnitechViewModel.Current.NewBarcode(ScanView.Data, ScanView.Data.Length, 0, null);
         };
      }

      void PostInit()
      {
         //List<View> list = XFHelper.GetAllChildren(this.Parent);
         //var tile = list.Where(x => x is Tile).ToArray()[1] as Tile;

         //if (tile != null) tile.IsEnabled = false;
      }
   }
}
