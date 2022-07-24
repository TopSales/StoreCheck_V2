
using System;
using ZPF;
using System.Windows.Forms;
using System.IO;

#if WinCE
using OpenNETCF.Win32;
#endif

namespace Configurator
{
   /// <summary>
   /// Summary description for ConfigHelper.
   /// </summary>
   public class ConfigHelper
   {
      public ConfigHelper()
      {
         //
         // TODO: Add constructor logic here
         //
      }

      public static TStrings Configuration = new TStrings();

      public const string Version = "0.99.2";

      public static bool ApplyConfig(ProgressBar progressBar, Label labelState)
      {
         bool Result = false;

         bool WarmBoot = false;
         bool ColdBoot = false;
         bool Exit = false;

         //string NO = "";

         //
         // - - - First Pass - Get It - - -

         if (progressBar != null)
         {
            progressBar.Maximum = Configuration.Count * 2;
            progressBar.Value = 0;
         };

         for (int i = 0; i < Configuration.Count; i++)
         {
            if (progressBar != null)
            {
               progressBar.Value = progressBar.Value + 1;
#if WinCE
               if (labelState != null) labelState.Text = String.Format("{0}/{1}", progressBar.Value, progressBar.Maximum);
#endif
            };

            Application.DoEvents();

            if (Configuration[i].StartsWith("NO="))
            {
#if WinCE
               InputNum dlg = new InputNum();

               dlg.Text = "N° Terminal";
               dlg.labelCaption.Text = "N° Terminal";

               dlg.maskedEdit.InputMask = Configuration[ i ].Substring( 3 );
               dlg.maskedEdit.Text = "";

               if( dlg.ShowDialog() == DialogResult.OK )
               {
                  NO = dlg.maskedEdit.Value;
               }
               else
               {
                  NO = Configuration[ i ].Substring( 3 );
               };
#endif
            }
            else
            {
            };
         };

         //
         // - - - Second Pass - Do It - - -

         for (int i = 0; i < Configuration.Count; i++)
         {
            if (progressBar != null)
            {
               progressBar.Value = progressBar.Value + 1;
#if WinCE
               if (labelState != null) labelState.Text = String.Format("{0}/{1}", progressBar.Value, progressBar.Maximum);
#endif
            };

            Application.DoEvents();

#if WinCE
            if (Configuration[i].StartsWith("ID="))
            {
               string st = Configuration[i].Substring(3);

               try
               {
                  TStrings Identity = new TStrings();
                  Identity.Add(@"[HKEY_LOCAL_MACHINE\Ident]");

                  Identity.Add(String.Format("\"Name\"=\"{0}\"", String.Format(st, NO)));

                  Identity.SaveToFile(@"\Application\Identity.reg");

                  RegistryHelper.InstallREG(@"\Application\Identity.reg");
               }
               catch { };
            }
            else if (Configuration[i].StartsWith("AB_IP="))
            {
               TStrings File = new TStrings();
               File.LoadFromFile(@"\Application\AirBeam.reg");
               File["\"SERVERIP\""] = '"' + Configuration[i].Substring(6) + '"';
               File.SaveToFile(@"\Application\AirBeam.reg");

               RegistryHelper.InstallREG(@"\Application\AirBeam.reg");
            }
            else if (Configuration[i].StartsWith("AB_DIR="))
            {
               TStrings File = new TStrings();
               File.LoadFromFile(@"\Application\AirBeam.reg");
               File["\"PACKAGEDIR\""] = '"' + Configuration[i].Substring(7) + '"';
               File.SaveToFile(@"\Application\AirBeam.reg");

               RegistryHelper.InstallREG(@"\Application\AirBeam.reg");
            }
            else if (Configuration[i].StartsWith("AB_USER="))
            {
               TStrings File = new TStrings();
               File.LoadFromFile(@"\Application\AirBeam.reg");
               File["\"FTPUSER\""] = '"' + Configuration[i].Substring(8) + '"';
               File.SaveToFile(@"\Application\AirBeam.reg");

               RegistryHelper.InstallREG(@"\Application\AirBeam.reg");
            }
            else if (Configuration[i].StartsWith("AB_PWD="))
            {
               TStrings File = new TStrings();
               File.LoadFromFile(@"\Application\AirBeam.reg");
               File["\"FTPPASSWORD\""] = '"' + Configuration[i].Substring(7) + '"';
               File.SaveToFile(@"\Application\AirBeam.reg");

               RegistryHelper.InstallREG(@"\Application\AirBeam.reg");
            }
            else if (Configuration[i].StartsWith("AB_PKG1="))
            {
               TStrings File = new TStrings();
               File.LoadFromFile(@"\Application\AirBeam.reg");
               File["\"PACKAGE1\""] = '"' + Configuration[i].Substring(8) + '"';
               File.SaveToFile(@"\Application\AirBeam.reg");

               RegistryHelper.InstallREG(@"\Application\AirBeam.reg");
            }
            else if (Configuration[i].StartsWith("AB_PKG2="))
            {
               TStrings File = new TStrings();
               File.LoadFromFile(@"\Application\AirBeam.reg");
               File["\"PACKAGE2\""] = '"' + Configuration[i].Substring(8) + '"';
               File.SaveToFile(@"\Application\AirBeam.reg");

               RegistryHelper.InstallREG(@"\Application\AirBeam.reg");
            }
            else if (Configuration[i].StartsWith("DELETE="))
            {
               if (File.Exists(Configuration[i].Substring(7)))
               {
                  try
                  {
                     File.Delete(Configuration[i].Substring(8));
                  }
                  catch { };
               };
            }
            else if (Configuration[i].StartsWith("INVPDT="))
            {
               TStrings File = new TStrings();
               File.LoadFromFile(@"\Application\INVPDT.cfg");

               if (File.Count == 0)
               {
                  File.Add("No");
                  File.Add("10.245.xxx.144");
                  File.Add("ftpuser");
                  File.Add("010106");
                  File.Add("00021");
               };

               File[0] = NO;
               File[1] = Configuration[i].Substring(7);

               File.SaveToFile(@"\Application\INVPDT.cfg");
            }
            else if (Configuration[i].StartsWith("KIOSK_DOM="))
            {
               TIniFile Ini = new TIniFile("\\Application\\Kiosk.ini");
               Ini.WriteInteger("General", "Domain", Int16.Parse(Configuration[i].Substring(10)));
               Ini.UpdateFile();
            }
            else if (Configuration[i].StartsWith("KIOSK_URL="))
            {
               TIniFile Ini = new TIniFile("\\Application\\Kiosk.ini");
               Ini.WriteString("General", "URLINV", Configuration[i].Substring(10));
               Ini.UpdateFile();
            }
            else if (Configuration[i].StartsWith("KIOSK_PWD="))
            {
               TStrings File = new TStrings();
               File.Add(@"[HKEY_LOCAL_MACHINE\Software\Metro\Kiosk]");
               File.Add(String.Format("\"Password\"=\"{0}\"", Configuration[i].Substring(10)));
               File.SaveToFile(@"\Application\Kiosk.reg");

               RegistryHelper.InstallREG(@"\Application\Kiosk.reg");
            }
            else if (Configuration[i].StartsWith("RUN="))
            {
               Basics.Exec(Configuration[i].Substring(4), "", false);
            }
            else if (Configuration[i] == "{Sign}")
            {
               TStrings Sign = new TStrings();
               Sign.Add(@"[HKEY_CURRENT_USER\Software\Configurator]");
               Sign.Add("\"Signed\"=dword:00000001");
               Sign.SaveToFile(@"\Application\ConfiguratorSigned.reg");

               RegistryHelper.InstallREG(@"\Application\ConfiguratorSigned.reg");

               //RegistryKey registryKey = Registry.CurrentUser.CreateSubKey( @"\Software\Configurator" );
               //registryKey.SetValue( "Signed", (int)(1) );
               //registryKey.Close();
            }
            else 
#endif
            if (Configuration[i] == "{WarmBoot}")
            {
               WarmBoot = true;
            }
            else if (Configuration[i] == "{ColdBoot}")
            {
               ColdBoot = true;
            }
            else if (Configuration[i] == "{Exit}")
            {
               Exit = true;
            }
            else
            {
            };
         };

         //
         // - - -  - - - 

#if WinCE
         if (ColdBoot) Platform.ColdBoot();
         if (WarmBoot) Platform.WarmBoot();
#endif
         if (Exit) Result = true;

         return Result;
      }

