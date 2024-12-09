using System.ComponentModel.DataAnnotations;
using static OnlineShop.Models.CartProducts;

namespace OnlineShop.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        //foreign key
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<CartProduct>? CartProducts { get; set; }

    }
}
