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
   public partial class EditEtatEmplacementView : ContentView
   {
      public EditEtatEmplacementView()
      {
         InitializeComponent();
      }

      public static bool Promo { get; set; } = false;

      public static async void Display(ContentPage parent, Grid mainGrid, System.Action callBack)
      {
         MainViewModel.Current.LogMemory("Before EditEtatEmplacementView");

         var view = new EditEtatEmplacementView();

         var tiles = new List<AppBarItem>(new AppBarItem[]
         {
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Happy_01_WF), "Yes"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Annoyed_WF ), "No"),
         });

         tiles[0].Clicked += (object s, EventArgs ea) =>
         {
            EditEtatEmplacementView.Promo = true;
         };
         tiles[1].Clicked += (object s, EventArgs ea) =>
         {
            EditEtatEmplacementView.Promo = false;
         };

         var h = ZPF.XF.Basics.Current.Display.Height;
         var sc = ZPF.XF.Basics.Current.Display.Scale;
         var h2 = h / sc;

         GridDlgOnTop.CustomPadding = new Thickness(30, h2 - 220, 30, 30);

         await GridDlgOnTop.DlgOnTop(mainGrid, view, tiles, 500, GridDlgOnTop.MarginWidths.custom, view.PostInit);

         // - - - save infos - - -

         if (string.IsNullOrEmpty(MainViewModel.Current.CurrentArticle.Guid))
         {
            // - - - new article - - -
            InterventionsViewModel.Current.AddToScann(new Intervention_Params.Scann
            {
               EAN = MainViewModel.Current.CurrentArticle.EAN,
               Brand = MainViewModel.Current.CurrentArticle.Brand,
               Name = MainViewModel.Current.CurrentArticle.Name,
               Condi = MainViewModel.Current.CurrentArticle.Condi,
               Price = MainViewModel.Current.CurrentArticle.Price,
               State = 1,
               // Facing 
               Promo = EditEtatEmplacementView.Promo,
               // QtStock
            });
         }
         else
         {
            // - - - existing article - - -
            InterventionsViewModel.Current.AddToScann(new Intervention_Params.Scann
            {
               Guid = MainViewModel.Current.CurrentArticle.Guid,
               EAN = MainViewModel.Current.CurrentArticle.EAN,
               //Brand = MainViewModel.Current.CurrentArticle.Brand,
               //Libelle = MainViewModel.Current.CurrentArticle.Label_FR,
               //Condi = MainViewModel.Current.CurrentArticle.Condi,
               Price = MainViewModel.Current.CurrentArticle.Price,
               State = 1,
               // Facing 
               Promo = EditEtatEmplacementView.Promo,
               // QtStock
            });
         };

         //DependencyService.Get<IScanner>().OpenScanner();

         MainViewModel.Current.LogMemory("Near End EditEtatEmplacementView");

         if (callBack != null)
         {
            callBack();
         };
      }

      void PostInit()
      {
         List<View> list = XFHelper.GetAllChildren(this.Parent);

         try
         {
            var tiles = list.Where(x => x is Tile).ToArray();

            (tiles[0] as Tile).BackgroundColor = Xamarin.Forms.Color.Green.MultiplyAlpha(.7);
            (tiles[1] as Tile).BackgroundColor = Xamarin.Forms.Color.Orange.MultiplyAlpha(.7);
         }
         catch
         {
         };
      }
   }
}
