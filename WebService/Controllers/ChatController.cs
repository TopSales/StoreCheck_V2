using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
using ZPF;
using ZPF.Chat;
using ZPF.SQL;

namespace StoreCheck
{
    public class ChatController : Controller
    {
        [AllowCrossSiteJson]
        [Route("~/SendDataToServer")]
        [HttpPut]
        public ChatData SendDataToServer([FromHeader] string authorization, [FromBody] ChatData chatData )
        {
            if (!string.IsNullOrEmpty(authorization) && MainViewModel.Current.CheckAuthorization(authorization))
            {
                if (chatData != null)
                {
                    var r = ServerViewModel.Current.ChatServer_OnDataEvent(null, null, chatData);
                    return r;
                };

                return null;
            }
            else
            {
                return null;
            };
        }
    }
}
