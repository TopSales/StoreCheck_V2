using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using FFImageLoading;
using Plugin.Media.Abstractions;
using StoreCheck.Pages;
using Xamarin.Forms;
using ZPF.AT;
using ZPF.XF;
using ZPF.XF.Compos;

namespace ZPF
{
   class PhotosPage : Page_Base
   {
      public AttachmentPanel att = null;
      PhotoView pv = null;

      // - - -  - - - 

      public PhotosPage(string PhotoTitle, string SubTitle)
      {
         Title = "photos";

         // - - -  - - - 

         var g = new Grid();

         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
         g.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

         //g.Children.Add(new MagasinView(), 0, 0);      // entête info 
         g.Children.Add(new InterventionView(), 0, 0);   // entête info 

         pv = new PhotoView() { Text = (string.IsNullOrEmpty(PhotoTitle) ? "l'intervention" : SubTitle) };
         g.Children.Add(pv, 0, 1);   // entête info 

         att = new AttachmentPanel(this);
         // att.FileName = string.Format("SIT.{0}.jpg", DB_SQL.QuickQueryInt("select PK from Site limit 1"));

         att.OnLoadData += LoadData;
         att.OnAddData += OnAddData;
         att.OnDelData += OnDelData;
         //att.OnUpdateData += OnUpdateData;
         att.OnProcessPhoto += OnProcessPhoto;

         att.Document = new Document_CE
         {
            Title = (string.IsNullOrEmpty(PhotoTitle) ? "Intervention" : PhotoTitle),
         };

         g.Children.Add(att, 0, 2);

         SetMainContent(g);

         // - - -  - - -       

         SetAppBarContent();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      protected override void OnAppearing()
      {
         base.OnAppearing();

         LoadData();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public string SubTitle { get => pv.Text; set => pv.Text = value; }
      public string Comment { get; internal set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      private async void OnAddData(IDocument document, string FileName)
      {
         var doc = new Document_CE
         {
            ExtType = "Intervention",
            ExtRef = (string.IsNullOrEmpty(document.ExtRef) ? MainViewModel.Current.SelectedIntervention.PK.ToString() : document.ExtRef),
            Title = document.Title,
            Comment = (string.IsNullOrEmpty(Comment) ? document.Title : Comment),
            Keywords = "Intervention," + document.Title,
            FullPath = FileName,
            FileExt = System.IO.Path.GetExtension(FileName),
            FileDate = DateTime.Now,
            InternalDocType = Document.InternalDocumentTypes.image,
         };

         MainViewModel.Current.Documents.Add(doc);
         MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);

         try
         {
            if (
               MainViewModel.Current.IsInternetAccessAvailable &&
               await MainViewModel.Current.UploadDoc(doc.FullPath, global::Document.InternalDocumentTypes.image, doc.ExtType, doc.ExtRef, doc.Title, doc.Comment, doc.GUID))
            {
               doc.SyncOn = DateTime.Now;
               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);
            }
            else
            {
               doc.SyncOn = DateTime.MinValue;
               MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);

               await DisplayAlert("Oups ...", "??? Upload ???", "ok");
            };
         }
         catch { };
      }

      private bool OnDelData(TAttachment Attachment)
      {
         try
         {
            MainViewModel.Current.Documents.Remove(MainViewModel.Current.Documents.Where(x => x.ExtRef == Attachment.ExternalRef && x.FileName.StartsWith(Attachment.FileName)).FirstOrDefault());
            MainViewModel.Current.SaveLocalDB(MainViewModel.DBRange.Documents);

            try
            {
               System.IO.File.Delete(Attachment.FullPath);
            }
            catch { };
         }
         catch
         {
            return false;
         };

         return true;
      }

