//using System;
//using System.Windows.Data;
//using ZPF;
//using ZPF.SQL;

//namespace StoreCheck.Pages
//{
//   public class ImageSourceConverter : IValueConverter
//   {
//      #region IValueConverter implementation

//      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//      {
//         if (MainViewModel.Current.IsMultiDB)
//         {
//            var FileExt = DB_SQL.QuickQuery($"select CAST(Document.PK AS nvarchar(15)) + Document.FileExt from Document where Document.FKDocumentType=3 and Document.FK={ItemsViewModel.Current.SelectedArticle.PK}");

//            if (!string.IsNullOrEmpty(FileExt))
//            {
//               string FilePath = System.IO.Path.GetTempPath() + $"ART_IMG{FileExt}";

//               if (System.IO.File.Exists(FilePath))
//               {
//                  return FilePath;
//               }
//               else
//               {
//                  var PK = DB_SQL.QuickQueryInt64($"select Document.PK from Document where Document.FKDocumentType=3 and Document.FK={ItemsViewModel.Current.SelectedArticle.PK}");
//                  if (BaseDocViewModel.Current.DownloadDoc(FilePath, Document.InternalDocumentTypes.image, PK))
//                  {
//                     return FilePath;
//                  };
//               };
//            };

//            return "";
//         };

//         return value;
//      }

//      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
//      {
//         if (MainViewModel.Current.IsMultiDB)
//         {
//            return null;
//         };

//         return value;
//      }

//      #endregion
//   }
//}
