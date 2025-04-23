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
        // AES �ӽ���ʾ�� key/iv
        static readonly byte[] _aesKey = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");
        static readonly byte[] _aesIv = Encoding.UTF8.GetBytes("ABCDEF0123456789");

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
            var encrypted = AESEncryptionService
                .EncryptStringAES(pwd, _aesKey, _aesIv);

            // 4) �� SecureStorage ������ JSON �б�
            string key = $"pwlist_{username}";
            var json = await SecureStorage.GetAsync(key) ?? "[]";
            var list = JsonSerializer.Deserialize<List<Password>>(json)!;

            // 5) �������Ŀ
            list.Add(new Password
            {
                WebsiteName = site,
                EncryptedPassword = encrypted,
                Username = username
            });

            // 6) ���л��� SecureStorage
            var newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(key, newJson);

            // 7) �ر�
            await Navigation.PopModalAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();
    }
}
