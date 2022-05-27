using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZPF;
using ZPF.AT;

public class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   static MainViewModel _Current = null;

   public static MainViewModel Current
   {
      get
      {
         if (_Current == null)
         {
            _Current = new MainViewModel();
         };

         return _Current;
      }

      set
      {
         _Current = value;
      }
   }

   public Config Config { get; private set; } = new Config();

   public string DeviceID
   {
      get => _DeviceID;
      set { SetField(ref _DeviceID, value); AuditTrailViewModel.Current.TerminalID = _DeviceID; }
   }
   string _DeviceID = "";

   public string EntryMsg { get => _EntryMsg; set => SetField(ref _EntryMsg, value); }
   string _EntryMsg = "";

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public string DataFolder { get; private set; }

   public MainViewModel()
   {
      DataFolder = FileSystem.AppDataDirectory;

      System.Diagnostics.Debug.WriteLine($"Data: {DataFolder}");

      AuditTrailViewModel.Current.Init(new JSONAuditTrailWriter(DataFolder + "AuditTrail.json", JSONAuditTrailWriter.FileTypes.PartialJSON));
      AuditTrailViewModel.Current.Application = "SC_Term";
      AuditTrailViewModel.Current.Clean();
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -


   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
