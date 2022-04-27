using System;
using System.Diagnostics;
using System.IO;
using ZPF.SQL;

#if WPF
#endif

namespace ZPF
{
   public class BaseDocViewModel : BaseViewModel
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      static BaseDocViewModel _Instance = null;

      public static BaseDocViewModel Current
      {
         get
         {
            if (_Instance == null)
            {
               _Instance = new BaseDocViewModel();
            };

            return _Instance;
         }

         set
         {
            _Instance = value;
         }
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public BaseDocViewModel()
      {
         _Instance = this;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public bool UploadDoc(string FilePath, Document.InternalDocumentTypes DocType, string refType, long extRef, long FKDocumentType = 0)
      {
         if (DocType == Document.InternalDocumentTypes.FilePath)
         {
            return false;
         };

         FileInfo fileInfo = new FileInfo(FilePath);

         if (FKDocumentType == 0)
         {
            switch (DocType)
            {
               case Document.InternalDocumentTypes.image:
                  FKDocumentType = 3;
                  break;

               case Document.InternalDocumentTypes.signature:
                  FKDocumentType = 4;
                  break;
            };
         };

         try
         {
            string SQL = $"insert Document ( {(DocType == Document.InternalDocumentTypes.doc ? "Document" : "Picture")}, InternalDocType, TypeFK, FK, FileName, FileExt, FileSize, FileDate, FKDocumentType  )"
               + $" values ( CONVERT(varbinary(max), @DocData) , {(int)DocType}, '{refType}', {extRef}, '{DB_SQL.StringToSQL(fileInfo.Name)}', '{DB_SQL.StringToSQL(fileInfo.Extension)}', {fileInfo.Length}, { DB_SQL.DateTimeToSQL(fileInfo.LastWriteTimeUtc)}, {FKDocumentType} )";

            return DB_SQL.UploadFile(DB_SQL._ViewModel, FilePath, SQL);
         }
         catch (Exception ex)
         {
            Debug.WriteLine(ex.Message);
         };

         return false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public bool DownloadDoc(string FilePath, Document.InternalDocumentTypes DocType, long PK)
      {
         if (DocType == Document.InternalDocumentTypes.FilePath)
         {
            return false;
         };

         FileInfo fileInfo = new FileInfo(FilePath);

         string SQL = $"select {(DocType == Document.InternalDocumentTypes.doc ? "Document" : "Picture")} from Document where PK={PK}";

         return DB_SQL.DownloadFile(DB_SQL._ViewModel, FilePath, SQL);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public bool IsImage(string FilePath)
      {
         var ext = System.IO.Path.GetExtension(FilePath).ToLower();

         return (".gif.jpg.jpeg.png.bmp.tiff.tif.ico.").Contains(ext);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      //public void Init()
      //{
      //   SaveDocToDB = true;
      //   SaveDocToDisk = false;

      //   //if (IsInDesignMode())
      //   //{
      //   //   // Code runs in Blend --> create design time data.
      //   //}
      //   //else
      //   {
      //      // Code runs "for real"

      //      using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView("SELECT * FROM sXc_DocumentType order by SortOrder"))
      //      {
      //         var list = new List<DocumentType>();

      //         foreach (DataRow row in dataTable.Rows)
      //         {
      //            list.Add(DB_SQL.DataRowToObj<DocumentType>(row));
      //         };

      //         DocumentTypes = list;

      //         OnPropertyChanged("DocumentTypes");
      //      };

      //      LoadData();
      //   }
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //// public ObservableCollection<Document> Documents = new ObservableCollection<Document>();

      //public IEnumerable<Document> Documents { get; private set; }
      //public ObservableCollection<Document> TimeOutDocuments = new ObservableCollection<Document>();
      //public ObservableCollection<Document> DeletedDocuments = new ObservableCollection<Document>();
      //public ObservableCollection<Document> DocumentsAll = new ObservableCollection<Document>();
      //public IEnumerable<DocumentType> DocumentTypes { get; private set; }

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //bool SaveDocToDisk { get; set; }
      //bool SaveDocToDB { get; set; }

      //bool _ReadOnly = true;

      //public bool ReadOnly
      //{
      //   get { return _ReadOnly; }
      //   set
      //   {
      //      _ReadOnly = value;
      //      OnPropertyChanged();
      //   }
      //}

      //bool _IsBusy = false;
      //public bool IsBusy
      //{
      //   get { return _IsBusy; }
      //   set
      //   {
      //      _IsBusy = value;
      //      OnPropertyChanged();
      //   }
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //Document _Document = null;

      //public Document Document
      //{
      //   get { return _Document; }
      //   set
      //   {
      //      if (_Document != value)
      //      {
      //         _Document = value;
      //         ReadOnly = true;
      //         OnPropertyChanged();
      //      };
      //   }
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //int _ExternalRef = -1;

      //public int ExternalRef
      //{
      //   get { return _ExternalRef; }
      //   set
      //   {
      //      if (_ExternalRef != value)
      //      {
      //         _ExternalRef = value;
      //         LoadData();
      //      };
      //   }
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //public void LoadData(bool Caisse = false, bool UseDispatcher = false)
      //{
      //   AuditTrail.Write("", string.Format("BaseDocViewModel {0} {1}", Caisse, UseDispatcher));

      //   this.Document = null;

      //   if (UseDispatcher)
      //   {
      //      Application.Current.Dispatcher.Invoke(new Action(() =>
      //      {
      //         using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView(string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and Deleted = 0 order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //         {
      //            var list = new List<Document>();

      //            foreach (DataRow row in dataTable.Rows)
      //            {
      //               list.Add(DB_SQL.DataRowToObj<Document>(row));
      //            };

      //            Documents = list;

      //            OnPropertyChanged("Documents");
      //         };
      //      }));
      //   }
      //   else
      //   {
      //      using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView(string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and Deleted = 0 order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //      {
      //         var list = new List<Document>();

      //         if (dataTable != null)
      //         {
      //            foreach (DataRow row in dataTable.Rows)
      //            {
      //               list.Add(DB_SQL.DataRowToObj<Document>(row));
      //            };
      //         };

      //         Documents = list;

      //         OnPropertyChanged("Documents");
      //      };

      //      //using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView( string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and Deleted = 0 order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //      //{
      //      //   Documents.Clear();

      //      //   foreach (DataRow row in dataTable.Rows)
      //      //   {
      //      //      Documents.Add(DB_SQL.DataRowToObj<Document>(row));
      //      //   };

      //      //   OnPropertyChanged("Documents");
      //      //};
      //   };


      //   //
      //   // - - -  - - - 

      //   string sIDState = this.IDState;

      //   switch (sIDState)
      //   {
      //      case "OK": IDStateColor = System.Windows.Media.Brushes.Green; break;
      //      case "": IDStateColor = System.Windows.Media.Brushes.Orange; sIDState = "OO"; break;
      //      case "OO": IDStateColor = System.Windows.Media.Brushes.Orange; break;
      //      case "KO": IDStateColor = System.Windows.Media.Brushes.Red; break;
      //   };

      //   //
      //   // - - -  - - - 

      //   if (Caisse)
      //   {
      //      if (sIDState != "")
      //      {
      //         MessageHelper.SendString("BaseDoc=" + sIDState);
      //      };

      //      return;
      //   };

      //   using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView(string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and Deleted = 0 order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //   {
      //      var list = new List<Document>();

      //      foreach (DataRow row in dataTable.Rows)
      //      {
      //         list.Add(DB_SQL.DataRowToObj<Document>(row));
      //      };

      //      Documents = list;

      //      OnPropertyChanged("Documents");
      //   };

      //   //using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView( string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and Deleted = 0 order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //   //{
      //   //   Documents.Clear();

      //   //   foreach (DataRow row in dataTable.Rows)
      //   //   {
      //   //      Documents.Add(DB_SQL.DataRowToObj<Document>(row));
      //   //   };

      //   //   OnPropertyChanged("Documents");
      //   //};

      //   using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView(string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and Deleted = 0 and Now()>DateEnd order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //   {
      //      TimeOutDocuments.Clear();

      //      foreach (DataRow row in dataTable.Rows)
      //      {
      //         TimeOutDocuments.Add(DB_SQL.DataRowToObj<Document>(row));
      //      };

      //      OnPropertyChanged("TimeOutDocuments");
      //   };

      //   using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView(string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and Deleted = 1 order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //   {
      //      DeletedDocuments.Clear();

      //      foreach (DataRow row in dataTable.Rows)
      //      {
      //         DeletedDocuments.Add(DB_SQL.DataRowToObj<Document>(row));
      //      };

      //      OnPropertyChanged("DeletedDocuments");
      //   };

      //   using (DataTable dataTable = (DataTable)DB_SQL.QuickQueryView(string.Format("SELECT sXc_Document.*, sXc_DocumentType.Ref as DocumentType FROM sXc_Document, sXc_DocumentType where ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK order by TimeStampUpdate DESC LIMIT 0, 100;", ExternalRef)))
      //   {
      //      DocumentsAll.Clear();

      //      foreach (DataRow row in dataTable.Rows)
      //      {
      //         DocumentsAll.Add(DB_SQL.DataRowToObj<Document>(row));
      //      };

      //      OnPropertyChanged("DocumentsAll");
      //   };
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //public bool databaseFilePut(string Title, string FileName, Int64 FKDocType = 1, byte[] Data = null)
      //{
      //   Log.Write("*", string.Format("Title={0}, FileName={1}", Title, FileName));

      //   bool Result = true;
      //   IsBusy = true;

      //   Task[] tasks = {
      //   System.Threading.Tasks.Task.Run (async () =>
      //   {
      //      byte[] file;
      //      using (var stream = new FileStream(FileName, FileMode.Open, FileAccess.Read))
      //      {
      //         using (var reader = new BinaryReader(stream))
      //         {
      //            file = reader.ReadBytes((int)stream.Length);
      //         }
      //      }

      //      // Set insert query
      //      string SQL = string.Format("insert sXc_Document ( Document, DocumentRef, TimeStampCreation, TimeStampUpdate, ExternalRef, FileName, FileExt, FileSize, Title, FKDocumentType, DumpEID ) "
      //                                    + " values (  @DocData, '', Now(), Now(), {0}, '{1}', '{2}', {3}, '{4}', {5}, @Dump )",
      //                                    ExternalRef, DB_SQL.StringToSQL(DB_SQL.Connection, Path.GetFileName(FileName)), DB_SQL.StringToSQL(DB_SQL.Connection,Path.GetExtension(FileName)), new FileInfo(FileName).Length, DB_SQL.StringToSQL(DB_SQL.Connection,Title), FKDocType);

      //      // Initialize SqlCommand object for insert.
      //      DbCommand cmd = DB_SQL._ViewModel.NewCommand(SQL);

      //      // We are passing the image byte data as SQL parameters.
      //      cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@DocData", (object)file));
      //      cmd.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@Dump", (object)Data));

      //      try
      //      {
      //         // Open connection and execute insert query.
      //         Result = (cmd.ExecuteNonQuery() == 1);
      //         IsBusy = false;
      //      }
      //      catch (Exception ex)
      //      {
      //         IsBusy = false;
      //         MessageBox.Show(ex.Message);
      //         AuditTrail.Write("***", ex.ToString());
      //         AuditTrail.Write("***", SQL );
      //         Result = false;
      //      };

      //   })};

      //   System.Threading.Tasks.Task.WaitAll(tasks);

      //   return Result;
      //}

      //public void databaseFileRead(string FileName, bool Picture = false)
      //{
      //   // http://stackoverflow.com/questions/2579373/saving-any-file-to-in-the-database-just-convert-it-to-a-byte-array

      //   // using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))

      //   string SQL = "SELECT Document FROM sXc_Document WHERE PK = @varID";
      //   if (Picture)
      //   {
      //      SQL = "SELECT Picture FROM sXc_Document WHERE PK = @varID";
      //   };

      //   using (var sqlQuery = DB_SQL._ViewModel.NewCommand(SQL))
      //   {
      //      sqlQuery.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@varID", _Document.PK));
      //      //sqlQuery.Parameters["varID"].Value = _Document.PK;

      //      using (var sqlQueryResult = sqlQuery.ExecuteReader())
      //      {
      //         if (sqlQueryResult != null)
      //         {
      //            sqlQueryResult.Read();
      //            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
      //            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);

      //            using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
      //            {
      //               fs.Write(blob, 0, blob.Length);
      //            }
      //         }
      //      }
      //   }
      //}

      //public Stream databaseFileReadStream(bool Picture = false)
      //{
      //   // http://stackoverflow.com/questions/2579373/saving-any-file-to-in-the-database-just-convert-it-to-a-byte-array

      //   // using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))

      //   string SQL = "SELECT Document FROM sXc_Document WHERE PK = @varID";
      //   if (Picture)
      //   {
      //      SQL = "SELECT Picture FROM sXc_Document WHERE PK = @varID";
      //   };

      //   using (var sqlQuery = DB_SQL._ViewModel.NewCommand(SQL))
      //   {
      //      sqlQuery.Parameters.Add(new MySql.Data.MySqlClient.MySqlParameter("@varID", _Document.PK));
      //      //sqlQuery.Parameters["varID"].Value = _Document.PK;

      //      using (var sqlQueryResult = sqlQuery.ExecuteReader())
      //      {
      //         if (sqlQueryResult != null)
      //         {
      //            sqlQueryResult.Read();
      //            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
      //            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);

      //            return new MemoryStream(blob);

      //            //using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write))
      //            //{
      //            //   fs.Write(blob, 0, blob.Length);
      //            //}
      //         }
      //      }
      //   }

      //   return null;
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //public void UpdateDoc()
      //{
      //   Document.TimeStampUpdate = DateTime.Now;

      //   Document.DocumentType = DB_SQL.QuickQuery("select Ref from sXc_DocumentType where PK = " + Document.FKDocumentType.ToString());
      //   DB_SQL.Update("sXc_Document", Document);
      //}

      //public void DeleteDoc()
      //{
      //   DB_SQL.QuickQuery("update sXc_Document set Deleted=1, TimeStampUpdate=Now() where PK=" + Document.PK);

      //   Documents = Documents.Except(Document.ToEnumerable());
      //   OnPropertyChanged("Documents");

      //   Document = null;
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //string IDState
      //{
      //   get
      //   {
      //      string SQL = String.Format(@"
      //            select * from 
      //            (
      //            SELECT 'OK' as State 
      //            FROM sXc_Document, sXc_DocumentType 
      //            where  ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and sXc_DocumentType.IsID and Deleted = 0 and DateEnd > Now()
      //            union
      //            SELECT 'OO' as State 
      //            FROM sXc_Document, sXc_DocumentType 
      //            where  ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and sXc_DocumentType.IsID and Deleted = 0 and ( DateEnd is null or DATE_ADD(DateEnd,INTERVAL 365 DAY) > Now())
      //            union
      //            SELECT 'KO' as State 
      //            FROM sXc_Document, sXc_DocumentType 
      //            where  ExternalRef={0} and sXc_Document.FKDocumentType =  sXc_DocumentType.PK and sXc_DocumentType.IsID and Deleted = 0 and DateEnd < (DATE_ADD(Now(),INTERVAL -365 DAY)) 
      //            ) T
      //            limit 0,1 ", ExternalRef);

      //      return DB_SQL.QuickQuery(SQL);
      //   }
      //}

      //// - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      //System.Windows.Media.SolidColorBrush _IDStateColor = System.Windows.Media.Brushes.LightGray;

      //public System.Windows.Media.SolidColorBrush IDStateColor
      //{
      //   get { return _IDStateColor; }
      //   set
      //   {
      //      if (_IDStateColor != value)
      //      {
      //         _IDStateColor = value;

      //         OnPropertyChanged("IDStateColor");
      //      };
      //   }
      //}

      //public void SearchRefEGIC(string Ref, bool Caisse = false)
      //{
      //   var vm = VMLocator.BaseDoc;

      //   int ExternalRef = int.Parse(DB_SQL.QuickQuery(string.Format("select PK from sXc_Client where Code='{0}'", Ref.ToUpper())));
      //   vm.ExternalRef = ExternalRef;

      //   LoadData(Caisse);
      //}

      //internal void SetExternalRef(int value)
      //{
      //   _ExternalRef = value;
      //}

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -   
   }
}
