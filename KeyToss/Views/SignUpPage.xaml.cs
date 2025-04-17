namespace KeyToss;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
    }

    private async void OnSignUpButtonClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            DisplayAlert("Error", "Please fill in all fields.", "OK");
        }
        else
        {
            if (password != confirmPassword)
            {
                DisplayAlert("Error", "Passwords do not match.", "OK");
            }
            else
            {
                await SecureStorage.SetAsync("username", username);
                await SecureStorage.SetAsync("email", email);
                await SecureStorage.SetAsync("password", password);
                DisplayAlert("Account Created", "You have successfully created your account.", "OK");
                Navigation.PopAsync();
            }
        }
    }
}