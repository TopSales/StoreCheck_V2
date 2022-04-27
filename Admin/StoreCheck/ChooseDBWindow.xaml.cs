using System.Diagnostics;
using System.Windows;
using ZPF;
using ZPF.SQL;
using ZPF.WPF;

namespace StoreCheck
{
   /// <summary>
   /// Interaction logic for ChooseDBWindow.xaml
   /// </summary>
   public partial class ChooseDBWindow : Window
   {
      public ChooseDBWindow()
      {
         InitializeComponent();

         // - - -  - - - 

         DBViewModel.Current.PropertyChanged += Instance_PropertyChanged;

         // - - -  - - - 

         OpenDBPage.DBListFileName = MainViewModel.Current.DataPath + MainViewModel.AppTitle + ".DBList.json";
         OpenDBPage.DBTypes = new DBType[] { DBType.SQLite, DBType.SQLServer };

         frameBody.Navigate(new OpenDBPage());
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "IsConnected" && DBViewModel.Current.IsConnected )
         {
            DBViewModel.Current.SaveList(MainViewModel.Current.DataPath + MainViewModel.AppTitle + ".DBList.json");

            this.Hide();

            new SplashWindow().ShowDialog();

            this.Close();
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
