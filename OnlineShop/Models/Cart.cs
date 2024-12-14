using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static OnlineShop.Models.CartProducts;

namespace OnlineShop.Models
{
    public class Cart
    {
        public Cart()
        {
            IsActive = true;
        }

        [Key]
        public int CartId { get; set; }

        //foreign key
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<CartProduct>? CartProducts { get; set; }

        public bool IsActive { get; set; }
    }
}
