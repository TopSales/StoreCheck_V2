using System.Diagnostics;

namespace ZPF.XF.Compos
{
    public class TileMenu : AbsoluteLayout
   {
      public double FontSize { get; set; } = 24;
      public double ImageScale { get; set; } = 1;

      public class Line
      {
         private TileMenu _Parent;

         double _Height = -1;
         public double Height
         {
            get { return _Height; }
            set
            {
               value = (value < -1 ? -1 : value);
               _Height = value;
            }
         }

         public Line(TileMenu Parent, double Height = -1)
         {
            _Parent = Parent;
            this.Height = Height;
         }

         public void AddDummy()
         {
            Tiles.Add(null);
         }

         public List<Tile> Tiles = new List<Tile>();


         public Tile AddTile(ImageSource imageSource, string Tag)
         {
            var t = new Tile
            {
               BackgroundColor = _Parent.TileBackgroundColor,
               Source = imageSource,
               FontSize = _Parent.FontSize,
            };

            if (!string.IsNullOrEmpty(Tag))
            {
               t.Text = Tag;
               t.CommandParameter = Tag;
            };

            Tiles.Add(t);

            return t;
         }

         public Tile AddTile(string iconChar, string Tag, string Hint)
         {
            return AddTile(iconChar, Tag, Hint, "");
         }

         public Tile AddTile(string iconChar, string Tag, string Hint = "", string CommandParameter = "")
         {
            var t = new Tile
            {
               BackgroundColor = _Parent.TileBackgroundColor,
               IconChar = iconChar,
               BoldText = _Parent.BoldText,
               FontSize = _Parent.FontSize,
               ImageScale = _Parent.ImageScale,
            };

            if (!string.IsNullOrEmpty(Tag))
            {
               t.Text = Tag;
               t.CommandParameter = Tag;
            };

            if (!string.IsNullOrEmpty(CommandParameter))
            {
               t.CommandParameter = CommandParameter;
            };

            if (!string.IsNullOrEmpty(Hint))
            {
               t.Hint = Hint;
            };

            Tiles.Add(t);

            return t;
         }
      }

      private Page _ParentPage;

      public Microsoft.Maui.Graphics.Color TileBackgroundColor { get; set; }
      public EventHandler OnClicked { get; set; }
      public double TopMargin { get; set; }
      public double BottomMargin { get; set; }
      public bool FixLeftMargin { get; set; }
      public bool BoldText { get; set; }

      public TileMenu(Page page)
      {
         _ParentPage = page;

         _ParentPage.SizeChanged += _ParentPage_SizeChanged;

         TopMargin = 0;
         BottomMargin = 0;
         FixLeftMargin = false;

         // - - -  - - - 

         VerticalOptions = LayoutOptions.FillAndExpand;
         HorizontalOptions = LayoutOptions.FillAndExpand;

         // - - -  - - - 

         //Tile.SkinImageScale = 1;
      }

      public List<Line> Lines = new List<Line>();

      private void _ParentPage_SizeChanged(object sender, System.EventArgs e)
      {
         Debug.WriteLine("_ParentPage_SizeChanged");

         if (CalcSize())
         {
            DrawTiles();
         };
      }

      public double DrawTiles(bool Simulate = false)
      {
         Debug.WriteLine("DrawTiles");

         if (!HasCalcSize) CalcSize();

         double PosY = (LeftMargin == 2 * _Margin ? 5 : LeftMargin);
         double PosX = LeftMargin;

         try
         {
            if (Device.Idiom == TargetIdiom.Desktop)
            {
               PosY = _Margin;
            };

            if (Device.RuntimePlatform == Device.Android)
            {
               PosY = -8;
            };

            // - - -  - - - 

            foreach (var l in Lines)
            {
               double w = Width3;

               double h = -1;

               switch (l.Tiles.Count)
               {
                  case 1:
                     w = Width1;
                     h = (l.Height == -1 ? Width2 : l.Height);
                     //PosX = _Margin * 2;
                     break;

                  case 2:
                     w = Width2;
                     h = (l.Height == -1 ? w : l.Height);
                     break;

                  default:
                     w = Width3;
                     h = (l.Height == -1 ? w : l.Height);
                     break;
               };


               if (!Simulate)
               {
                  foreach (var t in l.Tiles)
                  {
                     if (t != null)
                     {
                        t.PlaceTile(this, PosX, PosY, t.IconChar, t.Text, OnClicked, w, h);
                     };

                     PosX = PosX + w + _Margin;
                  };
               };

               PosX = LeftMargin;
               PosY = PosY + h + _Margin;
            };
         }
         catch (Exception ex)
         {
            Debug.WriteLine($"DrawTiles: {ex.Message}");
         };

         return PosY;
      }

