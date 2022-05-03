using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NetworkSpeedMonitor
{
   public static class NetworkHelper
   {
      public static async Task<string> CheckInternetSpeed()
      {
         DateTime dt1 = DateTime.Now;
         string internetSpeed = "";

         try
         {
            var client = new HttpClient();
            byte[] data = await client.GetByteArrayAsync("http://www.google.com");
            DateTime dt2 = DateTime.Now;

            internetSpeed = $"Connection Speed: (kb/s) {Math.Round((data.Length / 1024) / (dt2 - dt1).TotalSeconds, 2)}";
         }
         catch (Exception ex)
         {
            internetSpeed = $"Connection Speed: Exception {ex.Message}";
         };

         return internetSpeed;
      }

      public static async Task<string> CheckInternet()
      {
         if (Connectivity.NetworkAccess == NetworkAccess.Internet)
         {
            return await CheckInternetSpeed();
         }
         else
         {
            return "KO";
         };
      }

   }
}
