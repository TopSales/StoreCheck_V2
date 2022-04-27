using StoreCheck.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZPF;
using ZPF.AT;
using ZPF.SQL;
using ZPF.WPF;
using ZPF.XLS;
using static BackboneViewModel;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();

         menuGridView.DataContext = MainViewModel.Current;
         menuGridView.SetModules(MainViewModel.Current.GetModules());

         // I18nViewModel.Current.T(this);

#if DEBUG
         // I18nViewModel.Current.FindMissing = true;

         //tileFactures.Visibility = Visibility.Visible;
#else
         //tileFactures.Visibility = Visibility.Collapsed;
#endif

         WPFMessageBox.Dico = WPFMessageBox.DicoFR;

         BackboneViewModel.Current.InitMsgCallBack(MsgCallBack);
         BackboneViewModel.Current.InitDoEventsCallBack(DoEventsCallBack);

         try
         {
            MenuViewModel.Instance.MenuCallBack += NavigateMenu;
         }
         catch (Exception ex)
         {
            BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Info,
               ex.Message + Environment.NewLine + ex.StackTrace);
         };

         Log.WriteTimeStamp("Before Nav");

         NavigateMenu("HOME");
         // NavigateMenu("STYLE");

         columnDefinition.Width = new GridLength(MainViewModel.Current.LoadSplitter("Main", "Splitter", 150));

         Log.WriteTotalTime("End Splash");
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      bool MsgCallBack(MessageBoxType type, string Text, string Caption = "")
      {
         bool Result = true;

         DoIt.OnMainThread(() =>
         {
            Result = WPFMessageBox.Show(this, type, Text, Caption);
         });

         return Result;
      }


      void DoEventsCallBack()
      {
         try
         {
            if (System.Windows.Application.Current != null)
            {
               System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new System.Threading.ThreadStart(delegate { }));
            };
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Tile_Click(object sender, RoutedEventArgs e)
      {
         if (sender is MahApps.Metro.Controls.Tile)
         {
            string cmd = (sender as MahApps.Metro.Controls.Tile).CommandParameter.ToString();
            NavigateMenu(cmd);
         };

         if (sender is ZPF.WPF.Compos.MenuGridView)
         {
            if (e.Source is ZPF.WPF.Compos.TileEx)
            {
               string cmd = (e.Source as ZPF.WPF.Compos.TileEx).CommandParameter.ToString();
               NavigateMenu(cmd);
            };

            if (e.Source is ZPF.WPF.Compos.SkiaTile)
            {
               string cmd = (e.Source as ZPF.WPF.Compos.SkiaTile).CommandParameter.ToString();
               NavigateMenu(cmd);
            };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public TStrings PrevPages { get => _PrevPages; set => _PrevPages = value; }
      TStrings _PrevPages = new TStrings();


      private void NavigateMenu(string currentRef)
      {
         //Log.Write("MENU", (string.IsNullOrEmpty(MenuViewModel.Instance.Current.Title) ? "" : MenuViewModel.Instance.Current.Title));
         UserViewModel.Current.UpdateUserSession(currentRef);

         if (currentRef == "BACK")
         {
            if (PrevPages.Count > 1)
            {
               PrevPages.Pop(); // Current page

               string title = PrevPages[PrevPages.Count - 1];
               //tileBack.Visibility = (PrevPages.Count > 1 ? Visibility.Visible : Visibility.Collapsed);

               NavigatePage(title, (Page)PrevPages.PopObject());

               return;
            }
            else
            {
               PrevPages.Clear();
               currentRef = "HOME";
            };
         }
         else
         {
            if (currentRef == "HOME" || currentRef == "STYLE" || currentRef == "DB")
            {
               PrevPages.Clear();
            };
         };

         switch (currentRef)
         {
            case "DB":
               MainViewModel.Current.Title = "Choix de la base de donnée";

               OpenDBPage.DBListFileName = MainViewModel.Current.DataPath + MainViewModel.AppTitle + ".DBList.json";
               OpenDBPage.DBTypes = new DBType[] { DBType.SQLite, DBType.SQLServer };

               frameBody.Navigate(new OpenDBPage());
               break;

            case "STYLE":
               string fmt = Environment.CurrentDirectory + @"\Styles\{0}.xaml";

               TStrings FileNames = new TStrings();
               FileNames.Add(string.Format(fmt, MainViewModel.Current.Style));

               //(Application.Current as App).DynamicLoadStyles(FileNames);
               MainViewModel.Current.Title = "";
               frameBody.Navigate(new DashBoardPage());
               break;

            case "HOME":
               MainViewModel.Current.Title = "Tableau de bord";
               frameBody.Navigate(new DashBoardPage());
               break;

            // - - -  - - - 

            case "ABOUT":
               NavigatePage("Outils / à propos", new AboutPage());
               break;

            case "IMP_EXP":
               NavigatePage("Import / export", new ImportPage());
               break;

            case "Factures":
               MainViewModel.Current.Title = "Facturation";
               //ZPF.Facturation.Facturation.Facture_Clicked(@"D:\OneDrive\ZPF\Compta\Facturation\");
               // frameBody.Navigate(new AnalyticsPage());
               break;

            case "Analytics":
               NavigatePage("Analytics", new AnalyticsPage());
               break;

            case "TOOLS":
               NavigatePage("Outils", new ToolsPage());
               break;

            case "DATA":
               if (UserViewModel.Current.CheckRights("Referentiel.View"))
               {
                  MainViewModel.Current.LoadEditReferentiel(); // Refresh data

                  NavigatePage("Outils / référentiel", new ReferentielPage());
               };
               break;

            case "USER":
               UserViewModel.Current.Init(DBViewModel.Current.Connection);
               UserAdminViewModel.Current.LoadData(true);

               if (UserViewModel.Current.CheckRights("User.View"))
               {
                  MainViewModel.Current.Title = "Outils / utilisateurs";

                  var p = new UserManagementPage();
                  p.OnPrintBadge += ((object sender, RoutedEventArgs e) =>
                  {
                     if (UserViewModel.Current.CurrentUser != null)
                     {
                        TStrings fields = new TStrings();
                        fields.Add("Title", UserViewModel.Current.CurrentUser.Login);
                        fields.Add("Barcode", UserViewModel.Current.CurrentUser.Ref);

                        PrintLabelHelper.PrintOld(fields);
                     };
                  });

                  frameBody.Navigate(p);
               };
               break;

            case "CONFIG":
               if (UserViewModel.Current.CheckRights("Config.View"))
               {
                  MainViewModel.Current.LoadEditReferentiel(); // Refresh data

                  NavigatePage("Outils / paramètres", new SettingsPage());
               };
               break;

            case "DBASE":
               NavigatePage("Outils / database", new DatabasePage());
               break;

            case "WINCE":
               NavigatePage("Gestion des terminaux", new WinCE_Page());
               break;

            case "AUDITTRAIL":
               NavigatePage("Traces", new AuditTrailPage(AuditTrailViewModel.Current, MainViewModel.IniFileName));
               break;

            case "BASEDOC":
               string Ref = "3615";
               NavigatePage("Basedoc", new BaseDoc.BaseDocMain(false, Ref));
               break;

            default: BackboneViewModel.Current.MessageBox(MessageBoxType.Error, "Oups ..."); break;
         };

         tileBack.Visibility = (PrevPages.Count > 0 ? Visibility.Visible : Visibility.Collapsed);
         tbTitle.Text = MainViewModel.Current.Title;
         BackboneViewModel.Current.CurrentRef = currentRef;
      }

      private void NavigatePage(string Title, Page page)
      {
         PrevPages.PushObject(Title, page);

         NavigateOnly(Title, page);
      }

      private void NavigateOnly(string Title, Page page)
      {
         MainViewModel.Current.Title = Title;
         tbTitle.Text = MainViewModel.Current.Title;

         frameBody.Navigate(page);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         Basics.ReadFormPos(MainViewModel.IniFileName, this, false, false);
         Application.Current.MainWindow = this;

         LoginWindow dlg = null;

         if (!MainViewModel.Current.CheckDB())
         {
            BackboneViewModel.Current.MessageBox(MessageBoxType.Error, "Version de base de données incompatible!");

            if (!Debugger.IsAttached)
            {
               this.Close();
               App.Current.Dispatcher.InvokeShutdown();
               return;
            };
         };

         this.Title = this.Title + " - " + DBViewModel.Current.Name;

         if (MainViewModel.Current.ServerAutoStart)
         {
            //ToDo: Chat: MainViewModel.Current.StartWebServer();
         };

         {
#if DEBUG
            dlg = new LoginWindow(true);
#else
            dlg = new LoginWindow(false);
#endif

            dlg.Owner = this;
            bool Result = dlg.ShowDialog() == true;

            if (!Result)
            {
               this.Close();
               App.Current.Dispatcher.InvokeShutdown();
            };

            BackboneViewModel.Current.IsBusy = false;
         };


         MainViewModel.Current.TestUnitaires = false;

         //(frameBody.Content as DashBoardPage).Animation();
         // VMLocator.Main.UpdateRightsMenu();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
      {
         Basics.WriteFormPos(MainViewModel.IniFileName, this, false);

         // if (I18nViewModel.Current.FindMissing)
         {
            // I18nViewModel.Current.SaveMissing();
         };

         // - - -  - - - 

         try
         {
            //stop server
            //ToDo: Chat: 
            //if (MainViewModel.Current.webServer != null)
            //{
            //   MainViewModel.Current.webServer.Stop();
            //};
         }
         catch { };

         System.Threading.Thread.Sleep(1000);

         // - - -  - - - 

         if (DB_SQL._ViewModel.CurrentTransaction != null)
         {
            try
            {
               DB_SQL.Commit(DB_SQL._ViewModel.CurrentTransaction);
               Log.Write(ErrorLevel.Critical, "Pending transaction on exit !!!");
            }
            catch { };
         };

         DB_SQL._ViewModel.Close();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -    

      private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
      {
         if (columnDefinition.Width.Value > 150) columnDefinition.Width = new GridLength(150);
         if (columnDefinition.Width.Value < 150) columnDefinition.Width = new GridLength(90);

         MainViewModel.Current.SaveSplitter("Main", "Splitter", columnDefinition.Width.Value);
         menuGridView.Redraw();
      }

      private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
      {
#if DEBUG
         if (Debugger.IsAttached)
         {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
            };

            if (Keyboard.IsKeyDown(Key.LeftAlt))
            {
            };
         };
#endif
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
