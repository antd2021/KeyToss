using System.ComponentModel;

namespace KeyToss.Models
{
    public class Password : INotifyPropertyChanged  
    {
        public int PasswordId { get; set; }
        public string WebsiteName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string EncryptedPassword { get; set; } = string.Empty;

        private bool _isPasswordVisible;

        public DateTime LastModified { get; set; }
        public DateTime ExpirationDate { get; set; }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged(nameof(IsPasswordVisible));
                OnPropertyChanged(nameof(DisplayPassword));
            }
        }

        private string _decryptedPassword;
        public string DecryptedPassword
        {
            get => _decryptedPassword;
            set
            {
                _decryptedPassword = value;
                OnPropertyChanged(nameof(DecryptedPassword));
                OnPropertyChanged(nameof(DisplayPassword));
            }
        }

        public string DisplayPassword => IsPasswordVisible ? DecryptedPassword : "••••••";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}