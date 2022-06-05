namespace StoreCheck.Views
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class PhotoView : ContentView
   {
      public PhotoView()
      {
         this.BindingContext = MainViewModel.Current.SelectedIntervention;

         InitializeComponent();
      }

      public string Text { get => lbText.Text; set => lbText.Text = value; }
   }
}
