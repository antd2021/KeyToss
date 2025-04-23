namespace KeyToss;

public partial class ProfilePage : ContentPage
{
    private bool _showPassword = false;
    private string _actualPassword = string.Empty;

    public ProfilePage()
    {
        InitializeComponent();
        LoadUserDataAsync();
    }

    //Loading Data from SecureStorage
    private async void LoadUserDataAsync()
    {
        try
        {
            string username = await SecureStorage.GetAsync("username");
            string email = await SecureStorage.GetAsync("email");
            string password = await SecureStorage.GetAsync("password");

            if (!string.IsNullOrEmpty(username))
                UsernameLabel.Text = username;

            if (!string.IsNullOrEmpty(email))
                EmailLabel.Text = email;

            if (!string.IsNullOrEmpty(password))
                _actualPassword = password;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load user data.", "OK");
        }
    }

    //Hiding Password
    private void OnShowPasswordButtonClicked(object sender, EventArgs e)
    {
        _showPassword = !_showPassword;

        if (_showPassword)
        {
            PasswordLabel.Text = _actualPassword;
            ShowPasswordButton.Text = "Hide";
        }
        else
        {
            PasswordLabel.Text = "••••••?";
            ShowPasswordButton.Text = "Show";
        }
    }

    //SignOut Button Functionality
    private async void OnSignOutButtonClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Sign Out", "Are you sure you want to sign out?", "Yes", "No");

        if (confirm)
        {
            Application.Current.MainPage = new NavigationPage(new Views.LoginPage());
        }
    }
}