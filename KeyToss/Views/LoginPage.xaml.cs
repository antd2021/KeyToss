using Plugin.Maui.Biometric;

namespace KeyToss.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		Navigation.PushAsync(new SignUpPage());
    }

    private async void LoginBtn_Clicked(object sender, EventArgs e)
    {
        string enteredUsername = Username.Text;
        string enteredPassword = Password.Text;

        string securedUsername = await SecureStorage.GetAsync("username"); //Could likely use the same method to get user information for the profile page
        string securedPassword = await SecureStorage.GetAsync("password");

        if (enteredUsername == securedUsername && enteredPassword == securedPassword)
        {
            Navigation.PushAsync(new PasswordsPage());
        }
        else
        {
            DisplayAlert("Error", "Invalid username or password. Try Again.", "OK");
        }

        //var biometricResult = await BiometricAuthenticationService.Default.AuthenticateAsync(new AuthenticationRequest()
        //{
        //    Title = "Please Authenticate ",
        //    NegativeText = "Cancel Authentication"
        //}, CancellationToken.None);

        //if (biometricResult.Status == BiometricResponseStatus.Success)
        //{
            
        //}
        //else { await DisplayAlert("Error!", "Authentication Failed", "OK"); }
    }
}
