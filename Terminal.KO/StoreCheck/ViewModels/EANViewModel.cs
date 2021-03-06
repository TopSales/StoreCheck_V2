using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZPF;
using System.Text.Json;
using ZPF.AT;
using ZPF.SQL;
using System.Runtime.InteropServices.ComTypes;

public class EANViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static EANViewModel _Current = null;

   public static EANViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new EANViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public EANViewModel()
   {
      _Current = this;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<EAN_Article> ArticlesEAN { get; private set; } = new List<EAN_Article>();

   internal void SetLocalArticlesEAN(List<EAN_Article> list)
   {
      ArticlesEAN = list;

      OnPropertyChanged("ArticlesEAN");
   }

   internal void AddLocalArticlesEAN(List<EAN_Article> list)
   {
      ArticlesEAN.AddRange(list);

      OnPropertyChanged("ArticlesEAN");
   }

   static bool WasInit = false;
   internal async void SetArticlesEAN()
   {
      //var folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      //System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
      //var fileName = folder + @"\Data\Norma.db3";

      if (!WasInit)
      {
         WasInit = true;

         BackboneViewModel.Current.IncBusy();

         DoIt.OnBackground( async () => 
         {
            var fileName = await CopyFile("Norma.db3");

            if (System.IO.File.Exists(fileName))
            {
               var _DBSQLViewModel = new DBSQLViewModel(new MSSQLiteEngine());
               string connectionString = DB_SQL.GenConnectionString(DBType.SQLite, "", fileName, "", "");

               var result = _DBSQLViewModel.Open(connectionString, true);

               var list = DB_SQL.Query<EAN_Article>(_DBSQLViewModel, "select EAN, Brand, Label_FR, Condi, UCondi, Price from EAN_Article");

               if (string.IsNullOrEmpty(DB_SQL._ViewModel.LastError))
               {
                  ArticlesEAN = list;

                  OnPropertyChanged("ArticlesEAN");
               }
               else
               {
                  // DB_SQL._ViewModel.LastError == “SQLite Error 14: 'unable to open database file'.”
                  // https://github.com/xamarin/xamarin-android/issues/3819

                  Debug.WriteLine(DB_SQL._ViewModel.LastError);
                  Debugger.Break();
               };
            }
            else
            {
               Debugger.Break();
            };

            DoIt.OnMainThread(() =>
            {
               BackboneViewModel.Current.DecBusy();
            });
         });
      };
   }

   public async Task<string> CopyFile(string name)
   {
      var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      var finalPath = Path.Combine(basePath, name);

      if (File.Exists(finalPath))
      {
         File.Delete(finalPath);
      };

      var assembly = GetType().Assembly;
      var tmpName = $"{assembly.GetName().Name}.Data.{name}";

      using (var tempFileStream = assembly.GetManifestResourceStream(tmpName))
      {
         using (var fileStream = File.Open(finalPath, FileMode.CreateNew))
         {
            await tempFileStream.CopyToAsync(fileStream);
         };
      };

      return finalPath;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public EAN_Article CurrentArticleEAN { get => _CurrentArticleEAN; set => SetField(ref _CurrentArticleEAN, value); }
   EAN_Article _CurrentArticleEAN = null;

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
