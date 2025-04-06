using KeyToss.Models;
using KeyToss.Models.ViewModels;

namespace KeyToss.Views;

public partial class AddPasswordPage : ContentPage
{
    public AddPasswordPage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var site = WebsiteEntry.Text?.Trim();
        var pwd = PasswordEntry.Text;
        var conf = ConfirmEntry.Text;

        if (!string.IsNullOrEmpty(site) && pwd == conf)
        {
            if (Application.Current.MainPage is NavigationPage nav &&
                nav.CurrentPage is PasswordsPage pp &&
                pp.BindingContext is PasswordListViewModel vm)
            {
                vm.Passwords.Add(new Password
                {
                    PasswordId = vm.Passwords.Count + 1,
                    WebsiteName = site,
                    EncryptedPassword = pwd
                });
            }
            await Navigation.PopModalAsync();
        }
        else
        {
            await DisplayAlert("Error", "Please fill correctly.", "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
