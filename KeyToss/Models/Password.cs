using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyToss.Models
{
    public class Password
    {
        [PrimaryKey, AutoIncrement]
        public int PasswordId { get; set; }
        public string WebsiteName { get; set; }
        public string Username { get; set; }
        public string SiteUsername { get; set; }
        public string EncryptedPassword { get; set; }

        public DateTime LastModified { get; set; }
        public DateTime ExpirationDate { get; set; }

    }
}
