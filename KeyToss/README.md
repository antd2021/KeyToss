# Getting started with KeyToss
The program should be able to start off the bat without any issues. However, if you encounter any problems, please follow the instructions below to resolve them.


# Running the Program on a Windows VM

When attempting to run the program on a Windows Virtual Machine (VM), there is an important adjustment you need to make in order to avoid runtime issues.

## Required Change

You will need to **comment out the first method** within the `LoginPage.xaml.cs` file.

---

### Why?

Some code within the first method may depend on platform-specific functionality that is incompatible or unavailable in the Windows VM environment. Commenting it out ensures the application can run without encountering errors related to these dependencies.

---

### How to Do It

1. Open the `LoginPage.xaml.cs` file.
2. Locate the **first method** (usually right after the constructor or any using directives).
3. Comment out the entire method.

   For example:
       //protected override async void OnAppearing()
    //{
    //    base.OnAppearing();

    //    var biometricResult = await BiometricAuthenticationService.Default.AuthenticateAsync(new AuthenticationRequest
    //    {
    //        Title = "Please Authenticate",
    //        NegativeText = "Cancel"
    //    }, CancellationToken.None);

    //    if (biometricResult.Status == BiometricResponseStatus.Success)
    //    {
    //        await Navigation.PushAsync(new PasswordsPage());
    //    }
    //    else
    //    {
    //        await DisplayAlert("Error", "Biometric Authentication Failed", "OK");
    //    }
    //}
