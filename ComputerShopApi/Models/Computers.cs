using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerShopApi.Models
{
    public class Computers
    {
        [Key]
        [MaxLength(36)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(150)]
        public string Brand { get; set; }
        [Required]
        [MaxLength(150)]
        public string Type { get; set; }
        public double Display { get; set; }
        public int Memory { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid OsId { get; set; }
        [ForeignKey("OsId")]
        public Osystem Osystem { get; set; }
        public int Price { get; set; }
    }
}
