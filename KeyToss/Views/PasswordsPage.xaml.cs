using KeyToss.Models;
using KeyToss.Models.ViewModels;

namespace KeyToss.Views;

public partial class PasswordsPage : ContentPage
{
    public PasswordsPage()
    {
        InitializeComponent();

        if (BindingContext is PasswordListViewModel vm)
        {
            vm.Passwords.Add(new Password { PasswordId = 1, WebsiteName = "GitHub" });
            vm.Passwords.Add(new Password { PasswordId = 2, WebsiteName = "StackOverflow" });
            vm.Passwords.Add(new Password { PasswordId = 3, WebsiteName = "Google" });
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync(
            title: "Edit Password",
            message: "Enter new password:",
            accept: "Save",
            cancel: "Back",
            placeholder: "New password",
            maxLength: 50,
            keyboard: Keyboard.Text);

        if (!string.IsNullOrEmpty(result))
        {

        }
    }

    // ©¤©¤ Delete
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            title: "Delete",
            message: "Are you sure you want to delete the currently selected password?",
            accept: "Yes",
            cancel: "No");

        if (confirm)
        {
        }
    }

    // ©¤©¤ Search
    private async void OnSearchClicked(object sender, EventArgs e)
    {
        string site = await DisplayPromptAsync(
            title: "Search",
            message: "Enter Website Name:",
            accept: "Search",
            cancel: "Back",
            placeholder: "Website Name");

        if (!string.IsNullOrEmpty(site))
        {

        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new AddPasswordPage());
    }

    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new KeyToss.ProfilePage());
    }
}
