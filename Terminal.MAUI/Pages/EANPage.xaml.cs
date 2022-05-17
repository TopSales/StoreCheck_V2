namespace StoreCheck.Pages;

public partial class EANPage : ContentPage
{
    public EANPage()
    {
        InitializeComponent();
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}