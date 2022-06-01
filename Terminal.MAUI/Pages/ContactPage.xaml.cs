namespace StoreCheck.Pages;

public partial class ContactPage : ContentPage
{
   private Store_CE selectedStore;

   public ContactPage()
	{
		InitializeComponent();
	}

   public ContactPage(Store_CE selectedStore)
   {
      this.selectedStore = selectedStore;
   }
}