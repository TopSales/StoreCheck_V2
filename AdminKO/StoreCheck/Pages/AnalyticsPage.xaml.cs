using System.Collections.Generic;
using System.Windows.Controls;
using Microcharts;
using SkiaSharp;
using ZPF;

namespace StoreCheck.Pages
{
   /// <summary>
   /// Interaction logic for AnalyticsPage.xaml
   /// </summary>
   public partial class AnalyticsPage : Page
   {
      public AnalyticsPage()
      {
         InitializeComponent();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
      // https://blog.xamarin.com/microcharts-elegant-cross-platform-charts-for-any-app/
      // https://github.com/aloisdeniel/Microcharts

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void SKElement_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
      {
         SKImageInfo info = e.Info;
         SKSurface surface = e.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         SKPaint paint = new SKPaint
         {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Red,
            StrokeWidth = 25
         };

         //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);

         // - - -  - - - 

         var entries = new[]
         {
            new ChartEntry(212)
            {
               Label = "UWP",
               XValue=0,
               ValueLabel = "212",
               Color = Color.Parse("#2c3e50")
            },
            new ChartEntry(248)
            {
               Label = "Android",
               XValue=1,
               ValueLabel = "248",
               Color = Color.Parse("#77d065")
            },
            new ChartEntry(128)
            {
               Label = "iOS",
               XValue=2,
               ValueLabel = "128",
               Color = Color.Parse("#b455b6")
            },
            new ChartEntry(514)
            {
               Label = "Shared",
               XValue=3,
               ValueLabel = "514",
               Color = Color.Parse("#3498db")
            } };

         var chart = new LineChart()
         {
            Entries = entries,
            LabelTextSize = 24,
            IsAnimated = false,
            LineMode = LineMode.Straight,
            ValueLabelOrientation = Microcharts.Orientation.Horizontal,
            // LineMode = LineMode.Spline
         };

         chart.Draw(canvas, e.Info.Width, e.Info.Height);

         // - - -  - - - 

         paint.Color = SKColors.Blue;
         canvas.DrawCircle(e.Info.Width / 2, e.Info.Height / 2, e.Info.Height / 10, paint);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void SKElement_PaintSurfaceA(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
      {
         SKImageInfo info = e.Info;
         SKSurface surface = e.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         List<ChartEntry> entries = new List<ChartEntry>();

         //if (StockCalcViewModel.Current.DashDonatFamilles.Count > 0)
         //{
         //   foreach (var i in StockCalcViewModel.Current.DashDonatFamilles)
         //   {
         //      var entry = new ChartEntry((float)i.Value)
         //      {
         //         Label = i.Name,
         //         XValue = entries.Count,
         //         ValueLabel = i.Value.ToString(),
         //         Color = Color.Parse(colors[entries.Count % 7])
         //      };

         //      entries.Add(entry);
         //   };
         //};

         var chart = new Microcharts.DonutChart()
         {
            Entries = entries,
            LabelTextSize = 24,
            IsAnimated = false,
            Radius = 0.8F,
         };

         chart.Draw(canvas, e.Info.Width, e.Info.Height);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      // http://www.perceptualedge.com/articles/visual_business_intelligence/rules_for_using_color.pdf

      string[] colors = new string[]{
         "#99737373", "#99f1595f", "#9979c36a", "#99599ad3",
         "#99f9a65a", "#999e66ab", "#99cd7058", "#99d77fb3" };

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void SKElement_PaintSurfaceB(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
      {
         SKImageInfo info = e.Info;
         SKSurface surface = e.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         List<ChartEntry> entries = new List<ChartEntry>();

         //if (StockCalcViewModel.Current.DashDonatEmplacements.Count > 0)
         //{
         //   foreach (var i in StockCalcViewModel.Current.DashDonatEmplacements)
         //   {
         //      var entry = new ChartEntry((float)i.Value)
         //      {
         //         Label = i.Name,
         //         XValue = entries.Count,
         //         ValueLabel = i.Value.ToString(),
         //         Color = Color.Parse(colors[entries.Count % 7])
         //      };

         //      entries.Add(entry);
         //   };
         //};

         var chart = new Microcharts.PieChart()
         {
            Entries = entries,
            LabelTextSize = 24,
            IsAnimated = false,
            Radius = 0.8F,
         };

         chart.Draw(canvas, e.Info.Width, e.Info.Height);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void SKElement_PaintSurfaceC(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
      {
         SKImageInfo info = e.Info;
         SKSurface surface = e.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         List<ChartEntry> entries = new List<ChartEntry>();

         //if (StockCalcViewModel.Current.DashDonatTransactions.Count > 0)
         //{
         //   foreach (var i in StockCalcViewModel.Current.DashDonatTransactions)
         //   {
         //      var entry = new ChartEntry((float)i.Value)
         //      {
         //         Label = i.Name,
         //         XValue = entries.Count,
         //         ValueLabel = i.Value.ToString(),
         //         Color = Color.Parse(colors[entries.Count % 7])
         //      };

         //      entries.Add(entry);
         //   };
         //};

         var chart = new Microcharts.PieChart()
         {
            Entries = entries,
            LabelTextSize = 24,
            IsAnimated = false,
            Radius = 0.8F,
         };

         chart.Draw(canvas, e.Info.Width, e.Info.Height);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void SKElement_PaintSurfaceD(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
      {
         SKImageInfo info = e.Info;
         SKSurface surface = e.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         List<ChartEntry> entries = new List<ChartEntry>();

         //if (StockCalcViewModel.Current.DashDonatFamParVol.Count > 0)
         //{
         //   foreach (var i in StockCalcViewModel.Current.DashDonatFamParVol)
         //   {
         //      var entry = new ChartEntry((float)i.Value)
         //      {
         //         Label = i.Name,
         //         XValue = entries.Count,
         //         ValueLabel = i.Value.ToString(),
         //         Color = Color.Parse(colors[entries.Count % 7])
         //      };

         //      entries.Add(entry);
         //   };
         //};

         var chart = new Microcharts.PieChart()
         {
            Entries = entries,
            LabelTextSize = 24,
            IsAnimated = false,
            Radius = 0.8F,
         };

         chart.Draw(canvas, e.Info.Width, e.Info.Height);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void SKElement_PaintSurfaceE(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
      {
         SKImageInfo info = e.Info;
         SKSurface surface = e.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         List<ChartEntry> entries = new List<ChartEntry>();

         //if (StockCalcViewModel.Current.DashDonatArtParVol.Count > 0)
         //{
         //   foreach (var i in StockCalcViewModel.Current.DashDonatArtParVol)
         //   {
         //      var entry = new ChartEntry((float)i.Value)
         //      {
         //         Label = i.Name,
         //         XValue = entries.Count,
         //         ValueLabel = i.Value.ToString(),
         //         Color = Color.Parse(colors[entries.Count % 7]),
         //      };

         //      entry.StackedEntries.Add(entry.Copy());
         //      entry.StackedEntries.Add( 
         //         new ChartEntry( float.Parse(i.Tag.ToString()) - entry.Value )
         //         { Color= Color.Parse("8F00") });

         //      entry.CalcSum();

         //      entries.Add(entry);
         //   };
         //};

         var chart = new Microcharts.BarChart()
         {
            Entries = entries,
            LabelTextSize = 24,
            IsAnimated = false,
            Orientation = Microcharts.Orientation.Vertical,
            LabelOrientation = Microcharts.Orientation.Horizontal,
            ValueLabelOrientation = Microcharts.Orientation.Horizontal,
            LabelColor = SKColor.Parse("d333"),
            Stacked = true,
         };

         chart.Draw(canvas, e.Info.Width, e.Info.Height);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
