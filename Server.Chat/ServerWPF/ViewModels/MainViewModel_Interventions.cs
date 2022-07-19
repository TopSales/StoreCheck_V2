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

   public List<Intervention_CE> LoadInterventions(DateTime dt, bool MCE, long? FKUser = null, long? FKClient = null, long? FKAction = null, long? FKIntervention = null, bool ActionActif = true, bool Parametres = true)
   {
      string SQL = "";

      SQL = $@"
select 
	Customer.CustomerName as Customer,
	Action.Name as Action, Intervention.FKActionType,
	Intervention.PK, Intervention.FKStore, 
	Store.StoreName, Intervention.StoreType, Store.Zone,
	Intervention.FKIntervener, Intervention.Intervener, 
	Intervention.Input, Intervention.Output, 
    Intervention.Observations, 
	Intervention.IsClosed, Intervention.IsVerified, Intervention.NbKm, 
    
	Intervention.DatePrevue, Intervention.DateBeginIntervention, Intervention.DateEndIntervention, Intervention.DateVerified, 
	Intervention.Parameters, Intervention.Actif, Intervention.Tag, Intervention.UpdatedOn
from 
   Intervention, Action, Customer, Store
where 
	Intervention.FKAction = Action.PK
	and Intervention.FKStore = Store.PK 
	and Action.FKCustomer = Customer.PK 
   and 1=1  
order by 
   Intervention.DateBeginIntervention, Intervention.DatePrevue, Intervention.UpdatedOn 	   
  ";

      if (dt != null && dt != DateTime.MinValue)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and  Intervention.UpdatedOn > {DB_SQL.DateTimeToSQL(DBType.SQLServer, dt.AddHours(-2))} ");
      };

      if (FKUser != null)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and Intervention.FKIntervener = {FKUser} ");
      };

      if (FKClient != null)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and Client.FKUser = {FKClient} ");
      };

      if (FKAction != null)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and Action.PK = {FKAction} ");
      };

      if (FKIntervention != null)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and Intervention.PK = {FKIntervention} ");
      };

      if (ActionActif)
      {
         SQL = SQL.Replace("1=1", $" 1=1 and Action.Actif = 1 ");
      }

      return DB_SQL.Query<Intervention_CE>(MainViewModel.Current.Connection_DB, SQL);
   }

   public List<Intervention_CE> GetInterventions(DateTime dt, bool MCE, long? FKUser = null, long? FKClient = null, long? FKAction = null, long? FKIntervention = null)
   {
      return LoadInterventions(dt, MCE, FKUser, FKClient, FKAction, FKIntervention);
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
