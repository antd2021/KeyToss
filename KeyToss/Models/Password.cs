using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyToss.Models
{
    class Password
    {
        public int PasswordId { get; set; }
        public string WebsiteName { get; set; }
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public string UserId { get; set; }

    }
}
