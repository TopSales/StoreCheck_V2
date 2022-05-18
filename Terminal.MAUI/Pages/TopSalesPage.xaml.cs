namespace StoreCheck.Pages;

public partial class TopSalesPage : ContentPage
{
    public TopSalesPage()
    {
        InitializeComponent();
    }

    private async void btnBack_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}