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
    //Method for biometric authentication

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
        //Check if the user is locked out
        if (isLockedOut)
        {
            await DisplayAlert("Error", "You are temporarily locked out due to too many failed attempts.", "OK");
            return;
        }

        var hashingService = new BcryptHashingService();

        string enteredUsername = Username.Text;
        string enteredPassword = Password.Text;
        var securedPassword = await SecureStorage.GetAsync("password");
        var securedUsername = await SecureStorage.GetAsync("username"); 
        bool verifiedPass = hashingService.VerifyPassword(enteredPassword, securedPassword);

        //Checking if the user exists and if not prompt them to create an account
        if (securedUsername == null)
        {
            await DisplayAlert("No User Found", "No user exists. Please create an account first.", "OK");
            return;
        }

        //Checking user login credentials
        if (enteredUsername == securedUsername && verifiedPass == true)
        {
            passwordAttempts = 3; // reset on success
            await Navigation.PushAsync(new PasswordsPage());
        }
        //If the user enters the wrong credentials, decrement the attempts and check if they are locked out
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
                    await Task.Delay(300000); // wait 5 minutes
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
