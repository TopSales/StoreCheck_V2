using StoreCheck.Pages;

namespace StoreCheck;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

    private void btnStart_Clicked(object sender, EventArgs e)
    {

    }

    private void btnStop_Clicked(object sender, EventArgs e)
    {

    }

    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

    private async void btnContact_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new TopSalesPage());

    }

    private async void btnEAN_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new EANPage());
    }

    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}

