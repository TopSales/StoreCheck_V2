//using System;
//using OxyPlot;
//using OxyPlot.Series;
//using System.Windows.Data;
//using System.Collections.Generic;

//namespace ZPF
//{
//   public class DonutConverter : IValueConverter
//   {
//      public DonutConverter()
//      {
//         //ValueColor = Color.FromHex("#009DD9");
//         //BaseColor = Color.FromHex("#d3d3d3");
//      }

//      //public Color ValueColor { get; set; }
//      //public Color BaseColor { get; set; }

//      #region IValueConverter implementation

//      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//      {
//         var x = (List<NameValue_Double>)(object)(value);

//         // if (!(value is double)) return null;

//         var plotModel = new PlotModel();

//         var series = new PieSeries
//         {
//            //Title = "Series 1",
//            InnerDiameter = 0.25,
//            Diameter = 0.80,

//            StrokeThickness = 2.0,
//            InsideLabelPosition = 0.50,
//            AngleSpan = 360,
//            StartAngle = 20,
//            FontWeight = FontWeights.Bold,
//            FontSize = 16,
//            //TextColor = OxyColors.White,
//         };


//         foreach( var nv in x )
//         {
//            series.Slices.Add(new PieSlice( nv.Name, nv.Value) { IsExploded = true });
//         }

//         plotModel.Series.Add(series);

//         return plotModel;
//      }

//      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//      {
//         throw new NotImplementedException();
//      }

//      #endregion
//   }
//}