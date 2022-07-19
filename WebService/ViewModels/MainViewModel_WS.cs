using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;
using ZPF.SQL;

public partial class MainViewModel : BaseViewModel
{
    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

    public string Prologue(HttpRequest request, string json = null)
    {
        if (OpenDB(request))
        {
            Log.Write(new AuditTrail
            {
                Level = ErrorLevel.Log,
                Application = "wsSC",
                Tag = "ws",
                IsBusiness = false,
                Message = $"Prologue {request.Path}",
                DataIn = $"IsHttps : {request.IsHttps}"
                        + (string.IsNullOrEmpty(json) ? "" : Environment.NewLine + json),
                TerminalIP = request.HttpContext.Connection.RemoteIpAddress?.ToString()
            });
        };

        return json;
    }

    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

    public bool OpenDB(HttpRequest request)
    {
        // if (true || request.Host.ToString().ToUpper().Contains(".TECH") || request.Host.ToString().ToUpper().Contains("LOCALHOST"))

#if DEBUG
        // - - - DEV - - -
        if (Connection_DB == null || !Connection_DB.CheckConnection())
        {
            //Connection_DB = SmarterASPViewModel.Current.OpenStoreCheckDev();
            Connection_DB = SmarterASPViewModel.Current.OpenStoreCheckMaquette();
        };

        if (Connection_AT == null || !Connection_AT.CheckConnection())
        {
            Connection_AT = SmarterASPViewModel.Current.OpenAuditTrailDev();
            Connection_AT = Connection_DB;

            // - - -  - - - 

            #region Create table & co 

            string SQL = "";

            switch (Connection_AT.DBType)
            {
                case DBType.SQLServer: SQL = AuditTrail.PostScript_MSSQL; break;

                case DBType.SQLite: SQL = AuditTrail.PostScript_SQLite; break;

                case DBType.PostgreSQL: SQL = AuditTrail.PostScript_PGSQL; break;

                case DBType.MySQL: SQL = AuditTrail.PostScript_MySQL; break;
            };

            // - - -  - - - 

            DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail), SQL, "");
            DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail_App), SQL, "");

            #endregion
        };

        if (Connection_DOC == null || !Connection_DOC.CheckConnection())
        {
            Connection_DOC = SmarterASPViewModel.Current.OpenBaseDocDev();
        };
#else
      // - - - PROD - - -
      if (Connection_DB == null || !Connection_DB.CheckConnection())
      {
         Connection_DB = SmarterASPViewModel.Current.OpenStoreCheckProd();
      };

                if (Connection_AT == null || !Connection_AT.CheckConnection())
                {
                    Connection_AT = SmarterASPViewModel.Current.OpenAuditTrailProd();

                    // - - -  - - - 

        #region Create table & co 

                    string SQL = "";

                    switch (Connection_AT.DBType)
                    {
                        case DBType.SQLServer: SQL = AuditTrail.PostScript_MSSQL; break;

                        case DBType.SQLite: SQL = AuditTrail.PostScript_SQLite; break;

                        case DBType.PostgreSQL: SQL = AuditTrail.PostScript_PGSQL; break;

                        case DBType.MySQL: SQL = AuditTrail.PostScript_MySQL; break;
                    };

                    // - - -  - - - 

                    DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail), SQL, "");
                    DB_SQL.CreateTable(Connection_AT, typeof(AuditTrail_App), SQL, "");

        #endregion

                    // - - -  - - - 

                    AuditTrailViewModel.Current.Init(new DBAuditTrailWriter(Connection_AT));
                    AuditTrailViewModel.Current.Application = "SCAdmin";
                };

                if (Connection_DOC == null || !Connection_DOC.CheckConnection())
                {
                    Connection_DOC = SmarterASPViewModel.Current.OpenBaseDocProd();

                    // - - -  - - - 

        #region Create table & co 

                    string SQL = "";

                    switch (Connection_DOC.DBType)
                    {
                        case DBType.SQLServer: SQL = Document.SQLPostCreate_MSSQL; break;

                        case DBType.SQLite: SQL = Document.SQLPostCreate_SQLite; break;

                        case DBType.PostgreSQL: SQL = Document.SQLPostCreate_PGSQL; break;

                        case DBType.MySQL: SQL = Document.SQLPostCreate_MySQL; break;
                    };

                    // - - -  - - - 

                    DB_SQL.CreateTable(Connection_DOC, typeof(Document), SQL, "");
                    DB_SQL.CreateTrigger_OnUpdated(Connection_DOC, "Document", "UpdatedOn");
                    DB_SQL.CreateTrigger_OnCreated(Connection_DOC, "Document", "CreatedOn");

        #endregion

            };
#endif

        //Log.Write(new AuditTrail
        //{
        //    Level = ErrorLevel.Log,
        //    Tag = "API",
        //    Message = request.Path,
        //    TerminalID = request.HttpContext.Connection.RemoteIpAddress.ToString(),
        //    TerminalIP = request.HttpContext.Connection.RemoteIpAddress.ToString(),
        //});

        return true;
    }

    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

    internal bool CheckAuthorization(string authorization)
    {
        bool Result = false;

        try
        {
            string authenticationToken = authorization.Replace("Basic ", "");
            string decodedAuthenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationToken));
            string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
            string userName = usernamePasswordArray[0];
            string password = usernamePasswordArray[1];

            Result = (userName == "StoreCheck" && password == "ZPF");

            if (!Result)
            {
                Log.Write(new AuditTrail { Level = ErrorLevel.Info, Application = "ws", Tag = "ws", IsBusiness = false, Message = "CheckAuthorization failed." });
            };
        }
        catch { };

        return Result;
    }

    internal bool CheckToken(string Token, bool IsLogin = false)
    {
        bool Result = true;

        Result = Result && !string.IsNullOrEmpty(Token);

        //if (IsLogin)
        //{
        //   var t = wsToken.GetData(Token);
        //   Result = Result && t.PK == wsToken.NewToken().PK;
        //   Result = Result && t.USR == wsToken.NewToken().USR;
        //}
        //else
        //{
        //   var t = wsToken.GetData(Token);

        //   Result = Result && DB_SQL.QuickQueryInt($" select count(*) from UserAcount where PK={t.PK} and Login='{t.USR}'") == 1;
        //};

        return Result;
    }

    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -
}
