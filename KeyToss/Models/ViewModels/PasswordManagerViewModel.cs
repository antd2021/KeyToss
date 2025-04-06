using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyToss.Models.ViewModels
{
        public partial class PasswordListViewModel : ObservableObject
        {
            [ObservableProperty]
            private ObservableCollection<Password> passwords = new();
        }
    
}
