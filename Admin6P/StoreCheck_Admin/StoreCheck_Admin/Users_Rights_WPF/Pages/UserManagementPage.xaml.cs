using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZPF.SQL;

namespace ZPF
{
   /// <summary>
   /// Interaction logic for UserManagementPage.xaml
   /// </summary>
   public partial class UserManagementPage : Page
   {
      public Action<object, RoutedEventArgs> OnPrintBadge { get; set; }

      public UserManagementPage()
      {
         DataContext = UserAdminViewModel.Current;

         InitializeComponent();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static Stream GetRessourceStream(string Name)
      {
         Assembly assembly = Assembly.GetExecutingAssembly();

         try
         {
            return assembly.GetManifestResourceStream(assembly.GetName().Name.ToString() + "." + Name);
         }
         catch
         {
            if (Debugger.IsAttached)
            {
               // Missing ressource ???
               Debugger.Break();
            };

            return null;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void DataGridUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         DataGrid dg = (sender as DataGrid);

         if (dg.SelectedItem == null)
         {
            return;
         };

         UserViewModel.Current.Connection.NewTransaction(UserViewModel.Current.Connection);
         UserAdminViewModel.Current.SelectedUser = dg.SelectedItem as UserAccount;
      }

      private void btnUser_New_Click(object sender, RoutedEventArgs e)
      {
         UserViewModel.Current.Connection.RollbackTransaction();

         UserAdminViewModel.Current.SelectedUser = new UserAccount();
      }

      private void btnUser_Copy_Click(object sender, RoutedEventArgs e)
      {
         UserAccount u = UserAdminViewModel.Current.SelectedUser.Copy();

         u.PK = 0;
         u.LastFailure = DateTime.MinValue;
         u.LastLogin = DateTime.MinValue;
         u.LoginFailures = 0;

         UserAdminViewModel.Current.SelectedUser = u;
      }

      private void btnUser_Save_Click(object sender, RoutedEventArgs e)
      {
         if (UserAdminViewModel.Current.SelectedUser.PK == -1)
         {
            UserAdminViewModel.Current.SelectedUser.Password = UserViewModel.Current.Salt(UserAdminViewModel.Current.SelectedUser.Login, UserAdminViewModel.Current.SelectedUser.Password);
         }
         else
         {
            var u = DB_SQL.QueryFirst<UserAccount>($"Select * from UserAccount where PK={UserAdminViewModel.Current.SelectedUser.PK}");

            if (u.Password != UserAdminViewModel.Current.SelectedUser.Password)
            {
               UserAdminViewModel.Current.SelectedUser.Password = UserViewModel.Current.Salt(UserAdminViewModel.Current.SelectedUser.Login, UserAdminViewModel.Current.SelectedUser.Password);
            };
         };

         UserViewModel.Current.Connection.SaveRecord(UserViewModel.Current.Connection, "UserAccount", UserAdminViewModel.Current.SelectedUser);
         UserViewModel.Current.Connection.CommitTransaction();

         UserAdminViewModel.Current.LoadData(true);
      }

      private void btnUser_Cancel_Click(object sender, RoutedEventArgs e)
      {
         UserAdminViewModel.Current.LoadData(true);
      }

      private void btnUser_Delete_Click(object sender, RoutedEventArgs e)
      {
         if (MessageBox.Show("Etes-vous sur de vouloir supprimer cet élément?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.OK)
         {
            UserViewModel.Current.Connection.DeleteRecord(UserViewModel.Current.Connection, "UserAccount", UserAdminViewModel.Current.SelectedUser);
            UserViewModel.Current.Connection.CommitTransaction();

            UserAdminViewModel.Current.LoadData(true);
         }
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void DataGridProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         DataGrid dg = (sender as DataGrid);

         if (dg.SelectedItem == null)
         {
            return;
         };

         UserViewModel.Current.Connection.NewTransaction(UserViewModel.Current.Connection);

         UserAdminViewModel.Current.SelectedProfile = dg.SelectedItem as User_Profile;
      }

      private void btnProfile_New_Click(object sender, RoutedEventArgs e)
      {
         UserViewModel.Current.Connection.RollbackTransaction();

         UserAdminViewModel.Current.SelectedProfile = new User_Profile();
      }

      private void btnProfile_Copy_Click(object sender, RoutedEventArgs e)
      {
         User_Profile p = UserAdminViewModel.Current.SelectedProfile.Copy();

         p.PK = 0;

         UserAdminViewModel.Current.SelectedProfile = p;
      }

      private void btnProfile_Save_Click(object sender, RoutedEventArgs e)
      {
         if (UserAdminViewModel.Current.SelectedProfile == null)
         {
            MessageBox.Show("Vous devez d'abord sélectionner ou créer un profile.", "Oups ...", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
            return;
         };

         UserViewModel.Current.Connection.SaveRecord(UserViewModel.Current.Connection, "User_Profile", UserAdminViewModel.Current.SelectedProfile);
         UserViewModel.Current.Connection.CommitTransaction();

         if (UserAdminViewModel.Current.SelectedProfile.PK == 0)
         {
            UserAdminViewModel.Current.SelectedProfile = DB_SQL.QueryFirst<User_Profile>(UserViewModel.Current.Connection, $"select * from User_Profile where Label = '{UserAdminViewModel.Current.SelectedProfile.Label}'");
         };

         UserAdminViewModel.Current.LoadData(true);
      }

      private void btnProfile_Cancel_Click(object sender, RoutedEventArgs e)
      {
         UserAdminViewModel.Current.LoadData(true);
      }

      private void btnProfile_Delete_Click(object sender, RoutedEventArgs e)
      {
         if (MessageBox.Show("Are you sure you want to delete this item?", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) == MessageBoxResult.OK)
         {
            UserViewModel.Current.Connection.DeleteRecord(UserViewModel.Current.Connection, "User_Profile", UserAdminViewModel.Current.SelectedProfile);
            UserViewModel.Current.Connection.CommitTransaction();

            UserAdminViewModel.Current.LoadData(true);
         }
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void btnProfileAddRole_Click(object sender, RoutedEventArgs e)
      {
         if (UserAdminViewModel.Current.SelectedProfile.PK == 0)
         {
            MessageBox.Show("You should save the profile first.", "Oups ...", MessageBoxButton.OK, MessageBoxImage.Stop);
            return;
         };

         if (dgProfile_AllRoles.SelectedItem != null)
         {
            User_Role r = dgProfile_AllRoles.SelectedItem as User_Role;

            if (DB_SQL.QuickQuery(UserViewModel.Current.Connection, string.Format("insert into User_ProfileRole ( FKProfile, FKRole ) values ({0}, {1})", UserAdminViewModel.Current.SelectedProfile.PK, r.PK)) != "1")
            {
               DB_SQL.MessageBoxSQL();
            };

            UserAdminViewModel.Current.LoadProfileRoles();
            UserAdminViewModel.Current.LoadUserRoles();
         };
      }

      private void btnProfileDelRole_Click(object sender, RoutedEventArgs e)
      {
         if (dgProfile_GrantedRoles.SelectedItem != null)
         {
            User_ProfileRole r = dgProfile_GrantedRoles.SelectedItem as User_ProfileRole;

            DB_SQL.QuickQuery(UserViewModel.Current.Connection, string.Format("Delete from User_ProfileRole where  FKProfile = {0} and FKRole = {1}", UserAdminViewModel.Current.SelectedProfile.PK, r.FKRole));

            UserAdminViewModel.Current.LoadProfileRoles();
            UserAdminViewModel.Current.LoadUserRoles();
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void btnUserAddProfile_Click(object sender, RoutedEventArgs e)
      {
         if (UserAdminViewModel.Current.SelectedUser.PK == 0)
         {
            MessageBox.Show("You should save the user first.", "Oups ...", MessageBoxButton.OK, MessageBoxImage.Stop);
            return;
         };

         if (dgUser_AllProfiles.SelectedItem != null)
         {
            User_Profile p = dgUser_AllProfiles.SelectedItem as User_Profile;

            if (DB_SQL.QuickQuery(UserViewModel.Current.Connection, string.Format("insert into User_UserProfile ( FKUser, FKProfile ) values ({0}, {1})", UserAdminViewModel.Current.SelectedUser.PK, p.PK)) != "1")
            {
               DB_SQL.MessageBoxSQL();
            };

            UserAdminViewModel.Current.LoadUserData();
         };
      }

      private void btnUserDelProfile_Click(object sender, RoutedEventArgs e)
      {
         if (dgUser_GrantedProfiles.SelectedItem != null)
         {
            User_UserProfile p = dgUser_GrantedProfiles.SelectedItem as User_UserProfile;

            DB_SQL.QuickQuery(UserViewModel.Current.Connection, string.Format("Delete from User_UserProfile where  FKUser = {0} and FKProfile = {1}", UserAdminViewModel.Current.SelectedUser.PK, p.FKProfile));
            UserAdminViewModel.Current.LoadUserData();
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void btnUserAddAddRole_Click(object sender, RoutedEventArgs e)
      {
         if (UserAdminViewModel.Current.SelectedUser.PK == 0)
         {
            MessageBox.Show("You should save the user first.", "Oups ...", MessageBoxButton.OK, MessageBoxImage.Stop);
            return;
         };

         if (dgUser_AllRoles.SelectedItem != null)
         {
            User_Role r = dgUser_AllRoles.SelectedItem as User_Role;
            User_Role_DelAdd r_da = new User_Role_DelAdd()
            {
               FKUser = UserAdminViewModel.Current.SelectedUser.PK,
               FKRole = r.PK,
               RoleData = r.RoleData,
            };

            //string SQL = string.Format("insert into User_RoleAdd ( FKUser, FKRole ) values ({0}, {1})", UserAdminViewModel.Current.SelectedUser.PK, r.PK);

            //if (!string.IsNullOrEmpty(r.RoleData))
            //{
            //   SQL = string.Format("insert into User_RoleAdd ( FKUser, FKRole, RoleData ) values ({0}, {1}, '{2}')", UserAdminViewModel.Current.SelectedUser.PK, r.PK, r.RoleData);
            //};

            //if (DB_SQL.QuickQuery(UserViewModel.Current.Connection, SQL) != "1")

            if (!DB_SQL.Insert(UserViewModel.Current.Connection, "User_RoleAdd", r_da, SetPK: true))
            {
               DB_SQL.MessageBoxSQL();
            };

            UserAdminViewModel.Current.LoadUserData();
         };
      }

      private void btnUserDelAddRole_Click(object sender, RoutedEventArgs e)
      {
         if (dgUser_AddRoles.SelectedItem != null)
         {
            User_Role_DelAdd r = dgUser_AddRoles.SelectedItem as User_Role_DelAdd;

            DB_SQL.QuickQuery(UserViewModel.Current.Connection, string.Format("Delete from User_RoleAdd where  FKUser = {0} and FKRole = {1}", UserAdminViewModel.Current.SelectedUser.PK, r.FKRole));
            UserAdminViewModel.Current.LoadUserData();
         };
      }

      private void btnUserAddDelRole_Click(object sender, RoutedEventArgs e)
      {
         if (UserAdminViewModel.Current.SelectedUser.PK == 0)
         {
            MessageBox.Show("You should save the user first.", "Oups ...", MessageBoxButton.OK, MessageBoxImage.Stop);
            return;
         };

         if (dgUser_AllRoles.SelectedItem != null)
         {
            User_Role r = dgUser_AllRoles.SelectedItem as User_Role;

            if (DB_SQL.QuickQuery(UserViewModel.Current.Connection, string.Format("insert into User_RoleDel ( FKUser, FKRole ) values ({0}, {1})", UserAdminViewModel.Current.SelectedUser.PK, r.PK)) != "1")
            {
               DB_SQL.MessageBoxSQL();
            };

            UserAdminViewModel.Current.LoadUserData();
         };
      }

      private void btnUserDelDelRole_Click(object sender, RoutedEventArgs e)
      {
         if (dgUser_DelRoles.SelectedItem != null)
         {
            User_Role_DelAdd r = dgUser_DelRoles.SelectedItem as User_Role_DelAdd;

            DB_SQL.QuickQuery(UserViewModel.Current.Connection, string.Format("Delete from User_RoleDel where  FKUser = {0} and FKRole = {1}", UserAdminViewModel.Current.SelectedUser.PK, r.FKRole));
            UserAdminViewModel.Current.LoadUserData();
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void Page_Loaded(object sender, RoutedEventArgs e)
      {
         if (DB_SQL._ViewModel.CurrentTransaction != null)
         {
            if (Debugger.IsAttached)
            {
               Debugger.Break();
            };
         };

         btnUser_Badge.Visibility = (OnPrintBadge == null ? Visibility.Collapsed : Visibility.Visible);
      }

      private void Page_Unloaded(object sender, RoutedEventArgs e)
      {
         UserViewModel.Current.Connection.RollbackTransaction(false);
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void btnUser_Print_Click(object sender, RoutedEventArgs e)
      {
         if (OnPrintBadge != null)
         {
            OnPrintBadge(sender, e);
         };
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

      private void dgUser_AddRoles_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         if (sender == dgUser_AddRoles)
         {
            if (dgUser_AddRoles.SelectedItem != null)
            {
               UserAdminViewModel.Current.SelectUserRole_DelAdd(dgUser_AddRoles.SelectedItem as User_Role_DelAdd);
            }
            else
            {
               UserAdminViewModel.Current.SelectUserRole_DelAdd(null);
            };
         }
         else if (sender == dgUser_DelRoles)
         {
            UserAdminViewModel.Current.SelectUserRole_DelAdd(null);
         }
         else
         {
            UserAdminViewModel.Current.SelectUserRole_DelAdd(null);
         };
      }

      private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
      {
         // DB_SQL.Update("User_RoleAdd", UserViewModel.Current.CurrentUser_Role_DelAdd);
      }

      private void Button_Click(object sender, RoutedEventArgs e)
      {
         UserAdminViewModel.Current.CurrentUser_Role_DelAdd.Expiration = DateTime.MaxValue;
         UserAdminViewModel.Current.UpdateCurrentUser_Role_Add();
      }

      private void btnToday_Click(object sender, RoutedEventArgs e)
      {
         UserAdminViewModel.Current.CurrentUser_Role_DelAdd.Expiration = DateTime.Now.Date;
         UserAdminViewModel.Current.UpdateCurrentUser_Role_Add();
      }

      private void OnReturn(object sender)
      {
         tbDate.FormatOutput();
         UserAdminViewModel.Current.UpdateCurrentUser_Role_Add();
      }

      private void OnReturnData(object sender)
      {
         UserAdminViewModel.Current.UpdateCurrentUser_Role_Add();
      }

      private void TextBoxEx_LostFocus(object sender, RoutedEventArgs e)
      {
         UserAdminViewModel.Current.UpdateCurrentUser_Role_Add();
      }

      // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
   }
}
