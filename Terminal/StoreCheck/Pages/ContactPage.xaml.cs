using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZPF;
using ZPF.XF.Compos;

namespace StoreCheck.Pages
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ContactPage : Page_Base
   {
      public IContact Contact { get => _Contact; set { _Contact = value; OnPropertyChanged(); } }
      IContact _Contact = null;

      public ContactPage(IContact contact)
      {
         Contact = contact;

         this.BindingContext = Contact;

         InitializeComponent();

         Title = "contact";

         // - - -  - - - 

         var tiles = SetAppBarContent(new List<AppBarItem>(new AppBarItem[]
         {
            //new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Contact), "appeler"),
            //new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.SMS), "SMS"),
            new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Mail_03), "mail"),
         }));

         //tiles[0].Clicked += (object sender, System.EventArgs e) =>
         //{
         //   (sender as Tile).IsEnabled = false;

         //   Device.OpenUri(new Uri($"tel:{Contact.Telephone}"));

         //   (sender as Tile).IsEnabled = true;
         //};

         //tiles[1].Clicked += async (object sender, System.EventArgs e) =>
         //{
         //   try
         //   {
         //      var message = new SmsMessage("", new[] { Contact.Telephone });
         //      await Sms.ComposeAsync(message);
         //   }
         //   catch (FeatureNotSupportedException ex)
         //   {
         //      // Sms is not supported on this device.
         //   }
         //   catch (Exception ex)
         //   {
         //      // Other error has occurred.
         //   }
         //};

         tiles[0].Clicked += (object sender, System.EventArgs e) =>
         {
            (sender as Tile).IsEnabled = false;

            Device.OpenUri(new Uri($"mailto:{Contact.Mail}?subject=StoreCheck"));

            (sender as Tile).IsEnabled = true;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }
}
