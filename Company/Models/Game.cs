using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Company.Models
{
    [Table("Games")]
    public class Game
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Tytuł: ")]
        public string Title { get; set; }
        [Display(Name = "Wytwórnia: ")]
        public string Plant { get; set; }
        [StringLength(500, ErrorMessage = "Opis nie moze byc dluzszy niz 500 znakow.")]
        [Display(Name = "Wymagania Sprzętowe: ")]
        public string EquipmentDesc { get; set; }
        [Display(Name = "Kategoria: ")]
        public gameKind gamekind { get; set; }

        [Display(Name = "Cena PLN/szt: ")]
        public double PricePerPieces { get; set; }
        [Display(Name = "ilość dostępnych sztuk ")]
        public double quantity { get; set; }
        [StringLength(500, ErrorMessage = "Opis nie moze byc dluzszy niz 500 znakow.")]
        [Display(Name = "Opis: ")]
        public string Description { get; set; }
        [Display(Name = "Zdjęcie produktu: ")]
        public string Image { get; set; }

        [ForeignKey("GameID")]
        public virtual ICollection<Suppliers> suppliers { get; set; }

        [ForeignKey("GameID")]
        public virtual ICollection<Orders> orders { get; set; }
        public enum gameKind
        {
           RPG,
           wyscigowa,
           strategiczna,
           przygodowa,
           edukacyjna
        }




    }
}