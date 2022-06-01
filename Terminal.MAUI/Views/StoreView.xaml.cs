namespace StoreCheck.Views;

public partial class StoreView : ContentView
{
	public StoreView()
	{
      this.BindingContext = MainViewModel.Current;

      InitializeComponent();

      lTypeStore.Text = (MainViewModel.Current.SelectedIntervention != null ? MainViewModel.Current.SelectedIntervention.StoreType : "");
   }
}
