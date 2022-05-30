namespace ZPF.XF.Compos
{
    public class RadioButton : CheckBoxZPF
   {
      public RadioButton()
      {
         this.Clicked += RadioButton_Clicked;
         base.SizeChanged += new EventHandler(OnSizeChanged);
      }

      private void OnSizeChanged(object sender, EventArgs e)
      {
         //if (base.Height > 0)
         //{
         //    base.WidthRequest = base.Height;
         //}

         int fontSize = (int)FontSize;

         //base.ImageSourceChecked = ZPF.XF.Compos.SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.CircledCheckBox_01, fontSize, TextColor);
         //base.ImageSourceUnchecked = ZPF.XF.Compos.SkiaHelperXF.SkiaFontIcon(ZPF.Fonts.IF.Circle_01, fontSize, TextColor);
         ImageSourceChecked = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.CircledCheckBox_01);
         ImageSourceUnchecked = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Circle);

         ColorChecked = TextColor;
         ColorUnchecked = TextColor;

         base.RaiseCheckedChanged();
      }

      private void RadioButton_Clicked(object sender, EventArgs e)
      {
         if (!this.Checked)
         {
            this.Checked = true;
         }
         else
         {
            IList<IView> children = null;

            if (Parent is StackLayout)
            {
               children = (Parent as StackLayout).Children;
            };

            if (Parent is Grid)
            {
               children = (Parent as Grid).Children;
            };

            if (children != null)
            {
               foreach (var c in children)
               {
                  if (c is RadioButton)
                  {
                     var r = (c as RadioButton);

                     if (r == this)
                     {
                        r.Checked = true;
                     }
                     else
                     {
                        r.Checked = false;
                     };
                  };
               }
            }
         };
      }
   }
}
