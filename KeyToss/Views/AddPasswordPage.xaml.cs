using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;               // 或 using Newtonsoft.Json;
using KeyToss.Models;
using KeyToss.Services;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;

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
            var expirationDate = ExpirationDatePicker.Date;
            var siteUsername = UsernameEntry.Text.Trim();

            // 5) 添加新条目
            list.Add(new Password
            {
                WebsiteName = site,
                EncryptedPassword = encrypted,
                Username = username,
                SiteUsername = siteUsername,
                ExpirationDate = expirationDate
            });

            // 6) 序列化回 SecureStorage
            var newJson = JsonSerializer.Serialize(list);
            await SecureStorage.SetAsync(key, newJson);


            var notifyTime = expirationDate.AddDays(-3);
            var notification = new NotificationRequest
            {
                NotificationId = new Random().Next(1000, 9999),
                Title = "Password Expiration Reminder",
                Description = $"Your password for {site} expires in 3 days.",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(5) //Set to notify immediately for testing purposes
                }
            };
            await LocalNotificationCenter.Current.Show(notification);

            // 7) 关闭
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

        //Add expiration date entry and push notification when time nears expiration date
        //Add entry for username
    }
}
