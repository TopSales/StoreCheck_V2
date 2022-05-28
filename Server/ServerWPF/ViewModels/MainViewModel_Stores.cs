using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;
using ZPF.SQL;

public partial class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<Store_CE> LoadStores(DateTime dt, long? FKStore = null)
   {
      string SQL = "";

      SQL = @"
select 
   Chain.Name as Chain, SubChain.Name as SubChain, FKSubChain,
   Store.PK, Store.Ref, Store.StoreName, 
   Store.Contact, Store.Address, Store.AddressNo, Store.PC, Store.City, Store.Country, 
   Store.Phone, Store.Mail, Store.URL, 
   Store.Observations, 
   Store.Zone, Store.Latitude, Store.Longitude, 
   Store.Actif, Store.Tag, Store.UpdatedOn
from 
  	Store, SubChain, Chain
where 
   Store.FKSubChain = SubChain.PK
   and SubChain.FKChain = Chain.PK
   and 1=1  
";

      if (dt != null && dt != DateTime.MinValue)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and  Store.UpdatedOn > {DB_SQL.DateTimeToSQL(DBType.SQLServer, dt.AddHours(-2))} ");
      };

      if (FKStore != null)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and Store.PK = {FKStore} ");
      };

      return DB_SQL.Query<Store_CE>(MainViewModel.Current.Connection_DB, SQL);
   }

   public List<Store_CE> GetStores(DateTime dt, long? FKStore = null)
   {
      if (dt > DateTime.MinValue)
      {
         dt = dt.AddHours(-2);
         dt = dt.AddMinutes(-5);
      };

      return LoadStores(dt, FKStore);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
