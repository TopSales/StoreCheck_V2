using Microsoft.AspNetCore.Mvc;
using ZPF.Chat;

namespace StoreCheck
{
    public class ChatController : Controller
    {
        [AllowCrossSiteJson]
        [Route("~/SendDataToServer")]
        [HttpPut]
        public ChatData SendDataToServer([FromHeader] string authorization, [FromBody] ChatData chatData )
        {
            MainViewModel.Current.Prologue(Request);

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
