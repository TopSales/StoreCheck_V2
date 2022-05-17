using StoreCheck.Pages;

namespace StoreCheck;

public partial class MainPage : ContentPage
{
    int count = 0;

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

    private void btn1_Clicked(object sender, EventArgs e)
    {

    }

    private async void btnEAN_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new EANPage());
    }

    // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
}

