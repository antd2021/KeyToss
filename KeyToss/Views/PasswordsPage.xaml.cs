using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using KeyToss.Models;
using KeyToss.Models.ViewModels;
using KeyToss.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Text.Json;

namespace KeyToss.Views
{
    public partial class PasswordsPage : ContentPage
    {
        private List<Password> _allPasswords = new();
        public PasswordsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 1) Get the current logged-in username
            var username = await SecureStorage.GetAsync("username");
            if (string.IsNullOrEmpty(username))
                return;

            // 2) Retrieve and deserialize stored password list
            var key = $"pwlist_{username}";
            var json = await SecureStorage.GetAsync(key) ?? "[]";
            _allPasswords = JsonSerializer.Deserialize<List<Password>>(json)!;

            // 3) Load into ViewModel
            if (BindingContext is PasswordListViewModel vm)
            {
                vm.Passwords.Clear();
                foreach (var p in _allPasswords)
                    vm.Passwords.Add(p);
            }
        }

        // ©¤©¤ Search (retain original UI, but filter data from storage)
        private async void OnSearchClicked(object sender, EventArgs e)
        {
            string site = await DisplayPromptAsync(
                title: "Search",
                message: "Enter Website Name:",
                accept: "Search",
                cancel: "Back",
                placeholder: "Website Name");

            if (!(BindingContext is PasswordListViewModel vm))
                return;

            vm.Passwords.Clear();
            if (site is null)
            {
                // Restore all passwords when user cancels
                foreach (var p in _allPasswords)
                    vm.Passwords.Add(p);
            }
            else
            {
                // Filter list based on input
                foreach (var p in _allPasswords
                    .Where(pw => pw.WebsiteName
                        .Contains(site, StringComparison.OrdinalIgnoreCase)))
                {
                    vm.Passwords.Add(p);
                }
            }
        }

        // ©¤©¤ Home button: reset full password list
        private void OnHomeClicked(object sender, EventArgs e)
        {
            if (!(BindingContext is PasswordListViewModel vm))
                return;

            vm.Passwords.Clear();
            foreach (var p in _allPasswords)
                vm.Passwords.Add(p);
        }

        // ©¤©¤ Edit password
        private async void OnEditClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var selectedPassword = button.BindingContext as Password;

            if (selectedPassword == null)
                return;

            var username = await SecureStorage.GetAsync("username");
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Error", "User not found", "OK");
                return;
            }

            // Decrypt existing password
            string decrypted = AESEncryptionService.DecryptStringAES(
                selectedPassword.EncryptedPassword,
                Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF"),
                Encoding.UTF8.GetBytes("ABCDEF0123456789")
            );

            // Ask if user wants to generate a strong password
            bool useGenerated = await DisplayAlert(
                "Generate Password",
                "Do you want to generate a strong password?",
                "Yes", "No");

            string newPassword;

            if (useGenerated)
            {
                var generator = new PasswordGeneratorService();
                newPassword = generator.GeneratePassword();
            }
            else
            {
                newPassword = await DisplayPromptAsync(
                    title: "Edit Password",
                    message: "Edit your password:",
                    accept: "Save",
                    cancel: "Back",
                    initialValue: decrypted,
                    placeholder: "Enter new password",
                    maxLength: 50,
                    keyboard: Keyboard.Text);
            }

            if (string.IsNullOrEmpty(newPassword))
                return;

            // Re-encrypt updated password
            string newEncrypted = AESEncryptionService.EncryptStringAES(
                newPassword,
                Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF"),
                Encoding.UTF8.GetBytes("ABCDEF0123456789")
            );

            // Update stored password JSON
            string key = $"pwlist_{username}";
            var json = await SecureStorage.GetAsync(key) ?? "[]";
            var list = JsonSerializer.Deserialize<List<Password>>(json)!;

            var target = list.FirstOrDefault(p =>
                p.WebsiteName == selectedPassword.WebsiteName &&
                p.Username == selectedPassword.Username &&
                p.EncryptedPassword == selectedPassword.EncryptedPassword);

            if (target != null)
            {
                target.EncryptedPassword = newEncrypted;
            }

            string newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(key, newJson);

            // Update the page UI
            if (BindingContext is PasswordListViewModel vm)
            {
                selectedPassword.EncryptedPassword = newEncrypted;
                // ObservableCollection will refresh the UI automatically
            }
        }

        // ©¤©¤ Delete selected password from storage and UI
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                title: "Delete",
                message: "Are you sure you want to delete the currently selected password?",
                accept: "Yes",
                cancel: "No");

            if (!confirm) return;

            var button = (Button)sender;
            var selectedPassword = button.BindingContext as Password;

            if (selectedPassword == null)
                return;

            var username = await SecureStorage.GetAsync("username");
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Error", "User not found", "OK");
                return;
            }

            string key = $"pwlist_{username}";
            var json = await SecureStorage.GetAsync(key) ?? "[]";
            var list = JsonSerializer.Deserialize<List<Password>>(json)!;

            list.RemoveAll(p => p.WebsiteName == selectedPassword.WebsiteName &&
                                p.Username == selectedPassword.Username &&
                                p.EncryptedPassword == selectedPassword.EncryptedPassword);

            var newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(key, newJson);

            if (BindingContext is PasswordListViewModel vm)
            {
                vm.Passwords.Remove(selectedPassword);
            }
        }

        // ©¤©¤ Add new password entry (retains compatibility with original navigation)
        private async void OnAddClicked(object sender, EventArgs e)
            => await Navigation.PushModalAsync(new AddPasswordPage());

        // ©¤©¤ Navigate to Profile page
        private async void OnProfileClicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new ProfilePage());
    }
}
