using Plugin.Maui.Biometric;
using KeyToss.Services;

namespace KeyToss.Views;

public partial class LoginPage : ContentPage
{
    private int passwordAttempts = 3;
    private bool isLockedOut = false;

    public LoginPage()
	{
		InitializeComponent();
    }

    //Comment out the following method to test on windows devices
    //Uncomment if trying to test on android or ios

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

        if (isLockedOut)
        {
            await DisplayAlert("Error", "You are temporarily locked out due to too many failed attempts.", "OK");
            return;
        }

        var hashingService = new BcryptHashingService();

        string enteredUsername = Username.Text;
        string enteredPassword = Password.Text;
        var securedPassword = await SecureStorage.GetAsync("password");
        var securedUsername = await SecureStorage.GetAsync("username"); //Could likely use the same method to get user information for the profile page
        bool verifiedPass = hashingService.VerifyPassword(enteredPassword, securedPassword);


        if (enteredUsername == securedUsername && verifiedPass == true)
        {
            passwordAttempts = 3; // reset on success
            await Navigation.PushAsync(new PasswordsPage());
        }
        else
        {
            passwordAttempts--;

            if (passwordAttempts == 0)
            {
                isLockedOut = true;
                LoginBtn.IsEnabled = false;

                await DisplayAlert("Error", "You are temporarily locked out due to too many failed attempts.", "OK");

                _ = Task.Run(async () =>
                {
                    await Task.Delay(20000); // wait 20 seconds
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        passwordAttempts = 3;
                        LoginBtn.IsEnabled = true;
                        isLockedOut = false;
                    });
                });

            }
            await DisplayAlert("Error", $"Invalid username or password. You have {passwordAttempts} attempts left before lockout. Try Again.", "OK");
        }
    }
}
