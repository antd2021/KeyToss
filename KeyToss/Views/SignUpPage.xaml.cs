using KeyToss.Services;
using System.Security.Cryptography;

namespace KeyToss;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
    {
        InitializeComponent();
    }

    public static byte[] GenerateAESKey(int keySizeInBits = 256)
    {
        byte[] key = new byte[keySizeInBits / 8];
        RandomNumberGenerator.Fill(key);
        return key;
    }

    public static byte[] GenerateAESIV()
    {
        byte[] iv = new byte[16];
        RandomNumberGenerator.Fill(iv);
        return iv;
    }


    private async void OnSignUpButtonClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim();
        string email = EmailEntry.Text?.Trim();
        string plainTextPassword = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(plainTextPassword) ||
            string.IsNullOrWhiteSpace(confirmPassword))
        {
            DisplayAlert("Error", "Please fill in all fields.", "OK");
        }
        else
        {
            if (plainTextPassword != confirmPassword)
            {
                DisplayAlert("Error", "Passwords do not match.", "OK");
            }
            else
            {
                var hashingService = new BcryptHashingService();
                var hashedPassword = hashingService.HashPassword(plainTextPassword);


                await SecureStorage.SetAsync("username", username);
                await SecureStorage.SetAsync("email", email);
                await SecureStorage.SetAsync("password", hashedPassword);
                await SecureStorage.SetAsync("aesKey", Convert.ToBase64String(GenerateAESKey()));
                await SecureStorage.SetAsync("aesIV", Convert.ToBase64String(GenerateAESIV()));
                DisplayAlert("Account Created", "You have successfully created your account.", "OK");
                Navigation.PopAsync();
            }
        }
    }
}