using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Company.Models
{
    [Table("Suppliers")]
    public class Suppliers
    {

        [Key]
        public int ID { get; set; }
        [Display(Name = "Nazwa firmy: ")]
        public string NameCompany { get; set; }
        
        public int GameID { get; set; }
        [Display(Name = "ilość dostarczonych sztuk ")]
        public double quantity { get; set; }

        [ForeignKey("GameID")]
        public virtual Game game { get; set; }




    }
}