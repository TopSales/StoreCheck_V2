using ZPF.XF.Compos;

namespace StoreCheck.Pages;

public partial class ContactPage : PageEx
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
