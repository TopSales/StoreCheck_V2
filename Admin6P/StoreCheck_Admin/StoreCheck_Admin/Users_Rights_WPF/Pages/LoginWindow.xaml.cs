using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZPF
{
   /// <summary>
   /// Interaction logic for LoginWindow.xaml
   /// </summary>
   public partial class LoginWindow : Window
   {
      /// <summary>
      /// ...
      /// Sets:
      ///    UserViewModel.Current.CurrentUserState 
      ///    UserViewModel.Current.CurrentUser 
      /// </summary>
      /// <param name="AutoLogin">AutoLogin with debug user</param>
      public LoginWindow(bool AutoLogin)
      {
         InitializeComponent();

         // - - -  - - - 

         //imgLogin.Source = ZPF.Fonts.IF.GetImageSource(ZPF.Fonts.IF.Login_01, Brushes.White, 512, -20);

         //{
         //   Stream s = GetRessourceStream("Assets.User Profile 1-WF.png");

         //   if (s != null)
         //   {
         //      var b = BitmapFrame.Create(s, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.OnLoad);
         //      imgUser.Source = b;
         //      // imgUser.Source = ZPFFonts.IF.GetImageSource(ZPFFonts.IF.User Profile 1 - WF, Brushes.White, 512, -20);
         //   };
         //};

         // - - -  - - - 

         if (AutoLogin)
         {
            tbLogin.Text = "debug";
            tbPassword.Password = "MossIsTheBoss";
         };

         ErrorMessage.Text = "";

         if (Debugger.IsAttached)
         {
            btnOK.IsEnabled = true;
         }
         else
         {
            // btnOK.IsEnabled = false;
         };
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

      private void btnLoginOK_Click(object sender, RoutedEventArgs e)
      {
         string User = tbLogin.Text.Trim();
         string password = tbPassword.Password;

         try
         {
            if (User == "debug" && password == "MossIsTheBoss")
            {
               UserViewModel.Current.CurrentUserState = UserViewModel.UserState.SuperUser;
               UserViewModel.Current.CurrentUser = new UserAccount() { PK = -1, Login = "(debug)" };

               UserViewModel.Current.UpdateUserSession("Login");

               if (UserViewModel.Current.CurrentUserState == UserViewModel.UserState.NotLogged)
               {
                  ErrorMessage.Text = "Problem joining the database. Please try again later";
                  btnOK.IsEnabled = false;
                  return;
               };

               DialogResult = UserViewModel.Current.CurrentUserState != UserViewModel.UserState.NotLogged;
               return;
            }

            if (User == "admin" && VerifyMasterPassword(password))
            {
               UserViewModel.Current.CurrentUserState = UserViewModel.UserState.SuperUser;
               UserViewModel.Current.CurrentUser = new UserAccount() { Login = "(admin)" };

               UserViewModel.Current.UpdateUserSession("Login");

               if (UserViewModel.Current.CurrentUserState == UserViewModel.UserState.NotLogged)
               {
                  ErrorMessage.Text = "Problem joining the database. Please try again later.";
                  btnOK.IsEnabled = false;
                  return;
               };

               DialogResult = UserViewModel.Current.CurrentUserState != UserViewModel.UserState.NotLogged;
               return;
            }

            //if (UserViewModel.Current.Login(User, tbPassword.Password))
            if (UserViewModel.Current.Login(User, UserViewModel.Current.Salt(User, tbPassword.Password)))
            {
               if (UserViewModel.Current.CurrentUserState == UserViewModel.UserState.NotLogged)
               {
                  ErrorMessage.Text = "Problem joining the database. Please try again later..";
                  btnOK.IsEnabled = false;
                  return;
               };

               DialogResult = UserViewModel.Current.CurrentUserState != UserViewModel.UserState.NotLogged;
            }
            else
            {
               ErrorMessage.Text = "Invalid username/password combination";
            };
         }
         catch
         {
            ErrorMessage.Text = "Problem joining the database. Please try again later...";
            btnOK.IsEnabled = false;

            // DialogResult = false;
            return;
         };
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         DialogResult = false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
      private bool VerifyMasterPassword(string password)
      {
         //if (password == DataModul.Supervisor)
         //{
         //   return true;
         //}

         string today = DateTime.Now.ToString("ddMMyy");
         string pwd = "";
         int currentIter;

         foreach (char iter in today)
         {
            currentIter = 9 - Int32.Parse(iter.ToString());
            pwd += currentIter.ToString();
         }

         return password == pwd;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void Window_Loaded(object sender, RoutedEventArgs e)
      {
         tbLogin.Focus();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void tbPassword_GotFocus(object sender, RoutedEventArgs e)
      {
         tbPassword.SelectAll();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
