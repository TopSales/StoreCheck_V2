using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ZPF;
using ZPF.AT;
using ZPF.SQL;

public class UserAdminViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   private static UserAdminViewModel _Instance = null;

   public static UserAdminViewModel Current
   {
      get
      {
         if (_Instance == null)
         {
            _Instance = new UserAdminViewModel();
         };

         return _Instance;
      }

      set
      {
         _Instance = value;
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public UserAdminViewModel()
   {
      _Instance = this;

      worker.DoWork += worker_DoWork;
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public ObservableCollection<User_ProfileRole> UserRoles { get; set; } = new ObservableCollection<User_ProfileRole>();
   public ObservableCollection<User_UserProfile> UserProfiles { get; set; } = new ObservableCollection<User_UserProfile>();
   public ObservableCollection<User_Role_DelAdd> User_DelRoles { get; set; } = new ObservableCollection<User_Role_DelAdd>();
   public ObservableCollection<User_Role_DelAdd> User_AddRoles { get; set; } = new ObservableCollection<User_Role_DelAdd>();
   public ObservableCollection<User_ProfileRole> ProfileRoles { get; set; } = new ObservableCollection<User_ProfileRole>();

   public ObservableCollection<User_Role> Roles { get; set; } = new ObservableCollection<User_Role>();
   public ObservableCollection<User_Profile> Profiles { get; set; } = new ObservableCollection<User_Profile>();
   public ObservableCollection<UserAccount> Users { get; set; } = new ObservableCollection<UserAccount>();

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   /// <summary>
   /// Selected user (eg in a list)
   /// </summary>
   public UserAccount SelectedUser
   {
      get { return _SelectedUser; }
      set
      {
         if (SetField(ref _SelectedUser, value))
         {
            SelectUserRole_DelAdd(null);
            LoadUserData();
         };
      }
   }
   private UserAccount _SelectedUser;

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public User_Profile SelectedProfile
   {
      get { return _SelectedProfile; }
      set
      {
         if (SetField(ref _SelectedProfile, value))
         {
            LoadProfileRoles();
         };
      }
   }
   private User_Profile _SelectedProfile;

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   // ????
   public User_Role_DelAdd CurrentUser_Role_DelAdd { get => _CurrentUser_Role_DelAdd; set => _CurrentUser_Role_DelAdd = value; }
   public object DBViewModel { get; internal set; }

   User_Role_DelAdd _CurrentUser_Role_DelAdd = null;

   public void SelectUserRole_DelAdd(User_Role_DelAdd user_Role_DelAdd)
   {
      CurrentUser_Role_DelAdd = user_Role_DelAdd;
      OnPropertyChanged("CurrentUser_Role_DelAdd");
   }

   public void UpdateCurrentUser_Role_Add()
   {
      DB_SQL.Update(UserViewModel.Current.Connection, "User_RoleAdd", CurrentUser_Role_DelAdd);
      OnPropertyChanged("CurrentUser_Role_DelAdd");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   /// <summary>
   /// LoadUserData() is specific for the WPF admin dlg
   /// </summary>
   public void LoadUserData()
   {
      BackboneViewModel.Current.IncBusy();

      UserProfiles.Clear();

      foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, string.Format("select * from User_UserProfile where FKUser={0}", _SelectedUser.PK)) as DataTable).AsEnumerable())
      {
         User_UserProfile t = new User_UserProfile();
         t.CopyDataRowValues(r, true);

         DoIt.OnMainThread(() =>
         {
            UserProfiles.Add(t);
         });
      };

      OnPropertyChanged("UserProfiles");

      // - - -  - - -

      User_AddRoles.Clear();

      foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, string.Format("select User_RoleAdd.* from User_RoleAdd, User_Role where User_RoleAdd.FKRole=User_Role.PK and FKUser={0} order by User_Role.Label", _SelectedUser.PK)) as DataTable).AsEnumerable())
      {
         User_Role_DelAdd t = new User_Role_DelAdd();
         t.CopyDataRowValues(r, true);

         DoIt.OnMainThread(() =>
         {
            User_AddRoles.Add(t);
         });
      };

      OnPropertyChanged("User_AddRoles");

      // - - -  - - -

      User_DelRoles.Clear();
      foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, string.Format("select User_RoleDel.* from User_RoleDel, User_Role where User_RoleDel.FKRole=User_Role.PK and FKUser={0} order by User_Role.Label", _SelectedUser.PK)) as DataTable).AsEnumerable())
      {
         User_Role_DelAdd t = new User_Role_DelAdd();
         t.CopyDataRowValues(r, true);

         DoIt.OnMainThread(() =>
         {
            User_DelRoles.Add(t);
         });
      };

      OnPropertyChanged("User_DelRoles");

      // - - -  - - -

      LoadUserRoles();

      BackboneViewModel.Current.DecBusy();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public void LoadUserRoles()
   {
      if (_SelectedUser == null)
      {
         return;
      }

      UserRoles.Clear();

      try
      {
         string SQL = UserViewModel.Current.GetRolesSQL();

         foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, SQL.Replace("1=1", " 1=1 and UserAccount.PK=" + _SelectedUser.PK)) as DataTable).AsEnumerable())
         {
            User_ProfileRole t = new User_ProfileRole();
            t.CopyDataRowValues(r, true);
            UserRoles.Add(t);
         };

         OnPropertyChanged("UserRoles");
      }
      catch (Exception ex)
      {
         Log.Write(ErrorLevel.Error, ex);

         if (Debugger.IsAttached)
         {
            Debugger.Break();
         };
      };
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   public void LoadProfileRoles()
   {
      ProfileRoles.Clear();

      foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, string.Format("select User_ProfileRole.FKProfile, User_ProfileRole.FKRole from User_ProfileRole, User_Role where User_ProfileRole.FKRole=User_Role.PK and FKProfile={0} order by User_Role.Label", _SelectedProfile.PK)) as DataTable).AsEnumerable())
      {
         User_ProfileRole t = new User_ProfileRole();
         t.CopyDataRowValues(r, true);

         DoIt.OnMainThread(() =>
         {
            ProfileRoles.Add(t);
         });
      };

      OnPropertyChanged("ProfileRoles");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

   private readonly BackgroundWorker worker = new BackgroundWorker();

   public void LoadData(bool UseBackGroundWorker)
   {
      if (worker.IsBusy)
      {
         return;
      };

      BackboneViewModel.Current.IncBusy();

      Users.Clear();
      Profiles.Clear();
      Roles.Clear();

      if (UseBackGroundWorker)
      {
         worker.RunWorkerAsync();
      }
      else
      {
         worker_DoWork(null, null);
      };
   }

   private void worker_DoWork(object sender, DoWorkEventArgs e)
   {
      try
      {
         // - - -  - - -

         foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, "select * from UserAccount order by Login") as DataTable).AsEnumerable())
         {
            UserAccount t = new UserAccount();
            t.CopyDataRowValues(r, true);

            DoIt.OnMainThread(() =>
            {
               Users.Add(t);
            });
         };

         OnPropertyChanged("Users");

         // - - -  - - -

         foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, "select * from User_Profile order by Label") as DataTable).AsEnumerable())
         {
            User_Profile t = new User_Profile();
            t.CopyDataRowValues(r, true);

            DoIt.OnMainThread(() =>
            {
               Profiles.Add(t);
            });
         };

         OnPropertyChanged("Users");

         // - - -  - - -

         foreach (DataRow r in (DB_SQL.QuickQueryView(UserViewModel.Current.Connection, "select * from User_Role order by Label") as DataTable).AsEnumerable())
         {
            User_Role t = new User_Role();
            t.CopyDataRowValues(r, true);

            DoIt.OnMainThread(() =>
            {
               Roles.Add(t);
            });
         };

         OnPropertyChanged("Roles");

         // - - -  - - -

      }
      catch (Exception ex)
      {
         Debug.WriteLine(ex.Message);
         Debug.WriteLine(UserViewModel.Current.Connection.LastQuery);
         Debug.WriteLine(UserViewModel.Current.Connection.LastError);

         if (Debugger.IsAttached)
         {
            Debugger.Break();
         }

         UserViewModel.Current.CurrentUser = null;
         UserViewModel.Current.CurrentUserState = UserViewModel.UserState.NotLogged;
      };

      BackboneViewModel.Current.DecBusy();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
}

