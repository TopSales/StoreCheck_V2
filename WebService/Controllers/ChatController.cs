using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Xml.Linq;
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
                if (Request.Form.Files.Count == 1)
                {
                    var formFile = Request.Form.Files.First();

                    if (formFile.Length > 0)
                    {
                        string json = "";

                        using (var inputStream = formFile.OpenReadStream())
                        {
                            // stream to byte array
                            byte[] array = new byte[inputStream.Length];
                            inputStream.Seek(0, SeekOrigin.Begin);
                            inputStream.Read(array, 0, array.Length);

                            json = Encoding.UTF8.GetString(array);
                        };

                        if( !string.IsNullOrEmpty(json))
                        {

                        };
                    };
                };

                return "holla";
            }
            else
            {
                return null;
            };
        }
    }
}
