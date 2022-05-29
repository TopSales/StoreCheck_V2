using System;
using System.Collections.Generic;
using System.Text;

namespace ZPF.XF.Compos
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public class AppBarItem
   {
      public string IconChar { get; set; }
      public string Text { get; set; }
      public bool IsCancel { get; set; } = true;

      /// <summary>
      /// Occurs when the item is clicked.
      /// </summary>
      public event EventHandler Clicked;

      public AppBarItem(string iconChar, string text)
      {
         IconChar = iconChar;
         Text = text;
      }

      public void DoClick()
      {
         if (Clicked != null)
         {
            Clicked(this, null);
         };
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}
