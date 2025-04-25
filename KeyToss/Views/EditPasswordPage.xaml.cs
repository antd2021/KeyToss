using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using KeyToss.Models;
using KeyToss.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace KeyToss.Views
{
    public partial class EditPasswordPage : ContentPage
    {
        private byte[] _aesKey;
        private byte[] _aesIv;
        private readonly string _storageKey;
        private readonly Password _original;

        public EditPasswordPage(Password password)
        {
            InitializeComponent();
            _original = password;
            var user = SecureStorage.GetAsync("username").Result!;
            _storageKey = $"pwlist_{user}";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var b64Key = await SecureStorage.GetAsync("aesKey");
            var b64Iv = await SecureStorage.GetAsync("aesIV");
            _aesKey = Convert.FromBase64String(b64Key);
            _aesIv = Convert.FromBase64String(b64Iv);

            WebsiteEntry.Text = _original.WebsiteName;
            UsernameEntry.Text = _original.SiteUsername;
            ExpirationPicker.Date = _original.ExpirationDate;

            if (!string.IsNullOrEmpty(_original.EncryptedPassword))
            {
                try
                {
                    var plain = AESEncryptionService.DecryptStringAES(
                                  _original.EncryptedPassword,
                                  _aesKey, _aesIv);
                    PasswordEntry.Text = plain;
                    ConfirmEntry.Text = plain;
                }
                catch (CryptographicException)
                {
                    PasswordEntry.Text = "";
                    ConfirmEntry.Text = "";
                }
            }
        }

        private void OnGenerateClicked(object sender, EventArgs e)
        {
            var pw = new PasswordGeneratorService().GeneratePassword();
            PasswordEntry.Text = pw;
            ConfirmEntry.Text = pw;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            var site = WebsiteEntry.Text?.Trim();
            var pwd = PasswordEntry.Text;
            var siteUsername = UsernameEntry.Text.Trim();

            if (string.IsNullOrWhiteSpace(site) || pwd != ConfirmEntry.Text)
            {
                await DisplayAlert("Error", "Please fill correctly.", "OK");
                return;
            }

            var encrypted = AESEncryptionService.EncryptStringAES(pwd, _aesKey, _aesIv);

            var json = await SecureStorage.GetAsync(_storageKey) ?? "[]";
            var list = JsonSerializer.Deserialize<List<Password>>(json)!;

            var entry = list.FirstOrDefault(p => p.PasswordId == _original.PasswordId);
            if (entry != null)
            {
                entry.WebsiteName = site;
                entry.SiteUsername = siteUsername;
                entry.EncryptedPassword = encrypted;
                entry.ExpirationDate = ExpirationPicker.Date;
                entry.LastModified = DateTime.Now;
            }

            await SecureStorage.SetAsync(_storageKey, JsonSerializer.Serialize(list));
            await Navigation.PopModalAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();
    }
}
