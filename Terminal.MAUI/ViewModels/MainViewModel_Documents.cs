using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using ZPF;
using ZPF.AT;

public partial class MainViewModel : BaseViewModel
{
   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

   public List<Document_CE> Documents { get; private set; } = new List<Document_CE>();

   public Document_CE SelectedDocument { get => _SelectedDocument; set => SetField(ref _SelectedDocument, value); }
   Document_CE _SelectedDocument = null;

   internal void SetLocalDocuments(List<Document_CE> list)
   {
      Documents = list;

      OnPropertyChanged("Documents");
   }

   internal void SetDocuments(List<Document_CE> list)
   {
      // - - - remove previously deleted items - - - 

      //{
      //   var l = Documents.Where(x => x.Actif == false && x.SyncOn != DateTime.MinValue && (DateTime.Now - x.UpdatedOn).TotalDays > 10).ToList();

      //   foreach (var doc in l)
      //   {
      //      //ToDo: delete photo "file"
      //      if (System.IO.File.Exists(doc.FullPath))
      //      {
      //         System.IO.File.Delete(doc.FullPath);
      //      };

      //      Documents.Remove(doc);
      //   };
      //};

      // - - - sync unsynced files - - - 

      //{
      //   var l = Documents.Where(x => x.SyncOn == DateTime.MinValue).ToList();

      //   foreach (var doc in l)
      //   {
      //      if (doc.Actif == false)
      //      {
      //         //ToDo: delete photo "file" ???
      //      }
      //      else
      //      {
      //         DoIt.OnBackground(async () =>
      //         {
      //            // - - - sync photo "file" - - - 

      //            if (await MainViewModel.Current.UploadDoc(doc.FullPath, global::Document.InternalDocumentTypes.Signature, doc.ExtType, doc.ExtRef, doc.Title, doc.GUID))
      //            {
      //               doc.SyncOn = DateTime.Now;
      //               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);
      //            }
      //            else
      //            {
      //               // Oups 
      //            };
      //         });
      //      };
      //   };
      //};

      //// - - -  - - - 

      //foreach (var doc in list)
      //{
      //   var d = Documents.Where(x => x.GUID == doc.GUID).FirstOrDefault();

      //   if (d != null)
      //   {
      //      //ToDo: Update
      //      //Documents.Remove(d);
      //      //Documents.Add(doc);
      //   }
      //   else
      //   {
      //      Documents.Add(doc);

      //      // ToDo: DownloadDoc
      //      //if (doc.InternalDocType != Document_CE.InternalDocumentTypes.FilePath)
      //      //{
      //      //   DoIt.OnBackground(async () =>
      //      //   {
      //      //      // - - - sync photo "file" - - - 

      //      //      //      if (await MainViewModel.Current.DownloadDoc(doc))
      //      //      //      {
      //      doc.SyncOn = DateTime.Now;
      //      MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);
      //      //      //      }
      //      //      //      else
      //      //      //      {
      //      //      //         // Oups 
      //      //      //         Documents.Remove(doc);
      //      //      //      };
      //      //   });
      //      //};
      //   };
      //};

      // - - -  - - - 

      {
         foreach (var doc in list)
         {
            var d = Documents.Where(x => x.GUID == doc.GUID).FirstOrDefault();

            if (d != null)
            {
               if (doc.UpdatedOn > d.UpdatedOn)
               {
                  Documents.Remove(d);
                  doc.SyncOn = DateTime.Now;
                  Documents.Add(doc);
               };
            }
            else
            {
               doc.SyncOn = DateTime.Now;
               Documents.Add(doc);
            };
         };
      };

      // - - -  - - - 

      OnPropertyChanged("Documents");
   }

   // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}
