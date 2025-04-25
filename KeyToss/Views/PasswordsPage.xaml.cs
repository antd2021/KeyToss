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
using System.Security.Cryptography;
using Plugin.LocalNotification;

namespace KeyToss.Views
{
    public partial class PasswordsPage : ContentPage
    {
        private byte[] _aesKey;
        private byte[] _aesIv;

        private List<Password> _allPasswords = new();
        
        public PasswordsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await LocalNotificationCenter.Current.RequestNotificationPermission();

            var b64Key = await SecureStorage.GetAsync("aesKey");
            var b64Iv = await SecureStorage.GetAsync("aesIV");
            _aesKey = Convert.FromBase64String(b64Key);
            _aesIv = Convert.FromBase64String(b64Iv);

            var username = await SecureStorage.GetAsync("username");
            if (string.IsNullOrEmpty(username)) return;
            var json = await SecureStorage.GetAsync($"pwlist_{username}") ?? "[]";
            _allPasswords = JsonSerializer.Deserialize<List<Password>>(json)!;
            if (BindingContext is PasswordListViewModel vm)
            {
                vm.Passwords.Clear();
                foreach (var p in _allPasswords)
                    vm.Passwords.Add(p);
            }
        }

        // ── Search (retain original UI, but filter data from storage)
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

        // ── Home button: reset full password list
        private void OnHomeClicked(object sender, EventArgs e)
        {
            if (!(BindingContext is PasswordListViewModel vm))
                return;

            vm.Passwords.Clear();
            foreach (var p in _allPasswords)
                vm.Passwords.Add(p);
        }

        // ── Edit password
        private async void OnEditClicked(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var pwd = btn.BindingContext as Password;
            if (pwd == null) return;

            await Navigation.PushModalAsync(new EditPasswordPage(pwd));
        }

        // ── Delete selected password from storage and UI
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

            list.RemoveAll(p =>
                p.WebsiteName == selectedPassword.WebsiteName &&
                p.Username == selectedPassword.Username &&
                p.EncryptedPassword == selectedPassword.EncryptedPassword);
            _allPasswords = list;

            var newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(key, newJson);

            if (BindingContext is PasswordListViewModel vm)
            {
                vm.Passwords.Remove(selectedPassword);
            }
        }

        // ── Add new password entry (retains compatibility with original navigation)
        private async void OnShowPasswordClicked(object sender, EventArgs e)
        {
            //string base64Key = await SecureStorage.GetAsync("aesKey");
            //string base64IV = await SecureStorage.GetAsync("aesIV");

            //byte[] _aesKey = Convert.FromBase64String(base64Key);
            //byte[] _aesIv = Convert.FromBase64String(base64IV);

            //var button = (Button)sender;
            //var password = button.CommandParameter as Password;

            //if (password == null) return;

            //// Toggle visibility
            //password.IsPasswordVisible = !password.IsPasswordVisible;

            //// Decrypt the password if needed and we haven't already done so
            //if (password.IsPasswordVisible && string.IsNullOrEmpty(password.DecryptedPassword))
            //{
            //    password.DecryptedPassword = AESEncryptionService.DecryptStringAES(
            //        password.EncryptedPassword, _aesKey, _aesIv
            //    );
            //}
            var btn = (Button)sender;
            var pwd = btn.CommandParameter as Password;
            if (pwd == null) return;

            pwd.IsPasswordVisible = !pwd.IsPasswordVisible;

            if (pwd.IsPasswordVisible)
            {

                if (string.IsNullOrEmpty(pwd.EncryptedPassword))
                {
                    pwd.DecryptedPassword = "";
                    return;
                }

                try
                {
                    pwd.DecryptedPassword = AESEncryptionService.DecryptStringAES(
                        pwd.EncryptedPassword,
                        _aesKey,
                        _aesIv);
                }
                catch (CryptographicException)
                {
                    // 解密失败（格式不对／padding 错误），给个提示或显示空
                    pwd.DecryptedPassword = "";
                }
            }
        }

        private async void OnAddClicked(object sender, EventArgs e)
            => await Navigation.PushModalAsync(new AddPasswordPage());

        // ── Navigate to Profile page
        private async void OnProfileClicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new ProfilePage());
    }
}
