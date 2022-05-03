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

   internal void SetArticlesEAN()
   {
      var folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 

      var _DBSQLViewModel = new DBSQLViewModel(new MSSQLiteEngine());
      string connectionString = DB_SQL.GenConnectionString(DBType.SQLite, "", folder + @"\Data\Norma.db3", "", "");

      var result = _DBSQLViewModel.Open(connectionString, true);

      var list = DB_SQL.Query<EAN_Article>(_DBSQLViewModel, "select EAN, Brand, Label_FR, Condi, UCondi, Price from EAN_Article");

      ArticlesEAN = list;

      OnPropertyChanged("ArticlesEAN");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public EAN_Article CurrentArticleEAN { get => _CurrentArticleEAN; set => SetField(ref _CurrentArticleEAN, value); }
   EAN_Article _CurrentArticleEAN = null;

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
