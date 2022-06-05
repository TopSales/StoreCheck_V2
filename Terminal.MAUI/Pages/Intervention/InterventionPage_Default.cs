using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   public partial class InterventionPage : PageEx
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void GetMenuDefault(TileMenu tm)
      {
         {
            var l = tm.NewLine();
            t1 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Data_Import), "arrived");

            switch ((long)MainViewModel.Current.SelectedInterventionParams.FKActionType)
            {
               case (long)FKActionTypes.MultipleChoice:
                  t21 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Document_Check), "report");
                  break;

               case (long)FKActionTypes.ReleveLinairaire:
                  t21 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Inventory2), "shelf");
                  t22 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Barcode_01), "SLIM");
                  break;
            }
         };

         // - - -  - - - 

         {
            var l = tm.NewLine();
            if (MainViewModel.Current.SelectedInterventionParams.GlobalPictures) tPhotos = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF), "photos");
            t4 = l.AddTile(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Check_Mark_01), "validate");
            t4.BackgroundColor = Microsoft.Maui.Graphics.Colors.Green.MultiplyAlpha( 0.7f );
         };
      }

      private void UpdateTilesDefault()
      {
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
