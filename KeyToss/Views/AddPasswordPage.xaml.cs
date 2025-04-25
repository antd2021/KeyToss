using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;               // �� using Newtonsoft.Json;
using KeyToss.Models;
using KeyToss.Services;
using Microsoft.Maui.Storage;

namespace KeyToss.Views
{
    public partial class AddPasswordPage : ContentPage
    {

        public AddPasswordPage()
        {
            InitializeComponent();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // 1) �õ�ǰ�û���
            var username = await SecureStorage.GetAsync("username");
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Error", "No logged in user.", "OK");
                return;
            }

            // 2) ������
            var site = WebsiteEntry.Text?.Trim();
            var pwd = PasswordEntry.Text;
            if (string.IsNullOrWhiteSpace(site) || pwd != ConfirmEntry.Text)
            {
                await DisplayAlert("Error", "Please fill correctly.", "OK");
                return;
            }

            // 3) AES ����
            // 3) AES ����
            var b64Key = await SecureStorage.GetAsync("aesKey");
            var b64Iv = await SecureStorage.GetAsync("aesIV");
            var aesKey = Convert.FromBase64String(b64Key);
            var aesIv = Convert.FromBase64String(b64Iv);
            var encrypted = AESEncryptionService.EncryptStringAES(pwd, aesKey, aesIv);

            // 4) �� SecureStorage ������ JSON �б�
            string key = $"pwlist_{username}";
            var json = await SecureStorage.GetAsync(key) ?? "[]";
            var list = JsonSerializer.Deserialize<List<Password>>(json)!;

            // 5) �������Ŀ
            int nextId = list.Any()
                ? list.Max(p => p.PasswordId) + 1
                : 1;

            list.Add(new Password
            {
                PasswordId = nextId,
                WebsiteName = site!,
                EncryptedPassword = encrypted,
                Username = username,
                LastModified = DateTime.Now,
                ExpirationDate = DateTime.Now  // ��ӽ����϶�
            });

            // 6) ���л��� SecureStorage
            var newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(key, newJson);

            await Navigation.PopModalAsync();
        }

        private void OnGenerateClicked(object sender, EventArgs e)
        {
            string password = new PasswordGeneratorService().GeneratePassword();
            PasswordEntry.Text = password;
            ConfirmEntry.Text = password;
        }

        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();
    }
}
