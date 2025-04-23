using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;               // 或 using Newtonsoft.Json;
using KeyToss.Models;
using KeyToss.Services;
using Microsoft.Maui.Storage;

namespace KeyToss.Views
{
    public partial class AddPasswordPage : ContentPage
    {
        // AES 加解密示例 key/iv
        static readonly byte[] _aesKey = Encoding.UTF8.GetBytes("0123456789ABCDEF0123456789ABCDEF");
        static readonly byte[] _aesIv = Encoding.UTF8.GetBytes("ABCDEF0123456789");

        public AddPasswordPage()
        {
            InitializeComponent();
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // 1) 拿当前用户名
            var username = await SecureStorage.GetAsync("username");
            if (string.IsNullOrEmpty(username))
            {
                await DisplayAlert("Error", "No logged in user.", "OK");
                return;
            }

            // 2) 输入检查
            var site = WebsiteEntry.Text?.Trim();
            var pwd = PasswordEntry.Text;
            if (string.IsNullOrWhiteSpace(site) || pwd != ConfirmEntry.Text)
            {
                await DisplayAlert("Error", "Please fill correctly.", "OK");
                return;
            }

            // 3) AES 加密
            var encrypted = AESEncryptionService
                .EncryptStringAES(pwd, _aesKey, _aesIv);

            // 4) 从 SecureStorage 读现有 JSON 列表
            string key = $"pwlist_{username}";
            var json = await SecureStorage.GetAsync(key) ?? "[]";
            var list = JsonSerializer.Deserialize<List<Password>>(json)!;

            // 5) 添加新条目
            list.Add(new Password
            {
                WebsiteName = site,
                EncryptedPassword = encrypted,
                Username = username
            });

            // 6) 序列化回 SecureStorage
            var newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(key, newJson);

            // 7) 关闭
            await Navigation.PopModalAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
            => await Navigation.PopModalAsync();
    }
}
