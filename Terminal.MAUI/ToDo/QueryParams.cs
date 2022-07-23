using System;

namespace ZPF.Chat
{
   public class QueryParams
   {
      public long FKUser { get; set; } = -1;
      public DateTime Begin { get; set; } = DateTime.MinValue;
      public DateTime End { get; set; } = DateTime.MaxValue;
   }
}
