using Microcharts;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace ZPF
{
   class ChartView : SkiaSharp.Views.WPF.SKElement
   {
      private Chart _chart;
      private InvalidatedWeakEventHandler<ChartView> _handler;

      public ChartView()
      {
         this.PaintSurface += ChartView_PaintSurface;
         //this.OnPaintSurface += OnPaintCanvas;
      }

      public Chart Chart
      {
         get => _chart;
         set
         {
            if (_chart != value)
            {
               if (_chart != null)
               {
                  _handler.Dispose();
                  _handler = null;
               }

               _chart = value;

               //Invalidate();
               this.InvalidateVisual();

               if (_chart != null)
               {
                  //_handler = _chart.ObserveInvalidate(this, (view) => view.Invalidate());
                  _handler = _chart.ObserveInvalidate(this, (view) => view.InvalidateVisual());
               }
            }
         }
      }

      //private void OnPaintCanvas(SKSurface sender, SKImageInfo e)
      //private void ChartView_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
      private void ChartView_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
      {
         if (_chart != null)
         {
            //_chart.Draw(sender.Canvas, e.Width, e.Height);
            _chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
         }
         else
         {
            //sender.Canvas.Clear(SKColors.Transparent);
            e.Surface.Canvas.Clear(SKColors.Transparent);
         }
      }
   }
}
