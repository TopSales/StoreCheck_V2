using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZPF;
using ZPF.AT;
using ZPF.SQL;
using ZPF.XF.Compos;

class AboutPage : Page_Base
{
   public AboutPage()
   {
      Title = "about";

      // - - -  - - - 

      BindingContext = this;

      var p = new Panorama();
      p.AutoHideHeader = true;

      p.Tabs.Add(new Panorama.Tab() { Name = "about", View = new AboutView(), Selected = true });
      p.Tabs.Add(new Panorama.Tab() { Name = "what's new", View = new AboutWhatsNewView() });

      p.Redraw();

      SetMainContent(p);

      // - - -  - - - 

      var tiles = SetAppBarContent(new List<AppBarItem>(new AppBarItem[]
      {
         new AppBarItem(ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Msg_sent_WF), "Panic"),
      }));


      tiles[0].Clicked += (object sender, System.EventArgs e) =>
      {
         AboutPage.SendPanicMail(this);
      };
   }


   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

   public static async void SendPanicMail(Page parent)
   {
      string body = "";
      string FileName = MainViewModel.Current.DataFolder + "AuditTrail.csv";

      AuditTrailViewModel.Current.LoadAuditTrail(false, ZPF.AT.AuditTrailViewModel.Current.MaxLines);
      ListToCSV(AuditTrailViewModel.Current.AuditTrail.ToList(), FileName);

      // - - -  - - - 

      body += $"Cassini - Version {VersionInfo.Current.sVersion} - {VersionInfo.Current.BuildOn}" + Environment.NewLine;

      body += $"{DeviceInfo.Manufacturer} {DeviceInfo.Model} \r\n";
      body += $"{DeviceInfo.Platform} {DeviceInfo.VersionString} \r\n";

      body += Environment.NewLine;
      body += $"Device Idiom     {Device.Idiom.ToString()} \r\n";

      body += Environment.NewLine;
      body += $"ISOLanguageName [{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpper()}] \r\n";
      body += $"NumberFormat    [{CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator}][{CultureInfo.CurrentUICulture.NumberFormat.NumberDecimalSeparator}] \r\n";

      body += Environment.NewLine;
      //// 'You need to declare using the permission: android.permission.BATTERY_STATS in your AndroidManifest.xml'
      //body += $"Battery state  {Xamarin.Essentials.Battery.State.ToString()} \r\n";
      //body += $"Battery charge level {(int)(Xamarin.Essentials.Battery.ChargeLevel * 100)}% \r\n";
      //body += Environment.NewLine;

      ////ToDo: - Niveau/qualité de reception
      //int NivRecept = 0;

      List<string> toList = new List<string>();
      toList.Add("Support@ZPF.fr");

      var message = new EmailMessage
      {
         Subject = "StoreCheck - Panic mail",
         Body = body,
         To = toList,
      };

      if (!string.IsNullOrEmpty(FileName))
      {
         message.Attachments.Add(new EmailAttachment(FileName));
      };

      try
      {
         await Email.ComposeAsync(message);
      }
      catch (Exception ex)
      {
         // await parent.DisplayAlert("Oups ..", ex.Message, "ok");

         await Share.RequestAsync(new ShareFileRequest
         {
            Title = System.IO.Path.GetFileNameWithoutExtension(FileName),
            File = new ShareFile(FileName),
         });
      };
   }

   /// <summary>
   /// Creates the CSV from a generic list.
   /// </summary>;
   /// <typeparam name="T"></typeparam>;
   /// <param name="list">The list.</param>;
   /// <param name="csvFileName">Name of CSV (w/ path) w/ file ext.</param>;
   public static void ListToCSV<T>(List<T> list, string csvFileName)
   {
      if (list == null || list.Count == 0)
      {
         return;
      };

      if (!Directory.Exists(Path.GetDirectoryName(csvFileName)))
      {
         Directory.CreateDirectory(Path.GetDirectoryName(csvFileName));
      };

      using (var sw = new StreamWriter(csvFileName))
      {
         // - - - gets all properties - - - 

         PropertyInfo[] properties = typeof(T).GetProperties();

         // - - - create header - - -

         for (int i = 0; i < properties.Length - 1; i++)
         {
            var prop = properties[i];

            if (Attribute.GetCustomAttribute(prop, typeof(DB_Attributes.IgnoreAttribute)) == null && Attribute.GetCustomAttribute(prop, typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)) == null)
            {
               sw.Write(prop.Name + ",");
            };
         };

         sw.Write(sw.NewLine);

         // - - - create rows - - -

         foreach (var item in list)
         {
            for (int i = 0; i < properties.Length - 1; i++)
            {
               var prop = properties[i];

               if (Attribute.GetCustomAttribute(prop, typeof(DB_Attributes.IgnoreAttribute)) == null && Attribute.GetCustomAttribute(prop, typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)) == null)
               {
                  if (prop.PropertyType.Name == "String")
                  {
                     sw.Write($@"""{prop.GetValue(item)}"",");
                  }
                  else
                  {
                     sw.Write(prop.GetValue(item) + ",");
                  };
               };
            };

            sw.Write(sw.NewLine);
         }
      }
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
}



