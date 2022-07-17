using System.Data;
using System.Diagnostics;
using StoreCheck.Pages;
using ZPF.SQL;

namespace StoreCheck;

public partial class MainPage : ContentPage
{
   public MainPage()
   {
      Title = "";
      NavigationPage.SetHasNavigationBar(this, false);

      InitializeComponent();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   protected async override void OnAppearing()
   {
      base.OnAppearing();

      try
      {
         MainViewModel.Current.Load();
         MainViewModel.Current.LoadLocalDB();

         // Microsoft.Maui
         var strType = typeof(Microsoft.Maui.Controls.Application);
         var assemblyUri = strType.Assembly.Location;
         var versionInfo = FileVersionInfo.GetVersionInfo(new Uri(assemblyUri).LocalPath);

         await DisplayAlert(versionInfo.ProductName, versionInfo.FileVersion, "ok");


         // .Net Core
         //var strType = typeof(System.String);
         //var assemblyUri = strType.Assembly.CodeBase;
         //var versionInfo = FileVersionInfo.GetVersionInfo(new Uri(assemblyUri).LocalPath);

         //await DisplayAlert("Version", versionInfo.ToString(), "ok");

         //6.0.6  await DisplayAlert("Version", Environment.Version.ToString(), "ok");
      }
      catch { };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   private async void btnStart_Clicked(object sender, EventArgs e)
   {
      if (MainViewModel.Current.Config.FKUser > 0)
      {
         await Navigation.PushModalAsync(new StoreListPage());
      }
      else
      {
         await Navigation.PushModalAsync(new EntryPage());
      };
   }

   private void btnStop_Clicked(object sender, EventArgs e)
   {

   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   private async void btnContact_Clicked(object sender, EventArgs e)
   {
      await Navigation.PushModalAsync(new TopSalesPage());

   }

   private async void btnEAN_Clicked(object sender, EventArgs e)
   {
      await Navigation.PushModalAsync(new EANPage());
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   private async void btnTest_Clicked(object sender, EventArgs e)
   {
      var dbPath = Path.Combine(FileSystem.AppDataDirectory, "demo.db3");

      var DBSQLViewModel = new DBSQLViewModel(new MSSQLiteEngine());
      var connectionString = DB_SQL.GenConnectionString(DBType.SQLite, "", dbPath, "", "");

      DBSQLViewModel.Open(connectionString, true);


      {
         string SQL =
         @"
DROP TABLE IF EXISTS contacts;

CREATE TABLE contacts (
	contact_id INTEGER PRIMARY KEY,
	first_name TEXT NOT NULL,
	last_name TEXT NOT NULL
);

INSERT INTO contacts (last_name, first_name ) VALUES( 'Gates', 'Bill');
INSERT INTO contacts (last_name, first_name ) VALUES( 'Allen', 'Paul');
INSERT INTO contacts (last_name, first_name ) VALUES( 'Ballmer', 'Steve');
";

         DB_SQL.QuickQuery(SQL);
      };

      {
         string SQL = @" select * from contacts ";

         DataTable dt = DB_SQL.QuickQueryView(SQL) as DataTable;

         await DisplayAlert("", $"Hello {dt.Rows[0].ItemArray[1].ToString()}!", "ok");
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}

