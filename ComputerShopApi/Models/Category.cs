using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ComputerShopApi.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; }

        // Navigációs tulajdonság a kategóriához tartozó alkatrészekhez
        public ICollection<Component> Components { get; set; } = new List<Component>();
    }
}
