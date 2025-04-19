using Plugin.Maui.Biometric;
using KeyToss.Services;

namespace KeyToss.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var biometricResult = await BiometricAuthenticationService.Default.AuthenticateAsync(new AuthenticationRequest
        {
            Title = "Please Authenticate",
            NegativeText = "Cancel"
        }, CancellationToken.None);

        if (biometricResult.Status == BiometricResponseStatus.Success)
        {
            await Navigation.PushAsync(new PasswordsPage());
        }
        else
        {
            await DisplayAlert("Error", "Biometric Authentication Failed", "OK");
        }
    }


    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		Navigation.PushAsync(new SignUpPage());
    }

    private async void LoginBtn_Clicked(object sender, EventArgs e)
    {
        var hashingService = new BcryptHashingService();

        string enteredUsername = Username.Text;
        string enteredPassword = Password.Text;
        var securedPassword = await SecureStorage.GetAsync("password");
        var securedUsername = await SecureStorage.GetAsync("username"); //Could likely use the same method to get user information for the profile page
        bool verifiedPass = hashingService.VerifyPassword(enteredPassword, securedPassword);


        if (enteredUsername == securedUsername && verifiedPass == true)
        {
            Navigation.PushAsync(new PasswordsPage());
        }
        else
        {
            DisplayAlert("Error", "Invalid username or password. Try Again.", "OK");
        }
    }
}
