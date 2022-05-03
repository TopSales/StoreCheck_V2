using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ZPF.SQL;
using ZPF.WPF.Compos;

namespace ZPF
{
   /// <summary>
   /// WPF & WS
   /// </summary>
   public partial class MainViewModel : BaseViewModel
   {
      public bool TestUnitaires { get; set; }
      public bool IsDebug { get; private set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public bool CheckDB()
      {
         bool Result = false;
         string SQL = string.Format("select Value from DBList where List = '{0}' and Param = '{1}'", "DBSchema", "Version");

         DBList dbList = DB_SQL.QueryFirst<DBList>(SQL);

         if (dbList != null)
         {
            // 24/02/17 - 1.0.1 - Add ListArticle
            // 24/02/17 - 1.0.1 - Add ListArticle_Line
            // 05/03/17 - 1.0.2 - Add UpdatedBy on several tables
            // 19/03/17 - 1.0.3 - Add FKListArticle to StockMVT FKEmplacement, RefEmplacment to ListeArticle_Line
            // 29/05/17 - 1.0.4 - Bugfix
            // 06/06/17 - 1.0.5 - Index
            // 07/08/17 - 1.0.6 - Add: Article.Commentaires
            // 08/08/17 - 1.0.7 - Add: DBList index
            // 26/08/17 - 1.0.8 - Add: Emplacement.IsSpecial
            // 15/09/17 - 1.0.9 - Add: Emplacement.FKEmplacement
            // 16/09/17 - 1.1.0 - Add: Famille.FKFamille, Article_Fournisseur, Report, EmplacementType, FamilleType, DocumentType.Parametres
            // 04/02/18 - 1.2.0 - Add: Client, ...
            // 05/02/18 - 1.2.1 - Add: Lot, Transport, Transporteur, Transport_Item, ...
            // 08/03/18 - 1.2.2 - Add: maj Lot, add AuditTrail, add Label, del Client, del Fournisseur, modif ListArticle
            // 15/03/18 - 1.2.3 - Add: maj Stock, Mouvement, Contact
            // 19/03/18 - 1.2.4 - Maj Contact.SortOrder
            // 21/03/18 - 1.2.5 - Add: Article.HasVariante, Article_Variante, Variante_Attribute, Variante_Type, Variante_Val
            // 26/03/18 - 1.2.6 - Maj: Lot, Stock, ListArticle_Line
            // 06/04/18 - 1.2.7 - Maj: ListArticle_Line
            // 16/04/18 - 1.2.8 - Maj: Ref (Article, Emplacement, Famille) not null, JustificationLitige, ListArticle_Line
            // 20/07/18 - 1.2.9 - Maj: UserSession & co (Wanao)
            // 02/08/18 - 1.2.10 - Add: Inventaire
            // 07/10/18 - 1.2.11 - Maj: Inventaire
            // 12/02/19 - 1.4.1 - Maj: StockMVT (QTStock) & Champs paramètres sur Familles, Emplacements
            // 06/03/19 - 1.4.2 - Maj: Document <-- BaseDoc, JustificationLitige --> Justification, Inventaire (+Justification)

            Result = (dbList.Value == "1.4.2");
         };

         return Result;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private void CleanAuditTrail()
      {
         {
            DateTime dt = DateTime.Now.AddYears(-1);
            DB_SQL.QuickQuery($"delete from AuditTrail where DATE(TimeStamp) <= {DB_SQL.DateTimeToSQL(DB_SQL._ViewModel.DBType, dt)}");
         };

         {
            DateTime dt = DateTime.Now.AddMonths(-1);
            DB_SQL.QuickQuery($"delete from AuditTrail where IsBusiness=0 and DATE(TimeStamp) <= {DB_SQL.DateTimeToSQL(DB_SQL._ViewModel.DBType, dt)}");
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private string _BonE_Prefix = "BC{0:00000}";
      public string BonE_Prefix
      {
         get { return _BonE_Prefix; }
         set { SetField(ref _BonE_Prefix, value); }
      }

      private string _BonE_Titre;
      public string BonE_Titre
      {
         get { return _BonE_Titre; }
         set { SetField(ref _BonE_Titre, value); }
      }

      private string _BonE_Desc;
      public string BonE_Desc
      {
         get { return _BonE_Desc; }
         set { SetField(ref _BonE_Desc, value); }
      }

      private string _BonS_Prefix = "BL{0:00000}";
      public string BonS_Prefix
      {
         get { return _BonS_Prefix; }
         set { SetField(ref _BonS_Prefix, value); }
      }

      private string _BonS_Titre;
      public string BonS_Titre
      {
         get { return _BonS_Titre; }
         set { SetField(ref _BonS_Titre, value); }
      }

      private string _BonS_Desc;

      public string BonS_Desc
      {
         get { return _BonS_Desc; }
         set { SetField(ref _BonS_Desc, value); }
      }

      public bool ReplaceWithBon { get; private set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public string MailAlerte
      {
         get => DB_SQL.QuickQuery("select Value from DBList where List = 'Sync' and Param = 'MailAlerte'");
         set
         {
            DBList c = DB_SQL.Query<DBList>("select * from DBList where List = 'Sync' and Param = 'MailAlerte'").FirstOrDefault();

            if (c != null)
            {
               c.Value = value;
               DB_SQL.Update(c);
            }
            else
            {
               DB_SQL.Insert(new DBList { List = "Sync", Param = "MailAlerte", Value = value, ValueType = DBList.ValueTypes.String });
            };
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void LoadIniFromDB(TIniFile IniFile)
      {
         List<DBList> list = DB_SQL.Query<DBList>("select * from DBList where List like 'Ini.%' ");

         if (list != null)
         {
            foreach (var e in list)
            {
               IniFile.WriteString(e.List.Replace("Ini.", ""), e.Param, e.Value);
            };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -

      private void SetIniBon(TIniFile IniFile)
      {
         BonE_Prefix = IniFile.ReadString("Bons", "BonE_Prefix", "BC{0:00000}");
         BonE_Titre = IniFile.ReadString("Bons", "BonE_Titre", "Bon de commande");
         BonE_Desc = IniFile.ReadString("Bons", "BonE_Desc", "? Fournisseur ?");

         BonS_Prefix = IniFile.ReadString("Bons", "BonS_Prefix", "BL{0:00000}");
         BonS_Titre = IniFile.ReadString("Bons", "BonS_Titre", "Bon de livraison");
         BonS_Desc = IniFile.ReadString("Bons", "BonS_Desc", "? Destinataire ?");

         ReplaceWithBon = IniFile.ReadBool("Bons", "ReplaceWithBon", true);
         ReplaceWithBon = true;

         BonE_Prefix = (BonE_Prefix == "*" ? "" : BonE_Prefix);
         BonE_Desc = (BonE_Desc == "*" ? "" : BonE_Desc);

         BonS_Prefix = (BonS_Prefix == "*" ? "" : BonS_Prefix);
         BonS_Desc = (BonS_Desc == "*" ? "" : BonS_Desc);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private void WriteIniToDB(TIniFile IniFile)
      {
         TStrings Sections = IniFile.ReadSections();

         for (int iS = 0; iS < Sections.Count; iS++)
         {
            TStrings Values = IniFile.ReadSectionValues(Sections[iS]);

            for (int iV = 0; iV < Values.Count; iV++)
            {
               DBList db = null;

               db = DB_SQL.QueryFirst<DBList>($"select * from DBList where List='Ini.{Sections[iS]}' and Param='{Values.Names(iV)}'");

               if (db == null)
               {
                  db = new DBList
                  {
                     List = $"'Ini.{Sections[iS]}'",
                     Param = Values.Names(iV),
                     Value = Values.ValueFromIndex(iV),
                     ValueType = DBList.ValueTypes.Unknown,
                  };

                  DB_SQL.Insert(db);
               }
               else
               {
                  db.Value = Values.ValueFromIndex(iV);

                  DB_SQL.Update(db);
               };
            };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  
      /*
   <ScrollViewer x:Name="scrollViewer" VerticalScrollBarVisibility="Auto" >
      <StackPanel>

         <StackPanel Orientation="Vertical" Margin="0,10,0,10" 
                  Visibility="{Binding ActualWidth, ConverterParameter=&gt; 110, Converter={StaticResource Size2Visibility}, ElementName=scrollViewer, Mode=OneWay}" >

            <Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="HOME" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <Image Source="Images/Tiles/House-07.png" Width="60" />
                  <TextBlock Text="Dashboard" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="GUICH" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <!--<Image Source="Images/Tiles/Counting-Machine.png" Width="60" />-->
                  <!--<Image Source="Images/Tiles/Parcel.png" Width="60" />-->
                  <Image Source="Images/Tiles/delivery-packages-on-a-trolley.png" Width="60" />
                  <TextBlock Text="Entrées / sorties" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <!--<Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="GUICH" >
            <StackPanel Orientation="Vertical" Margin="8">
               <Image Source="Images/Tiles/Shopping-Cart-02.png" Width="60" />
               <TextBlock Text="Magasin" Foreground="White" HorizontalAlignment="Center" />
            </StackPanel>
         </Controls:Tile>-->

            <Controls:Tile 
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     CommandParameter="STOCK" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <Image Source="Images/Tiles/Warehouse-01-WF.png" Width="60" />
                  <TextBlock Text="Stock" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <!-- Articles, Sites, Clients -->

            <Controls:Tile Visibility="Collapsed"
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     CommandParameter="PRINT" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <Image Source="Images/Tiles/Print - 01.png" Width="60" />
                  <TextBlock Text="Impression" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     CommandParameter="IMP_EXP" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <Image Source="Images/Tiles/Data Sync-WF.png" Width="60" />
                  <TextBlock Text="Import / export" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile Visibility="Collapsed"
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     CommandParameter="USERS" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <Image Source="Images/Tiles/User-Group.png" Width="60" />
                  <TextBlock Text="Utilisateurs" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile x:Name="tileFactures"
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     Visibility="{Binding Main.IsFuture, Converter={StaticResource ToVisibility}, Mode=OneWay, Source={StaticResource Locator}}" 
                     CommandParameter="Factures" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <!--<Image Source="Images/Tiles/Analytics.png" Width="60" />-->
                  <TextBlock Style="{DynamicResource IconFont}" FontSize="48" Text="{x:Static ZPFFonts:IF.Counting_Machine}" Margin="7" />
                  <TextBlock Text="Factures" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     Visibility="{Binding Main.IsDebug, Converter={StaticResource ToVisibility}, Mode=OneWay, Source={StaticResource Locator}}" 
                     CommandParameter="Analytics" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <!--<Image Source="Images/Tiles/Analytics.png" Width="60" />-->
                  <TextBlock Style="{DynamicResource IconFont}" FontSize="48" Text="{x:Static ZPFFonts:IF.Analytics}" Margin="7" />
                  <TextBlock Text="Analytics" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="100" Height="100" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="TOOLS" >
               <StackPanel Orientation="Vertical" Margin="8">
                  <Image Source="Images/Tiles/Tools-02.png" Width="60" />
                  <TextBlock Text="Tools" Foreground="White" HorizontalAlignment="Center" />
               </StackPanel>
            </Controls:Tile>
         </StackPanel>

         <StackPanel Orientation="Vertical" Margin="0,10,0,10" 
            Visibility="{Binding ActualWidth, ConverterParameter=&lt; 110, Converter={StaticResource Size2Visibility}, ElementName=scrollViewer, Mode=OneWay}" >

            <Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="HOME" >

               <StackPanel Orientation="Vertical" Margin="0">
                  <Image Source="Images/Tiles/House-07.png" Width="40" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="GUICH" >

               <StackPanel Orientation="Vertical" Margin="3">
                  <Image Source="Images/Tiles/delivery-packages-on-a-trolley.png" Width="34" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="STOCK" >

               <StackPanel Orientation="Vertical" Margin="0">
                  <Image Source="Images/Tiles/Warehouse-01-WF.png" Width="40" />
               </StackPanel>
            </Controls:Tile>

            <!-- Articles, Sites, Clients -->

            <Controls:Tile Visibility="Collapsed"
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="PRINT" >

               <StackPanel Orientation="Vertical" Margin="0">
                  <Image Source="Images/Tiles/Print - 01.png" Width="40" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
               Foreground="White" Background="{StaticResource Brush_Tile}"
               Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
               Title="" Click="Tile_Click" 
               CommandParameter="IMP_EXP" >

               <StackPanel Orientation="Vertical" Margin="0">
                  <Image Source="Images/Tiles/Data Sync-WF.png" Width="40" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile Visibility="Collapsed"
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     CommandParameter="USERS" >

               <StackPanel Orientation="Vertical" Margin="0">
                  <Image Source="Images/Tiles/User-Group.png" Width="40" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
                     Title="" Click="Tile_Click" 

                     Visibility="{Binding Main.IsFuture, Converter={StaticResource ToVisibility}, Mode=OneWay, Source={StaticResource Locator}}" 
                     CommandParameter="Analytics" >

               <StackPanel Orientation="Vertical" Margin="0">
                  <!--<Image Source="Images/Tiles/Tools-02.png" Width="40" />-->
                  <TextBlock Style = "{DynamicResource IconFont}" FontSize="30" 
                     Text="{x:Static ZPFFonts:IF.Analytics}" Margin="5" />
               </StackPanel>
            </Controls:Tile>

            <Controls:Tile 
                     Foreground="White" Background="{StaticResource Brush_Tile}"
                     Margin="0,5,0,5" Width="40" Height="40" TiltFactor="2"
                     Title="" Click="Tile_Click" 
                     CommandParameter="TOOLS" >

               <StackPanel Orientation="Vertical" Margin="0">
                  <Image Source="Images/Tiles/Tools-02.png" Width="40" />
               </StackPanel>
            </Controls:Tile>
         </StackPanel>

      </StackPanel>
   </ScrollViewer>

*/

      public List<Module> Modules { get; set; } = new List<Module>();

      public List<Module> GetModules()
      {
         Modules.Clear();

         // - - -  - - - 

         Modules.Add(new Module
         {
            Ref = "HOME",
            Name = "Dashboard",
            Title = "Dashboard",
            IconChar = ZPF.Fonts.IF.House,
         });

         Modules.Add(new Module
         {
            Ref = "GUICH",
            Name = "Entrées / sorties",
            Title = "Entrées / sorties",
            IconScale = 0.8,
            IconChar = ZPF.Fonts.IF.Delivery_packages_on_a_trolley,
         });

         Modules.Add(new Module
         {
            Ref = "STOCK",
            Name = "Stock",
            Title = "Stock",
            IconChar = ZPF.Fonts.IF.Warehouse_01_WF,
         });

         Modules.Add(new Module
         {
            Ref = "IMP_EXP",
            Name = "Import / export",
            Title = "Import / export",
            IconChar = ZPF.Fonts.IF.Data_Sync_WF,
         });

         Modules.Add(new Module
         {
            Ref = "Analytics",
            Name = "Analytics",
            Title = "Analytics",
            IconScale = 0.8,
            IconChar = ZPF.Fonts.IF.Analytics,
         });

         Modules.Add(new Module
         {
            Ref = "TOOLS",
            Name = "Tools",
            Title = "Tools",
            IconChar = ZPF.Fonts.IF.Tools_02,
         });

         // - - -  - - - 

         OnPropertyChanged("Modules");
         return Modules;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  
   }
}
