using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerShopApi.Models
{
    public class ComputerComponent
    {
        [Key, Column(Order = 0)]
        public Guid ComputerId { get; set; }

        [ForeignKey("ComputerId")]
        public Computers Computer { get; set; }

        [Key, Column(Order = 1)]
        public int ComponentId { get; set; }

        [ForeignKey("ComponentId")]
        public Component Component { get; set; }
    }
}
