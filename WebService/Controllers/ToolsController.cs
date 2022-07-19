using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Reflection.Metadata;
using ZPF;
using ZPF.SQL;

namespace StoreCheck
{
    public class ToolsController : Controller
    {
        [AllowCrossSiteJson]
        [Route("~/Tools/now")]
        [HttpGet]
        public string GetNow()
        {
            string lang = "";

            return GetNow(lang);
        }

        [AllowCrossSiteJson]
        [Route("~/Tools/now/{lang}")]
        [HttpGet]
        public string GetNow([FromRoute] string lang = "")
        {
            MainViewModel.Current.Prologue(Request);

            var userLangs = Request.Headers["Accept-Language"].ToString();

            var firstLang = userLangs.Split(',').FirstOrDefault();
            firstLang = firstLang.Split('-').FirstOrDefault().ToLower();

            var defaultLang = string.IsNullOrEmpty(firstLang) ? "en" : firstLang;
            if (("*en*de*fr*nl*").IndexOf(lang.ToLower()) > 0) defaultLang = lang.ToLower();

            switch (defaultLang)
            {
                case "de": return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); break;
                case "nl": return DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); break;
                case "fr": return DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); break;

                default:
                case "en": return DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"); break;
            };
        }

        [AllowCrossSiteJson]
        [Route("~/Tools/TestDB")]
        [HttpGet]
        public string GetTestDB()
        {
            string Result = "";

            MainViewModel.Current.Prologue(Request);

            try
            {
                if (!string.IsNullOrEmpty(MainViewModel.Current.Connection_DB.LastError))
                {
                    return MainViewModel.Current.Connection_DB.LastError;
                };
            }
            catch (Exception ex)
            {
                return "(1) " + ex.Message;
            };

            if (MainViewModel.Current.Connection_DB.DbConnection.State != System.Data.ConnectionState.Open)
            {
                MainViewModel.Current.Connection_DB.CheckConnection();
            };

            Result = DB_SQL.QuickQuery("SELECT @@VERSION ;");

            if (!string.IsNullOrEmpty(MainViewModel.Current.Connection_DB.LastError))
            {
                return MainViewModel.Current.Connection_DB.LastError;
            };

            return Result;
        }

        [AllowCrossSiteJson]
        [Route("~/Tools/Config")]
        [HttpGet]
        public ContentResult GetConfig([FromHeader] string authorization)
        {
            MainViewModel.Current.Prologue(Request);

            //if (!string.IsNullOrEmpty(authorization) && MainViewModel.Current.CheckAuthorization(authorization))

#if DEBUG
            {
                TStrings html = new TStrings();

                html.Add("Connection_DB", MainViewModel.Current.Connection_DB.DbConnection.Database);
                html.Add("Connection_AT", MainViewModel.Current.Connection_AT.DbConnection.Database);
                html.Add("Connection_DOC", MainViewModel.Current.Connection_DOC.DbConnection.Database);

                var sHtml = "<HTML><body><h1>DB Config</h1>" + html.HTMLTable() + "</body></HTML>";

                return Content(sHtml, "text/html");
            }
#else
            {
                return null;
            };
#endif
        }
    }
}
