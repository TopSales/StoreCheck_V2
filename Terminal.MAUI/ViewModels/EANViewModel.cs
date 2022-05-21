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
using ZPF;
using System.Text.Json;
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
   internal void SetArticlesEAN()
   {

      //var folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      //System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
      //var fileName = folder + @"\Data\Norma.db3";

      if (!WasInit)
      {
         WasInit = true;

         BackboneViewModel.Current.IncBusy();

         DoIt.OnBackground(async () =>
         {
            var fileName = await CopyFile("norma.db3");

            if (System.IO.File.Exists(fileName))
            {
               var _DBSQLViewModel = new DBSQLViewModel(new MSSQLiteEngine());
               string connectionString = DB_SQL.GenConnectionString(DBType.SQLite, "", fileName, "", "");

               try
               {
                  _DBSQLViewModel.Open(connectionString, true);
               }
               catch (Exception ex)
               {
                  Debug.WriteLine(ex.Message);
               };

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
      string basePath = "";

      basePath = FileSystem.AppDataDirectory;
      var finalPath = Path.Combine(basePath, name);

      if (File.Exists(finalPath))
      {
         return finalPath;
         //File.Delete(finalPath);
      };

      using var stream = await FileSystem.OpenAppPackageFileAsync(name);
      //using var reader = new StreamReader(stream);
      //var contents = await reader.ReadToEndAsync();
      //monkeyList = JsonSerializer.Deserialize<List<Monkey>>(contents);

      //var assembly = GetType().Assembly;
      //var tmpName = $"{assembly.GetName().Name}.Data.{name}";

      using (var tempFileStream = await FileSystem.OpenAppPackageFileAsync(name))
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

   #region - - - fixed size tools - - -

   private string _fsRecord;

   public EAN_Article GetRecord(string fileName, string keyToBeFound)
   {
      EAN_Article Result = new EAN_Article();

      long position = SearchFile(fileName, keyToBeFound);

      if (position > 0)
      {
         int ind = 0;

         Result.EAN = _fsRecord.Substring(ind, 13);
         ind += 13;

         Result.Brand = _fsRecord.Substring(ind, 32).TrimEnd();
         ind += 32;

         Result.Label_FR = _fsRecord.Substring(ind, 64).TrimEnd();
         ind += 64;

         Result.Condi = _fsRecord.Substring(ind, 8).TrimEnd();
         ind += 8;

         Result.UCondi = _fsRecord.Substring(ind, 8).TrimEnd();
         ind += 8;

         Result.Price = decimal.Parse(_fsRecord.Substring(ind, 8).TrimEnd());
         ind += 8;
      };

      return Result;
   }

   private long SearchFile(string fileName, string keyToBeFound, int keyPosition = 0, int keySize = 13, int recordSize = 133)
   {
      long left, right, middle;
      Byte[] recherche = new Byte[recordSize];
      long nb_Records;

      try
      {
         System.IO.FileInfo fi = new FileInfo(fileName);

         using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
         {
            long fileSize = fi.Length;
            nb_Records = fileSize / recordSize;
            if (nb_Records == 0)
            {
               return 0;
            };

            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Read(recherche, 0, recordSize);

            if (keyToBeFound.CompareTo(Encoding.ASCII.GetString(recherche, 0, keySize)) == -1)
            {
               fileStream.Close();
               return -1;
            };

            fileStream.Seek(fileSize - recordSize, SeekOrigin.Begin);
            fileStream.Read(recherche, 0, recordSize);

            int comparaison = keyToBeFound.CompareTo(Encoding.ASCII.GetString(recherche, 0, keySize));

            if (comparaison == 1)
            {
               fileStream.Close();
               return -(nb_Records + 1);
            }
            else if (comparaison == 0)
            {
               _fsRecord = Encoding.ASCII.GetString(recherche, 0, recordSize);

               fileStream.Close();
               return (nb_Records);
            };

            left = 0;
            right = nb_Records - 1;

            while (left + 1 < right)
            {
               middle = (left + right) / 2;
               fileStream.Seek(middle * recordSize, SeekOrigin.Begin);
               fileStream.Read(recherche, 0, recordSize);

               if (keyToBeFound.CompareTo(Encoding.ASCII.GetString(recherche, 0, keySize)) == -1)
                  right = middle;
               else
                  left = middle;
            }

            fileStream.Seek(left * recordSize, SeekOrigin.Begin);
            fileStream.Read(recherche, 0, recordSize);
            fileStream.Close();

            string Enregistrement = Encoding.ASCII.GetString(recherche, 0, recordSize);

            if (keyToBeFound.CompareTo(Enregistrement.Substring(0, keySize)) == 0)
            {
               _fsRecord = Enregistrement;
               return (left + 1);
            }
            else
            {
               return -(left + 2);
            };
         }
      }
      catch
      {
         return 0;
      }
   }

   #endregion

}
