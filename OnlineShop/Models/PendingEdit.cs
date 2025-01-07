using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models
{
    public class PendingEdit
    {
        [Key]
        public int PendingEditId { get; set; }

        public int ProductId { get; set; }
        public int OriginalProductId { get; set; }
        public int EditedProductId { get; set; }

        [ForeignKey("OriginalProductId")]
        public Product OriginalProduct { get; set; }

        [ForeignKey("EditedProductId")]
        public Product EditedProduct { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public DateTime CreatedDate { get; set; }
    }



}