      private bool OnUpdateData(TAttachment Attachment)
      {
         //Document d = DB_SQL.Query<Document>(string.Format("select * from Document where ExtType='Site' and RefExt={0} and FullPath like '%{1}'",
         //                           SiteViewModel.Instance.SelectedSite.PK.ToString(), DB_SQL.StringToSQL(System.IO.Path.GetFileName(Attachment.FullPath)))).FirstOrDefault();

         //if (d != null)
         //{
         //   d.Title = Attachment.Title;
         //   d.UpdatedOn = DateTime.Now;

         //   if (DB_SQL.Update(d))
         //   {
         //      SiteViewModel.Instance.CountDocs();
         //      return true;
         //   };
         //};

         return false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      internal void Reset()
      {
         if (att.AttachmentList != null)
         {
            att.AttachmentList.Clear();
            att.SelectedAttachment = null;

            att.AttachmentList_OnSelectionChanged(null, null);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 


      public int Count { get => _Count; }

      int _Count = 0;

      public void LoadData()
      {
         if (att.AttachmentList != null)
         {
            att.AttachmentList.Clear();

            var list = MainViewModel.Current.Documents.Where(x => x.ExtRef == MainViewModel.Current.SelectedIntervention.PK.ToString() && x.Comment == Comment);
            _Count = 0;

            foreach (var doc in list)
            {
               var a = new TAttachment()
               {
                  ExternalRef = doc.ExtRef,
                  Title = (string.IsNullOrEmpty(doc.Title) ? doc.FileName : doc.Title),
                  Summary = doc.Comment,
                  FullPath = doc.FullPath,
               };

               att.AttachmentList.AddAttachment(a);
               _Count++;
            };

            att.AttachmentList.Redraw();
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      async void saveToolbarItem_Activated(object sender, EventArgs e)
      {
         //SiteViewModel.Instance.SaveSelectedSite();
         //await Navigation.PopAsync();
      }

      async void cancelToolbarItem_Activated(object sender, EventArgs e)
      {
         //await SiteViewModel.Instance.SelectSite(SiteViewModel.Instance.SelectedSite.PK);
         //await Navigation.PopAsync();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public async Task<string> OnProcessPhoto(string FileName, MediaFile file)
      {
         if (!System.IO.Directory.Exists(MainViewModel.Current.DataFolder + @"/Photos/"))
         {
            System.IO.Directory.CreateDirectory(MainViewModel.Current.DataFolder + @"/Photos/");
         };

         FileName = MainViewModel.Current.DataFolder + @"/Photos/" + System.IO.Path.GetFileName(FileName);
         FileName = ZPF.XF.Basics.Current.FileIO.CleanPath(FileName);

         System.IO.File.Move(file.Path, FileName);

         //return FileName;

         // - - - Resize images - - - 

         if (Device.RuntimePlatform == Device.UWP)
         {
            return FileName;
         }
         else
         {
            string FileNameTN = System.IO.Path.GetFileNameWithoutExtension(FileName) + ".TN" + System.IO.Path.GetExtension(FileName);
            FileNameTN = MainViewModel.Current.DataFolder + @"/Photos/" + System.IO.Path.GetFileName(FileNameTN);
            FileNameTN = ZPF.XF.Basics.Current.FileIO.CleanPath(FileNameTN);

            try
            {
               Stream outputStream = null;

               if (AttachmentPanel.MaxSize > 0)
               {
                  if (AttachmentPanel.CompressionQuality > 0 && AttachmentPanel.CompressionQuality < 100)
                  {

                  }
                  else
                  {
                     outputStream = await ImageService.Instance
                       .LoadFile(FileName)
                       .AsJPGStreamAsync(quality: 70);
                  };
               }
               else
               {
                  if (AttachmentPanel.CompressionQuality > 0 && AttachmentPanel.CompressionQuality < 100)
                  {
                  }
                  else
                  {
                     outputStream = await ImageService.Instance
                    .LoadFile(FileName)
                    .DownSample(1024)
                    .AsJPGStreamAsync(quality: AttachmentPanel.CompressionQuality);
                  };
               };

               if (outputStream != null)
               {
                  if (ZPF.XF.Basics.Current.FileIO.WriteStream(outputStream, FileNameTN))
                  {
                     // await DisplayAlert("File Location", FileName, "OK");
                  };
               }
               else
               {
                  System.IO.File.Move(FileName, FileNameTN);
               };
            }
            catch (Exception ex)
            {
               System.IO.File.Copy(FileName, FileNameTN, true);

               Log.Write(ErrorLevel.Critical, ex);
            };

            return FileNameTN;
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  

   }
}
