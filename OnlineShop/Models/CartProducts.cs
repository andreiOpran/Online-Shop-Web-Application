
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class CartProducts
    {
        public class CartProduct
        {
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }
            public int? CartId { get; set; }
            public int? ProductId { get; set; }
            public virtual Product? Product { get; set; }
            public virtual Cart? Cart { get; set; }

            // Adaugă cantitatea produsului
            public int Quantity { get; set; } = 1; // Valoare implicită
        }
    }
}