      double LeftMargin = -1;
      double _Margin = -1;
      double Width1 = -1;
      double Width2 = -1;
      double Width3 = -1;


      Double PrevWidth = -1;
      Double PrevHeight = -1;

      bool HasCalcSize = false;
      double _LeftMargin = -1;

      private bool CalcSize()
      {
         Debug.WriteLine("CalcSize");

         HasCalcSize = true;

         Double sWidth = ZPF.XF.Display.Width;
         Double sHeight = ZPF.XF.Display.Height;

         if (PrevWidth == sWidth && PrevHeight == sHeight)
         {
            return false;
         }
         else
         {
            PrevWidth = sWidth;
            PrevHeight = sHeight;
         };

         Double scale = ZPF.XF.Display.Scale;

         Debug.WriteLine("CalcSize {0}  {1}  {2} ", sWidth, sHeight, scale);

         if (Device.RuntimePlatform == Device.Android)
         {
            sWidth = sWidth / scale;
            sHeight = sHeight / scale;
         };

         //Debug.WriteLine("Tiles display: {0}x{1}", sWidth, sHeight);
         //Debug.WriteLine("Tiles display: {0}x{1}", sWidth, sHeight - TopMargin);

         // - - -  - - - 

         Double uWidth = (sWidth > 450 ? uWidth = 450 : sWidth);
         Double uHeight = (sHeight > 900 ? uHeight = 900 : sHeight);

         do
         {
            _Margin = uWidth / 30;

            Width1 = (uWidth - ((2 + 2) * _Margin)) / 1;

            Width2 = (uWidth - ((2 + 3) * _Margin)) / 2;
            double Height2 = Width2;

            Width3 = (uWidth - ((2 + 4) * _Margin)) / 3;
            double Height3 = Width3;

            LeftMargin = 2 * _Margin;
            LeftMargin = (sWidth - (Width2 * 2 + _Margin)) / 2;

            if (FixLeftMargin)
            {
               if (_LeftMargin == -1)
               {
                  LeftMargin = (LeftMargin > TopMargin ? TopMargin : LeftMargin);
                  _LeftMargin = LeftMargin;

               }
               else
               {
                  LeftMargin = _LeftMargin;
               };
            };

            uWidth = uWidth * 0.95;

            var Title = (string.IsNullOrEmpty(_ParentPage.Title) ? "" : _ParentPage.Title);
            Debug.WriteLine("Tiles height ({0}): {1} > {2}", _ParentPage.Title, DrawTiles(Simulate: true), (sHeight - TopMargin - BottomMargin));
            Debug.WriteLine($"LeftMargin({LeftMargin}): {HasCalcSize} ");
         }
         while (DrawTiles(Simulate: true) > (sHeight - TopMargin - BottomMargin));

         //Debug.WriteLine("Tiles height: {0}, {1}", uHeight, d);

         return true;
      }


      public Line NewLine(double Height = -1)
      {
         var l = new Line(this, Height);
         Lines.Add(l);

         return l;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public Tile this[string Name]
      {
         get
         {
            try
            {
               foreach (var l in Lines)
               {
                  foreach (var t in l.Tiles)
                  {
                     if (t != null && t.CommandParameter != null && (string)(t.CommandParameter) == Name)
                     {
                        return t;
                     };
                  };
               };
            }
            catch { };

            return null;
         }

         //set
         //{
         //   int i;

         //   i = IndexOfName(Name);

         //   if ((value != null) && (value.Length != 0))
         //   {
         //      if (i < 0)
         //      {
         //         i = Add(Name + "=" + value);
         //      }
         //      else
         //      {
         //         Put(i, Name + "=" + value);
         //      };
         //   }
         //   else
         //   {
         //      if (i >= 0)
         //         Delete(i);
         //   };
         //}

      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   }
}
