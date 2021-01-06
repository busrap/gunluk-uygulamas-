using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace gunluk.Models
{
    [Table("Notlar")]
    public class Not
    {
        public int ID { get; set; }
        public string ApplicationUserID { get; internal set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
        public DateTime? SonGuncellenmeTarihi { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}