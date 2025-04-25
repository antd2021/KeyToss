using System.ComponentModel;

namespace KeyToss.Models
{
    public class Password : INotifyPropertyChanged  
    {
        public string WebsiteName { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }

        private bool _isPasswordVisible;
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