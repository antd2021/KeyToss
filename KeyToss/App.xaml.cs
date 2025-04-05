using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace KeyToss;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var tabbedPage = new TabbedPage();

        tabbedPage.BarBackgroundColor = Color.FromArgb("#6200EE");
        tabbedPage.BarTextColor = Colors.White;
        tabbedPage.SelectedTabColor = Colors.White;

        var signUpPage = new SignUpPage();
        signUpPage.Title = "SIGN UP";
        tabbedPage.Children.Add(signUpPage);

        var profilePage = new ProfilePage();
        profilePage.Title = "PROFILE";
        tabbedPage.Children.Add(profilePage);

        MainPage = tabbedPage;
    }
}