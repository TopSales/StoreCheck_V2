namespace StoreCheck.Views
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class InterventionView : ContentView
   {
      public InterventionView()
      {
         this.BindingContext = MainViewModel.Current.SelectedIntervention;

         InitializeComponent();
      }
   }
}
