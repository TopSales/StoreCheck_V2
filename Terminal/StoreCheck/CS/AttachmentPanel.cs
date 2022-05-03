using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using ZPF.Compos.XF;
using ZPF.Conv;
using ZPF.XF.Compos;
using static ZPF.XF.Compos.AttachmentList;

namespace ZPF.XF
{
   public class AttachmentPanel : Grid
   {
      public static int MaxSize { get; set; } = 1024;
      public static int CompressionQuality { get; set; } = 70;

      // - - -  - - - 

      Tile btnDeleteImage = null;
      Tile btnAddImage = null;
      Tile btnChooseImage = null;

      // - - -  - - - 

      TAttachment _SelectedAttachment = null;
      public TAttachment SelectedAttachment
      {
         get { return _SelectedAttachment; }
         set
         {
            if (_SelectedAttachment != value)
            {
               _SelectedAttachment = value;
               OnPropertyChanged();
            };
         }
      }

      // - - -  - - - 

      AttachmentList _AttachmentList = null;

      public AttachmentList AttachmentList
      {
         get { return _AttachmentList; }
         set { _AttachmentList = value; }
      }

      // - - -  - - - 

      public delegate void VoidEventHandler();
      public VoidEventHandler OnLoadData { get; set; }

      public Action<IDocument, string> OnAddData
      {
         get => _OnAddData;
         set
         {
            _OnAddData = value;
            btnAddImage.IsEnabled = (_OnAddData != null);
            btnChooseImage.IsEnabled = (_OnAddData != null);
         }
      }
      Action<IDocument, string> _OnAddData = null;

      public Func<TAttachment, bool> OnDelData
      {
         get => _OnDelData;
         set
         {
            _OnDelData = value;
            btnDeleteImage.IsEnabled = (_OnDelData != null);
         }
      }
      Func<TAttachment, bool> _OnDelData = null;

      public Func<TAttachment, bool> OnUpdateData
      {
         get => _OnUpdateData;
         set => _OnUpdateData = value;
      }
      Func<TAttachment, bool> _OnUpdateData = null;

      public Func<string, MediaFile, Task<string>> OnProcessPhoto { get; set; }

      // - - -  - - - 

      public string FileName { get; set; }
      public IDocument Document { get; set; }

      
      // - - -  - - - 

      AImage iPreview = null;
      private Page _Parent;

      public AttachmentPanel(Page parent, bool IsReadOnly = false)
      {
         _Parent = parent;
         FileName = "Image.jpg";

         Margin = new Thickness(10, 5, 10, 5);

         this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
         this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });
         this.RowDefinitions.Add(new RowDefinition { Height = new GridLength(100, GridUnitType.Absolute) });

         // - - -  - - - 

         {
            var b = new BoxView()
            {
               BackgroundColor = Xamarin.Forms.Color.Silver,
            };
            this.Children.Add(b, 0, 1, 0, 1);

            iPreview = new AImage();

            this.Children.Add(iPreview, 0, 0);
         };

         // - - -  - - - 

