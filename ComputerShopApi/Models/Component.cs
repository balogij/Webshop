using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerShopApi.Models
{
    public class Component
    {
        [Key]
        public int ComponentId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigációs tulajdonság a kapcsolótáblához (Many-to-Many a számítógépekkel)
        public ICollection<ComputerComponent> ComputerComponents { get; set; } = new List<ComputerComponent>();
    }
}
