using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZPF
{
    /// <summary>
    /// Interaction logic for PasswordDlg.xaml
    /// </summary>
    public partial class PasswordDlg : Window
   {
      public PasswordDlg()
      {
         InitializeComponent();
         DataContext = UserAdminViewModel.Current;
         tbActionPassword.Text = "";
      }

      private void btnOK_Click(object sender, RoutedEventArgs e)
      {

         if (string.IsNullOrEmpty(UserAdminViewModel.Current.SelectedUser .Login))
         {
            BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, "", "Vous devez saisir un login pour créer votre password");
            return;
         }
         // ToDO CTRL Login unique
         int nbLogin = UserAdminViewModel.Current.Users.Where(a => a.Login == UserAdminViewModel.Current.SelectedUser.Login).Count();
         if (nbLogin > 1)
         {
            var acts = UserAdminViewModel.Current.Users.Where(a => a.Login == UserAdminViewModel.Current.SelectedUser.Login).ToList();// .FirstOrDefault();
            foreach (var act in acts)
            {
               if (act != null)
               {
                  if (UserAdminViewModel.Current.SelectedUser.PK != act.PK)
                  {
                     BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, "", "Ce login existe déja dans votre DB");
                     return;
                  }

               }
            }
         }
         else
         {
            if (nbLogin == 1)
            {
               if (UserAdminViewModel.Current.SelectedUser.PK != UserAdminViewModel.Current.Users.Where(a => a.Login == UserAdminViewModel.Current.SelectedUser.Login).FirstOrDefault().PK)
               {
                  BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, "", "Ce login existe déja dans votre DB");
                  return;
               }
            }
         }
         
         if (string.IsNullOrEmpty(tbActionPassword.Text.Trim()))
         {
            BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, "", "Vous devez saisir un password pour créer votre user");
            return;
         }
         UserAdminViewModel.Current.SelectedUser.Password = UserViewModel.Current.Salt(UserAdminViewModel.Current.SelectedUser.Login, tbActionPassword.Text.Replace(" ", ""));
         DialogResult = true;
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e)
      {
         DialogResult = false;
      }
   }
   public class LoginPwdConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value != null)
         {
            return value.ToString().Replace(" ", "");
         }
         else
         {
            return null;
         }

      }

      public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
      {
         if (value != null)
         {
            return value.ToString().Replace(" ", "");
         }
         else
         {
            return null;
         }
      }
   }
}
