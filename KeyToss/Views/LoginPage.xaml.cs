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

    private void SignUpBtn_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new PasswordsPage());
    }
}