      // http://www.codeproject.com/csharp/csRedundancyChckAlgorithm.asp
      // CCITT-8 CRC algorithm
      public static byte CCITT8(string data)
      {
         byte[] CRCTable = new byte[256]
         {
            0x00, 0x31, 0x62, 0x53, 0xC4, 0xF5, 0xA6, 0x97,
            0xB9, 0x88, 0xDB, 0xEA, 0x7D, 0x4C, 0x1F, 0x2E,
            0x43, 0x72, 0x21, 0x10, 0x87, 0xB6, 0xE5, 0xD4,
            0xFA, 0xCB, 0x98, 0xA9, 0x3E, 0x0F, 0x5C, 0x6D,
            0x86, 0xB7, 0xE4, 0xD5, 0x42, 0x73, 0x20, 0x11,
            0x3F, 0x0E, 0x5D, 0x6C, 0xFB, 0xCA, 0x99, 0xA8,
            0xC5, 0xF4, 0xA7, 0x96, 0x01, 0x30, 0x63, 0x52,
            0x7C, 0x4D, 0x1E, 0x2F, 0xB8, 0x89, 0xDA, 0xEB,
            0x3D, 0x0C, 0x5F, 0x6E, 0xF9, 0xC8, 0x9B, 0xAA,
            0x84, 0xB5, 0xE6, 0xD7, 0x40, 0x71, 0x22, 0x13,
            0x7E, 0x4F, 0x1C, 0x2D, 0xBA, 0x8B, 0xD8, 0xE9,
            0xC7, 0xF6, 0xA5, 0x94, 0x03, 0x32, 0x61, 0x50,
            0xBB, 0x8A, 0xD9, 0xE8, 0x7F, 0x4E, 0x1D, 0x2C,
            0x02, 0x33, 0x60, 0x51, 0xC6, 0xF7, 0xA4, 0x95,
            0xF8, 0xC9, 0x9A, 0xAB, 0x3C, 0x0D, 0x5E, 0x6F,
            0x41, 0x70, 0x23, 0x12, 0x85, 0xB4, 0xE7, 0xD6,
            0x7A, 0x4B, 0x18, 0x29, 0xBE, 0x8F, 0xDC, 0xED,
            0xC3, 0xF2, 0xA1, 0x90, 0x07, 0x36, 0x65, 0x54,
            0x39, 0x08, 0x5B, 0x6A, 0xFD, 0xCC, 0x9F, 0xAE,
            0x80, 0xB1, 0xE2, 0xD3, 0x44, 0x75, 0x26, 0x17,
            0xFC, 0xCD, 0x9E, 0xAF, 0x38, 0x09, 0x5A, 0x6B,
            0x45, 0x74, 0x27, 0x16, 0x81, 0xB0, 0xE3, 0xD2,
            0xBF, 0x8E, 0xDD, 0xEC, 0x7B, 0x4A, 0x19, 0x28,
            0x06, 0x37, 0x64, 0x55, 0xC2, 0xF3, 0xA0, 0x91,
            0x47, 0x76, 0x25, 0x14, 0x83, 0xB2, 0xE1, 0xD0,
            0xFE, 0xCF, 0x9C, 0xAD, 0x3A, 0x0B, 0x58, 0x69,
            0x04, 0x35, 0x66, 0x57, 0xC0, 0xF1, 0xA2, 0x93,
            0xBD, 0x8C, 0xDF, 0xEE, 0x79, 0x48, 0x1B, 0x2A,
            0xC1, 0xF0, 0xA3, 0x92, 0x05, 0x34, 0x67, 0x56,
            0x78, 0x49, 0x1A, 0x2B, 0xBC, 0x8D, 0xDE, 0xEF,
            0x82, 0xB3, 0xE0, 0xD1, 0x46, 0x77, 0x24, 0x15,
            0x3B, 0x0A, 0x59, 0x68, 0xFF, 0xCE, 0x9D, 0xAC
         };

         byte crc = 0;

         for (int i = 0; i < data.Length; i++)
         {
            crc = CRCTable[(crc ^ data[i]) & 0xFF];
         }
         return crc;
      }
   }
}
