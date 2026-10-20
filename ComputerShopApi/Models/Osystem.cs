using System.ComponentModel.DataAnnotations;

namespace ComputerShopApi.Models
{
    public class Osystem
    {
        [Key]
        [MaxLength(36)]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(150)]
        public string Name { get; set; }
        public int Version { get; set; }
        public DateTime RegisterTime {  get; set; }
        public DateTime UpdateTime { get; set; }

        // Navigációs tulajdonság a kapcsolódó számítógépekhez
        public ICollection<Computers> ComputerList { get; set; } = new List<Computers>();
    }
}
