namespace ComputerShopApi.Dtos
{
    // Kategória létrehozásához
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }

    // Operációs Rendszer létrehozásához
    public class CreateOsystemDto
    {
        public string Name { get; set; }
        public int Version { get; set; }
    }

    // Alkatrész létrehozásához
    public class CreateComponentDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }

    // Számítógép konfiguráció létrehozásához
    public class CreateComputerDto
    {
        public string Brand { get; set; }
        public string Type { get; set; }
        public double Display { get; set; }
        public int Memory { get; set; }
        public Guid OsId { get; set; }
        public int Price { get; set; }
        public List<int> ComponentIds { get; set; } = new List<int>(); // A hozzá tartozó alkatrészek ID-i
    }
}
