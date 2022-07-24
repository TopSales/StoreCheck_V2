using System;
using System.Collections.Generic;
using System.Text;
using ZPF.SQL;

namespace ZPF
{
   public partial class SmarterASPViewModel : BaseViewModel
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      static SmarterASPViewModel _Instance = null;

      public static SmarterASPViewModel Current
      {
         get
         {
            if (_Instance == null)
            {
               _Instance = new SmarterASPViewModel();
            };

            return _Instance;
         }

         set
         {
            _Instance = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      public SmarterASPViewModel()
      {
         _Instance = this;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public DBSQLViewModel OpenNorma()
      {
         //string server = "sql6011.site4now.net";
         //string db = "db_a73afc_norma";
         //string user = "db_a73afc_norma_admin";
         //string password = "bg3TNp4RC9kGZZ";
         string server = "Diplodocus.dev";
         string db = "StoreCheck_Norma";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenNorma_ME()
      {
         string server = @"Server\SQLEXPRESS";
         string db = "TopSales_Norma";
         string user = "TopSales";
         string password = "Hughes&Demir";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenAventoLimoursSQLServer()
      {
         string server = "LocalHost";
         string db = "Avento";
         string user = "ImportUser";
         string password = "Nyt!HPM#pS%sP7";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }



      public DBSQLViewModel OpenAventoProdLimoursSQLServer()
      {
         string server = "LocalHost";
         string db = "AventoProd";
         string user = "ImportUser";
         string password = "Nyt!HPM#pS%sP7";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }



      public DBSQLViewModel OpenAventoTestLimoursSQLServer()
      {
         string server = "LocalHost";
         string db = "AventoTest";
         string user = "ImportUser";
         string password = "Nyt!HPM#pS%sP7";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }



      public DBSQLViewModel OpenAventoDumpLimoursSQLServer()
      {
         string server = "localhost";
         string db = "Avento";
         string user = "sa";
         string password = "Admin40!";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }




      public DBSQLViewModel OpenAventoChMSQLServer()
      {
         string server = "localhost";
         string db = "Avento";
         string user = "sa";
         string password = "Admin40!";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenAventoTestChMSQLServer()
      {
         string server = "localhost";
         string db = "AventoTest";
         string user = "sa";
         string password = "Admin40!";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenAventoProdChMSQLServer()
      {
         string server = "localhost";
         string db = "AventoProd";
         string user = "sa";
         string password = "Admin40!";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenAventoVPSSQLServer()
      {
         string server = "localhost";
         string db = "Avento";
         string user = "sa";
         string password = "v3E6xz5eHuL@bk";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenTopSales_AventoVPSSQLServer()
      {
         string server = "localhost";
         string db = "TopSales_Avento";
         string user = "sa";
         string password = "v3E6xz5eHuL@bk";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenAventoTestVPSSQLServer()
      {
         string server = "localhost";
         string db = "AventoTest";
         string user = "sa";
         string password = "v3E6xz5eHuL@bk";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenAventoProdVPSSQLServer()
      {
         string server = "localhost";
         string db = "AventoProd";
         string user = "sa";
         string password = "v3E6xz5eHuL@bk";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenMSSQLTarget()
      {

         string server = "sql6010.site4now.net";
         string db = "db_a44f11_uttarget";
         string user = "db_a44f11_uttarget_admin";
         string password = "4iMXJdjaH2HTa9";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenAventoDev()
      {

         string server = "SQL5080.site4now.net";
         string db = "db_a73608_avento";
         string user = "db_a73608_avento_admin";
         string password = "pNAnvrmXM6xV77";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenAventoProd()
      {/* pNAnvrmXM6xV77 / sql6011.site4now.net // db_a73afc_avento_admin*/

         string server = "SQL6011.site4now.net";
         string db = "db_a73afc_avento";
         string user = "db_a73afc_avento_admin";
         string password = "pNAnvrmXM6xV77";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

#region DB_Origin_TopSales // Bulgares + Avento

      public DBSQLViewModel OpenAventoPBI()
      {
         string server = "topsalesfmcg365.database.windows.net";
         string db = "PowerBIData";
         string user = "readonlysqlpowerbiuser";
         string password = "TS_Merlot.268";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      //public DBSQLViewModel OpenDBStrates_MetriShelf()
      //{
      //   string server = "5.180.164.109";
      //   string db = "metrishelf";
      //   string user = "usrro1";
      //   string password = "1rousr";
      //   string paramsString = "Port=3307";

      //   var _DBSQLViewModel = new DBSQLViewModel(new MySQLEngine());
      //   string connectionString = DB_SQL.GenConnectionString(DBType.MySQL, server, db, user, password, paramsString);

      //   var result = _DBSQLViewModel.Open(connectionString, true);

      //   DBViewModel.Current.Connection = _DBSQLViewModel;

      //   return _DBSQLViewModel;
      //}





      //public DBSQLViewModel OpenMetriShelf()
      //{
      //   string server = "5.180.164.109";
      //   string db = "metrishelf";
      //   string user = "usrro1";
      //   string password = "1rousr";

      //   var _DBSQLViewModel = new DBSQLViewModel(new MySQLEngine());
      //   string connectionString = DB_SQL.GenConnectionString(DBType.MySQL, server, db, user, password);//Port = 3306

      //   var result = _DBSQLViewModel.Open(connectionString, true);

      //   DBViewModel.Current.Connection = _DBSQLViewModel;

      //   return _DBSQLViewModel;
      //}


      #endregion

      //public DBSQLViewModel OpenSXC()
      //{
      //   string server = "mysql6003.site4now.net";
      //   string db = "db_a44f11_sxc";
      //   string user = "a44f11_sxc";
      //   string password = "Re4SMbAgvjozH3";

      //   var _DBSQLViewModel = new DBSQLViewModel(new MySQLEngine());
      //   string connectionString = DB_SQL.GenConnectionString(DBType.MySQL, server, db, user, password);//Port = 3306

      //   var result = _DBSQLViewModel.Open(connectionString, true);

      //   DBViewModel.Current.Connection = _DBSQLViewModel;

      //   return _DBSQLViewModel;
      //}


      public DBSQLViewModel OpenStockAPPro()
      {
         string server = "SQL6005.site4now.net";
         string db = "DB_A44F11_StockAPPro2dev";
         string user = "DB_A44F11_StockAPPro2dev_admin";
         string password = "MossIsTheBoss19";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenStoreCheckDev()
      {
         string server = "Diplodocus.dev";
         string db = "Storecheck_DBDev";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";


         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenStoreCheckProd()
      {
         string server = "Diplodocus.dev";
         string db = "Storecheck_DBProd";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";


         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenStoreCheckOld()
      {
         string server = "sql5105.site4now.net";
         string db = "db_a73608_storecheckold";
         string user = "db_a73608_storecheckold_admin";
         string password = "SkixeYSxtTX55Y";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      //

      public DBSQLViewModel OpenStoreCheckMaquette()
      {
         string server = "Diplodocus.dev";
         string db = "DB_Maquette";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";
         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenStoreCheckMaquetteChM()
      {
         string server = "localhost";
         string db = "DB_Maquette";
         string user = "sa";
         string password = "Admin40!";
         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenAuditTrailDev()
      {
         string server = "Diplodocus.dev";
         string db = "Storecheck_ATDev";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenAuditTrailProd()
      {
         string server = "Diplodocus.dev";
         string db = "Storecheck_ATDev";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }


      public DBSQLViewModel OpenBaseDocDev()
      {
         string server = "Diplodocus.dev";
         string db = "Storecheck_DocDev";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenBaseDocProd()
      {
         string server = "Diplodocus.dev";
         string db = "Storecheck_DocProd";
         string user = "StoreCheck_Admin";
         string password = "7u$x1o8V";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      //public DBSQLViewModel OpenMySQL()
      //{
      //   string server = "mysql-zpf.alwaysdata.net";
      //   string db = "zpf_mysql";
      //   string user = "zpf_wanao";
      //   string password = "wanao_zpf";

      //   var _DBSQLViewModel = new DBSQLViewModel(new MySQLEngine());
      //   string connectionString = DB_SQL.GenConnectionString(DBType.MySQL, server, db, user, password);

      //   var result = _DBSQLViewModel.Open(connectionString, true);

      //   DBViewModel.Current.Connection = _DBSQLViewModel;

      //   return _DBSQLViewModel;
      //}

      //public DBSQLViewModel OpenPostgreSQL()
      //{
      //   string server = "185.31.40.65";
      //   string db = "chm40_test";
      //   string user = "chm40";
      //   string password = "AlwaysTof40!";

      //   var _DBSQLViewModel = new DBSQLViewModel(new PostgreSQLEngine());
      //   string connectionString = DB_SQL.GenConnectionString(DBType.PostgreSQL, server, db, user, password);

      //   var result = _DBSQLViewModel.Open(connectionString, true);

      //   DBViewModel.Current.Connection = _DBSQLViewModel;

      //   return _DBSQLViewModel;
      //}

      public DBSQLViewModel OpenMSSQL()
      {
         string server = "SQL6005.site4now.net";
         string db = "DB_A44F11_Test";
         string user = "DB_A44F11_Test_admin";
         string password = "MossIsTheBoss19";

         var _DBSQLViewModel = new DBSQLViewModel(new SQLServerEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLServer, server, db, user, password);

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenMSSQLiteTMP()
      {
         string dbFileName = System.IO.Path.GetTempFileName() + @".db3";

         var _DBSQLViewModel = new DBSQLViewModel(new MSSQLiteEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLite, "", dbFileName, "", "");

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         DB_SQL.QuickQuery(_DBSQLViewModel, "PRAGMA journal_mode=WAL;");

         return _DBSQLViewModel;
      }

      public DBSQLViewModel OpenMSSQLite(string FileName = "")
      {
         string dbFileName = @"..\..\..\SQLite\" + (string.IsNullOrEmpty(FileName) ? "Output.db3" : FileName);
         dbFileName = System.IO.Path.GetFullPath(dbFileName);

         var _DBSQLViewModel = new DBSQLViewModel(new MSSQLiteEngine());
         string connectionString = DB_SQL.GenConnectionString(DBType.SQLite, "", dbFileName, "", "");

         var result = _DBSQLViewModel.Open(connectionString, true);

         DBViewModel.Current.Connection = _DBSQLViewModel;

         DB_SQL.QuickQuery(_DBSQLViewModel, "PRAGMA journal_mode=WAL;");
         return _DBSQLViewModel;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
