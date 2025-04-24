using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using KeyToss.Models;
using KeyToss.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace KeyToss.Views
{
    public partial class EditPasswordPage : ContentPage
    {
        // AES key/IV (example¡ªuse your actual secure key/iv)
        private static readonly byte[] _aesKey = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");
        private static readonly byte[] _aesIv = Encoding.UTF8.GetBytes("ABCDEF0123456789");
        
        private readonly Password _original;
        private readonly string _storageKey;

        public EditPasswordPage(Password password)
        {
            InitializeComponent();

            _original = password;

            // Build the storage key once
            var username = SecureStorage.GetAsync("username").Result;
            _storageKey = $"pwlist_{username}";

            // decrypt and prefill
            WebsiteEntry.Text = _original.WebsiteName;
            ExpirationPicker.Date = _original.ExpirationDate;

            var plain = AESEncryptionService.DecryptStringAES(
                password.EncryptedPassword, _aesKey, _aesIv);
            PasswordEntry.Text = plain;
            ConfirmEntry.Text = plain;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            string decrypted = AESEncryptionService.DecryptStringAES(
                _original.EncryptedPassword, _aesKey, _aesIv);

            PasswordEntry.Text = decrypted;
            ConfirmEntry.Text = decrypted;
        }

        // Generate a random password into both fields
        private void OnGenerateClicked(object sender, EventArgs e)
        {
            var pw = new PasswordGeneratorService().GeneratePassword();
            PasswordEntry.Text = pw;
            ConfirmEntry.Text = pw;
        }

        // Save changes: validate, encrypt, rewrite storage, pop back
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Validation
            var site = WebsiteEntry.Text?.Trim();
            var pwd = PasswordEntry.Text;
            if (string.IsNullOrWhiteSpace(site) || pwd != ConfirmEntry.Text)
            {
                await DisplayAlert("Error", "Check your inputs.", "OK");
                return;
            }

            // Encrypt new password
            var encrypted = AESEncryptionService.EncryptStringAES(pwd, _aesKey, _aesIv);

            // Load full list from storage
            var json = await SecureStorage.GetAsync(_storageKey) ?? "[]";
            var list = JsonSerializer.Deserialize<List<Password>>(json)!;

            // Find the record by ID and update
            var entry = list.FirstOrDefault(p => p.PasswordId == _original.PasswordId);
            if (entry != null)
            {
                entry.WebsiteName = site;
                entry.EncryptedPassword = encrypted;
                entry.ExpirationDate = ExpirationPicker.Date;
                entry.LastModified = DateTime.Now;
            }

            // Save back
            var newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(_storageKey, newJson);

            // Close and return
            await Navigation.PopModalAsync();
        }

        // Cancel and go back
        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();
    }
}
