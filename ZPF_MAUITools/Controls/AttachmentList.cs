namespace ZPF.XF.Compos
{
    public class AttachmentList : ScrollView
   {
      //public class AImage : CachedImage
      public class AImage : Image
      {
         public AImage()
         {
            Attachment = new TAttachment();

            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;

            //if( this.GetType().ToString() == "CachedImage")
            //{
            //   CachedImage ci = (CachedImage)(this);

            //   ((CachedImage)(this)).DownsampleToViewSize = true;
            //};

            WidthRequest = 500;
            HeightRequest = 500;

            //CacheDuration = TimeSpan.FromDays(1);
            //RetryCount = 0;
            //RetryDelay = 250;
            //LoadingPlaceholder = "loading.png";
            //ErrorPlaceholder = "error.png";
            //Source = FileName;
         }

         public TAttachment Attachment { get; set; }
      };

      StackLayout _StackLayout = null;

      public AttachmentList()
      {
         MaxAttachments = -1;

         Redraw();
         this.PropertyChanged += AttachmentList_PropertyChanged;
      }

      private void AttachmentList_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "Orientation" || e.PropertyName == "ContentSize")
         {
            Redraw();
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public int MaxAttachments { get; set; }
      public TAttachment SelectedAttachment { get; private set; }
      public Action<object, TAttachment> OnSelectionChanged { get; set; }
      public object DB_SQL { get; internal set; }

      public List<TAttachment> Attachments = new List<TAttachment>();

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public void AddFile(string FileName, string ExternalRef = null, string Title = null)
      {
         var a = new TAttachment(FileName);

         if (!string.IsNullOrEmpty(ExternalRef))
         {
            a.ExternalRef = ExternalRef;
         };

         if (!string.IsNullOrEmpty(Title))
         {
            a.Title = Title;
         };

         var i = new AImage()
         {
            //Source =  a.ImageSource,
            Source = LoadImage(a.FullPath),
            Attachment = a,
         };

         Attachments.Add(a);

         if (this.Orientation == ScrollOrientation.Horizontal)
         {
            //ToDo:???
            i.WidthRequest = this.Height;
         }
         else
         {
            i.HeightRequest = this.Width;
         };

         var l2Tap = new TapGestureRecognizer();
         l2Tap.Tapped += (object sender, EventArgs e) =>
         {
            SelectedAttachment = (sender as AImage).Attachment;

            if (OnSelectionChanged != null)
            {
               OnSelectionChanged(this, (sender as AImage).Attachment);
            };
         };

         i.GestureRecognizers.Add(l2Tap);

         if (Device.RuntimePlatform == Device.UWP)
         {
            i.MinimumWidthRequest = 50;
            i.MinimumHeightRequest = 50;
         };

         _StackLayout.Children.Add(i);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public static ImageSource LoadImage(string fullPath)
      {
         return ImageSource.FromFile(fullPath);

         //// http://stackoverflow.com/questions/37720864/xamarin-forms-uwp-image-never-closes-opened-file
         //var memoryStream = new System.IO.MemoryStream();

         //fullPath = ZPF.XF.Basics.Current.FileIO.CleanPath(fullPath);
         //using (var stream = ZPF.XF.Basics.Current.FileIO.ReadStream(fullPath))
         //{
         //   stream.CopyTo(memoryStream);
         //};

         //memoryStream.Position = 0;
         //return ImageSource.FromStream(() => memoryStream);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public bool AddAttachment(TAttachment Attachment)
      {
         Attachments.Add(Attachment);
         return true;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public void Redraw()
      {
         if (this.Orientation == ScrollOrientation.Horizontal)
         {
            _StackLayout = new StackLayout()
            {
               Orientation = StackOrientation.Horizontal,
            };

            // - - -  - - - 

            foreach (var a in Attachments)
            {
               try
               {
                  if (a.IsImage && a.ImageSource != null)
                  {
                     var i = new AImage()
                     {
                        Source = a.ImageSource,
                        Attachment = a,
                        WidthRequest = (this.Width > 0 ? this.Width / 3 : 50),
                        Aspect = Aspect.AspectFit,
                     };

                     _StackLayout.Children.Add(i);

                     var l2Tap = new TapGestureRecognizer();
                     l2Tap.Tapped += (object sender, EventArgs e) =>
                     {
                        SelectedAttachment = (sender as AImage).Attachment;

                        if (OnSelectionChanged != null)
                        {
                           OnSelectionChanged(this, (sender as AImage).Attachment);
                        };
                     };

                     i.GestureRecognizers.Add(l2Tap);
                  }
                  else
                  {
                     var i = new Tile();
                     //i.Source = ImageSource.FromResource( a.MimeIcon, typeof(PageEx));
                     i.IconChar = a.MimeIcon;
                     i.Text = a.FileName;
                     //i.Orientation = ImageOrientation.ImageToLeft;

                     i.Clicked += (object sender, EventArgs e) =>
                     {
                        if (OnSelectionChanged != null)
                        {
                           OnSelectionChanged(this, a);
                        };
                     };

                     _StackLayout.Children.Add(i);
                  }
               }
               catch
               {
               };
            }

            // - - -  - - - 

            Content = _StackLayout;
         }
         else
         {
            _StackLayout = new StackLayout()
            {
               Orientation = StackOrientation.Vertical,
            };

            // - - -  - - - 

            foreach (var a in Attachments)
            {
               if (a.IsImage)
               {
                  var i = new AImage()
                  {
                     Source = a.ImageSource,
                     Attachment = a,
                     HeightRequest = (this.Height > 0 ? this.Height / 3 : 50),
                     Aspect = Aspect.AspectFit,
                  };

                  _StackLayout.Children.Add(i);

                  var l2Tap = new TapGestureRecognizer();
                  l2Tap.Tapped += (object sender, EventArgs e) =>
                  {
                     SelectedAttachment = (sender as AImage).Attachment;

                     if (OnSelectionChanged != null)
                     {
                        OnSelectionChanged(this, (sender as AImage).Attachment);
                     };
                  };

                  i.GestureRecognizers.Add(l2Tap);
               }
               else
               {
                  var i = new Tile();
                  //i.Source = ImageSource.FromResource( a.MimeIcon, typeof(PageEx));
                  i.IconChar = a.MimeIcon;
                  i.Text = a.FileName;
                  //i.Orientation = ImageOrientation.ImageToLeft;

                  i.Clicked += (object sender, EventArgs e) =>
                  {
                     if (OnSelectionChanged != null)
                     {
                        OnSelectionChanged(this, a);
                     };
                  };

                  _StackLayout.Children.Add(i);
               }
            }

            // - - -  - - - 

            Content = _StackLayout;
         }
      }


      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      public void Clear()
      {
         _StackLayout.Children.Clear();
         Attachments.Clear();
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 
   }

   public class TAttachment
   {
      public TAttachment()
      {
      }

      public TAttachment(string FileName)
      {
         FullPath = FileName;
      }

      public string ExternalRef { get; set; }

      public string FullPath { get; set; }
      public string FileName { get { return Path.GetFileNameWithoutExtension(FullPath); } }


      string _Title = "";
      public string Title
      {
         get
         {
            return (string.IsNullOrEmpty(_Title) ? FileName : _Title);
         }
         set
         {
            _Title = value;
         }
      }


      public string Summary { get; set; }


      // Icons: 32 px padding 4 px
      public string MimeIcon
      {
         get
         {
            string Result = "";
            string IconChar = "";
            string ext = Path.GetExtension(FullPath);

            switch (ext.ToLower())
            {
               case ".txt":
                  Result = "text";
                  IconChar = ZPF.Fonts.IF.Text_Document;
                  break;

               case ".pdf":
                  Result = "pdf";
                  IconChar = ZPF.Fonts.IF.File_Format_PDF;
                  break;

               case ".mp4":
                  Result = "video";
                  IconChar = ZPF.Fonts.IF.Video;
                  break;

               case ".mp3":
               case ".wma":
                  Result = "audio";
                  IconChar = ZPF.Fonts.IF.Document_Music_02;
                  break;

               case ".xls":
               case ".xlsx":
                  Result = "xls";
                  IconChar = ZPF.Fonts.IF.Excel_Online;
                  break;

               case ".png":
               case ".jpg":
               case ".jpeg":
                  Result = "image";
                  IconChar = ZPF.Fonts.IF.Picture;
                  break;
            };

            Result = $"ZPF_XF_Compos.Images.FilesIcons.FileIcon_{Result}.png";

            return IconChar;
         }
      }

      public static string GetMime(string fileName)
      {
         string Result = "";
         // http://indyvision.net/2010/03/android-using-intents-open-files/

         string ext = Path.GetExtension(fileName);
         switch (ext.ToLower())
         {
            case ".txt":
               Result = "text/plain";
               break;

            case ".pdf":
               Result = "application/pdf";
               break;

            case ".mp4":
               Result = "video/*";
               break;

            case ".mp3":
               Result = "audio/*";
               break;

            case ".wma":
               Result = "audio/*";
               break;

            case ".xls":
               Result = "application/xls";
               break;

            case ".xlsx":
               Result = "application/xls";
               break;

            case ".png":
               Result = "image/png";
               break;

            case ".jpg":
               Result = "image/jpg";
               break;

            case ".jpeg":
               Result = "image/jpeg";
               break;
         };

         return Result;
      }

      public bool IsImage
      {
         get
         {
            return GetMime(FullPath).StartsWith("image/", StringComparison.Ordinal);
         }
      }

      public ImageSource ImageSource
      {
         get
         {
            if (IsImage)
            {
               if (System.IO.File.Exists(FullPath))
               {
                  return ImageSource.FromFile(FullPath);
               }
               else
               {
                  return ImageSource.FromResource("ZPF_XF_Compos.Images.PlaceHolder.png", typeof(PageEx));
               };
            }
            else
            {
               return ImageSource.FromResource("ZPF_XF_Compos.Images.PlaceHolder.png", typeof(PageEx));
            }
         }
      }
   }
}
