using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using ZPF;
using ZPF.SQL;

namespace StoreCheck
{
    public class ChatController : Controller
    {
        [AllowCrossSiteJson]
        [Route("~/SendDataToServer")]
        [HttpPost]
        public string SendDataToServer([FromHeader] string authorization)
        {
            if (!string.IsNullOrEmpty(authorization) && MainViewModel.Current.CheckAuthorization(authorization))
            {
                //return wsViewModel.Current.PostContact(json);

                return "holla";
            }
            else
            {
                return null;
            };
        }
    }
}
