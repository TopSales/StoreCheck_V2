
// Generated by UpdateVersionInfo 1.0.15
//# 1.0.2.1068
//# 28.07.2022

using System;

namespace ZPF
{
   public class VersionInfo
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      static VersionInfo _Current = null;

      public static VersionInfo Current
      {
         get
         {
            if (_Current == null)
            {
               _Current = new VersionInfo();
            };

            return _Current;
         }

         set
         {
            _Current = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public string sVersion { get => "1.0.2.1068"; }
      public Version Version { get => new Version(sVersion); }

      public string BuildOn { get => DateTime.Now.ToString("28.07.2022"); }
      public int RevisionNumber { get => Version.Revision; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