         {
            var gH = new Grid();
            gH.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gH.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50, GridUnitType.Absolute) });

            {
               var b = new BoxView()
               {
                  BackgroundColor = Xamarin.Forms.Color.Silver,
                  HeightRequest = 48,
               };

               gH.Children.Add(b, 0, 0);
            };

            {
               var b = new EntryEx()
               {
                  Margin = new Thickness(5, 0, 5, 0),
                  VerticalOptions = LayoutOptions.Center,
                  IsEnabled = !IsReadOnly,
               };

               b.Unfocused += B_Unfocused;
               b.TextChanged += B_TextChanged;

               b.BindingContext = this;
               b.SetBinding(Entry.TextProperty, "SelectedAttachment.Title");

               gH.Children.Add(b, 0, 0);
            };

            {
               btnDeleteImage = new Tile()
               {
                  Text = "",
                  IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Delete),
                  BackgroundColor = ColorViewModel.Current.ActionBackgroundColor,
                  TextColor = ColorViewModel.Current.TextColor,
                  HeightRequest = 48,
                  IsEnabled = !IsReadOnly && (OnDelData != null),
               };

               btnDeleteImage.BindingContext = this;
               btnDeleteImage.SetBinding(Tile.IsEnabledProperty, new Binding("SelectedAttachment", BindingMode.OneWay, new ToBoolConverter()));
               btnDeleteImage.Clicked += btnDeleteImage_Clicked;

               gH.Children.Add(btnDeleteImage, 1, 0);
            };

            this.Children.Add(gH, 0, 1);
         };

         // - - -  - - - 

         {
            var gH = new Grid();
            gH.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            gH.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50, GridUnitType.Absolute) });

            {
               AttachmentList = new AttachmentList
               {
                  BackgroundColor = Xamarin.Forms.Color.Silver,
                  MaxAttachments = 2,
                  Orientation = ScrollOrientation.Horizontal,
               };
               AttachmentList.OnSelectionChanged += AttachmentList_OnSelectionChanged;

               gH.Children.Add(AttachmentList, 0, 0);
            };

            {
               var s = new StackLayout
               {
               };

               {
                  btnAddImage = new Tile() // ImageButton()
                  {
                     Text = "",
                     IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Camera_01_WF),
                     BackgroundColor = ColorViewModel.Current.ActionBackgroundColor,
                     TextColor = ColorViewModel.Current.TextColor,
                     HeightRequest = 48,
                     IsEnabled = !IsReadOnly && (OnAddData != null),
                  };

                  btnAddImage.Clicked += btnAddImage_Clicked;
                  s.Children.Add(btnAddImage);
               };

               {
                  btnChooseImage = new Tile() // ImageButton()
                  {
                     Text = "",
                     IconChar = ZPF.Fonts.IF.GetContent(ZPF.Fonts.IF.Picture),
                     BackgroundColor = ColorViewModel.Current.ActionBackgroundColor,
                     TextColor = ColorViewModel.Current.TextColor,
                     HeightRequest = 48,
                     IsEnabled = !IsReadOnly && (OnAddData != null),
                  };

                  btnChooseImage.BindingContext = this;
                  btnChooseImage.Clicked += btnChooseImage_Clicked;
                  s.Children.Add(btnChooseImage);
               };

               gH.Children.Add(s, 1, 0);
            };

            this.Children.Add(gH, 0, 2);
         };

         // - - -  - - - 

      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      string PrevTitle = null;

      private void B_Unfocused(object sender, FocusEventArgs e)
      {
         if (SelectedAttachment != null)
         {
            if (PrevTitle != null && PrevTitle != SelectedAttachment.Title)
            {
               if (OnUpdateData != null)
               {
                  OnUpdateData(SelectedAttachment);
                  PrevTitle = null;
               };
            };
         };
      }

      private void B_TextChanged(object sender, TextChangedEventArgs e)
      {
         if (SelectedAttachment != null)
         {
            if (PrevTitle != null && PrevTitle != SelectedAttachment.Title)
            {
               if (OnUpdateData != null)
               {
                  OnUpdateData(SelectedAttachment);
                  PrevTitle = SelectedAttachment.Title;
               };
            };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - - 

      internal void AttachmentList_OnSelectionChanged(object sender, TAttachment Attachment)
      {
         SelectedAttachment = Attachment;

         if (Attachment == null)
         {
            PrevTitle = "";
            iPreview.Source = null;
         }
         else
         {
            PrevTitle = SelectedAttachment.Title;
            iPreview.Source = ImageSource.FromFile(Attachment.FullPath);
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private async void btnAddImage_Clicked(object sender, EventArgs e)
      {
         // jamesmontemagno / Xam.Plugin.Media
         // https://github.com/jamesmontemagno/MediaPlugin
         await CrossMedia.Current.Initialize();

         if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
         {
            await _Parent.DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
            return;
         }

         string _FileName = System.IO.Path.GetFileNameWithoutExtension(FileName) + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + System.IO.Path.GetExtension(FileName);

         Plugin.Media.Abstractions.MediaFile file = null;

         try
         {
            file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
               //PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
               PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
               MaxWidthHeight = MaxSize,
               CompressionQuality = CompressionQuality,
               Directory = @"Temp",
               Name = _FileName,
               AllowCropping = false,
               SaveToAlbum = false,
            });
         }
         catch (Exception ex)
         {
            var infos = new System.Collections.Generic.Dictionary<string, string>
                  {
                     { "Exception", ex.Message },
                     { "StackTrace", ex.StackTrace }
                  };

            Microsoft.AppCenter.Analytics.Analytics.TrackEvent("MediaImplementation.TakePhotoAsync", infos);

            BackboneViewModel.Current.MessageBox(BackboneViewModel.MessageBoxType.Error, "Problem while taking a picture.", "Oups ...");
         };

         if (file == null)
            return;

         BackboneViewModel.Current.IncBusy();
         BackboneViewModel.Current.BusyTitle = "Image processing";
         BackboneViewModel.Current.BusySubTitle = "Generation of the thumbnail ...";

         string newFilePath = _FileName;
         if (OnProcessPhoto != null)
         {
            newFilePath = await OnProcessPhoto(_FileName, file);
         };

         BackboneViewModel.Current.DecBusy();

         TAttachment attachment = new TAttachment(newFilePath);
         _AttachmentList.AddAttachment(attachment);

         if (OnAddData != null)
         {
            if (Document != null)
            {
               OnAddData(Document, newFilePath);
            }
            else
            {
               OnAddData(null, newFilePath);
            };
         };

         if (OnLoadData != null)
         {
            OnLoadData();
         }

         SelectedAttachment = attachment;
         AttachmentList_OnSelectionChanged(null, attachment);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private async void btnChooseImage_Clicked(object sender, EventArgs e)
      {
         if (!CrossMedia.Current.IsPickPhotoSupported)
         {
            await _Parent.DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");

            return;
         };

         string _FileName = System.IO.Path.GetFileNameWithoutExtension(FileName) + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + System.IO.Path.GetExtension(FileName);

         var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
         {
            PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
         });

         if (file == null)
            return;

         string newFilePath = _FileName;
         // public static async Task<string> ProcessPhoto(string FileName, MediaFile file)
         if (OnProcessPhoto != null)
         {
            BackboneViewModel.Current.IncBusy();
            BackboneViewModel.Current.BusyTitle = "Image processing";
            BackboneViewModel.Current.BusySubTitle = "Generation of the thumbnail ...";

            newFilePath = await OnProcessPhoto(_FileName, file);

            BackboneViewModel.Current.DecBusy();
         };

         TAttachment attachment = new TAttachment(newFilePath);
         _AttachmentList.AddAttachment(attachment);

         if (OnAddData != null)
         {
            if (Document != null)
            {
               OnAddData(Document, newFilePath);
            }
            else
            {
               OnAddData(null, newFilePath);
            };
         };

         if (OnLoadData != null)
         {
            OnLoadData();
         }

         SelectedAttachment = attachment;
         AttachmentList_OnSelectionChanged(null, attachment);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private async void btnDeleteImage_Clicked(object sender, EventArgs e)
      {
         //ToDo: _Parent.DisplayAlertT("Confirmation", "Supprimer l'image?", "OK", "annuler") 
         if (await _Parent.DisplayAlert("Confirmation", "Delete the image?", "OK", "annuler") == true)
         {
            if (OnDelData != null)
            {
               if (OnDelData(SelectedAttachment))
               {
                  string FullPath = SelectedAttachment.FullPath;
                  await SelectedAttachment.ImageSource.Cancel();
                  SelectedAttachment = null;
                  iPreview.Source = null;

                  try
                  {
                     if (System.IO.File.Exists(FullPath))
                     {
                        System.IO.File.Delete(FullPath);
                     };
                  }
                  catch { };

                  try
                  {
                     FullPath = FullPath.Replace(".TN.", ".");
                     if (System.IO.File.Exists(FullPath))
                     {
                        System.IO.File.Delete(FullPath);
                     };
                  }
                  catch { };

                  if (OnLoadData != null)
                  {
                     OnLoadData();
                  }
               };
            };
         };
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}
