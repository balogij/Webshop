using ComputerShopApi.Dtos;
using ComputerShopApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComputerShopController : ControllerBase
    {
        private readonly ComputerShopDbContext _context;

        public ComputerShopController(ComputerShopDbContext context)
        {
            _context = context;
        }

        // --- GET VÉGPONTOK ---

        // 1. Összes kategória lekérése
        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        // 2. Összes operációs rendszer lekérése
        [HttpGet("osystems")]
        public async Task<IActionResult> GetOsystems()
        {
            var osystems = await _context.Osystems.ToListAsync();
            return Ok(osystems);
        }

        // 3. Összes alkatrész lekérése
        [HttpGet("components")]
        public async Task<IActionResult> GetComponents()
        {
            var components = await _context.Components.ToListAsync();
            return Ok(components);
        }

        // 4. Mentett számítógép konfigurációk lekérése (opcionális, a konfigok megjelenítéséhez)
        [HttpGet("computers")]
        public async Task<IActionResult> GetComputers()
        {
            var computers = await _context.Computers
                .Include(c => c.Osystem)
                .Select(c => new
                {
                    c.Id,
                    c.Brand,
                    c.Type,
                    c.Display,
                    c.Memory,
                    c.CreatedTime,
                    c.Price,
                    OsName = c.Osystem != null ? c.Osystem.Name : "Nincs OS",
                    Components = _context.ComputerComponents
                        .Where(cc => cc.ComputerId == c.Id)
                        .Join(_context.Components,
                              cc => cc.ComponentId,
                              comp => comp.ComponentId,
                              (cc, comp) => new { comp.ComponentId, comp.Name, comp.Price })
                        .ToList()
                })
                .ToListAsync();

            return Ok(computers);
        }


        // --- POST VÉGPONTOK ---

        // 5. Kategória felvétele (POST)
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                Slug = dto.Slug
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kategória sikeresen létrehozva!", categoryId = category.CategoryId });
        }

        // 6. Operációs Rendszer felvétele (POST)
        [HttpPost("osystems")]
        public async Task<IActionResult> CreateOsystem([FromBody] CreateOsystemDto dto)
        {
            var osystem = new Osystem
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Version = dto.Version,
                RegisterTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            _context.Osystems.Add(osystem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Operációs rendszer sikeresen létrehozva!", osId = osystem.Id });
        }

        // 7. Alkatrész felvétele (POST)
        [HttpPost("components")]
        public async Task<IActionResult> CreateComponent([FromBody] CreateComponentDto dto)
        {
            // Ellenőrizzük, hogy létezik-e a kategória
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == dto.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "A megadott kategória nem létezik!" });
            }

            var component = new Component
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                CreatedAt = DateTime.Now
            };

            _context.Components.Add(component);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Alkatrész sikeresen létrehozva!", componentId = component.ComponentId });
        }

        // 8. Számítógép Konfiguráció felvétele alkatrészekkel (POST)
        [HttpPost("computers")]
        public async Task<IActionResult> CreateComputer([FromBody] CreateComputerDto dto)
        {
            // Ellenőrizzük, hogy létezik-e az OS
            var osExists = await _context.Osystems.AnyAsync(o => o.Id == dto.OsId);
            if (!osExists)
            {
                return BadRequest(new { message = "A megadott operációs rendszer nem létezik!" });
            }

            var computer = new Computers
            {
                Id = Guid.NewGuid(),
                Brand = dto.Brand,
                Type = dto.Type,
                Display = dto.Display,
                Memory = dto.Memory,
                CreatedTime = DateTime.Now,
                OsId = dto.OsId,
                Price = dto.Price
            };

            _context.Computers.Add(computer);
            await _context.SaveChangesAsync(); // Elmentjük, hogy legyen ComputerId-nk

            // Ha küldtünk alkatrész ID-kat is, összekötjük őket a kapcsolótáblán keresztül
            if (dto.ComponentIds != null && dto.ComponentIds.Any())
            {
                foreach (var compId in dto.ComponentIds)
                {
                    var computerComponent = new ComputerComponent
                    {
                        ComputerId = computer.Id,
                        ComponentId = compId
                    };
                    _context.ComputerComponents.Add(computerComponent);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Számítógép konfiguráció sikeresen létrehozva!", computerId = computer.Id });
        }

        // 9. Adatbázis feltöltése alapértelmezett adatokkal (Seed data POST)
        [HttpPost("seed")]
        public async Task<IActionResult> SeedDatabase()
        {
            // Ha már vannak kategóriák, ne fusson le feleslegesen
            if (await _context.Categories.AnyAsync())
            {
                return BadRequest(new { message = "Az adatbázis már tartalmaz adatokat!" });
            }

            // 1. Kategóriák létrehozása
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Processzor (CPU)", Slug = "cpu" },
                new Category { CategoryId = 2, Name = "Memória (RAM)", Slug = "ram" },
                new Category { CategoryId = 3, Name = "Videókártya (GPU)", Slug = "gpu" },
                new Category { CategoryId = 4, Name = "Alaplap (Motherboard)", Slug = "motherboard" }
            };
            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();

            // 2. Operációs rendszerek létrehozása
            var osId1 = Guid.NewGuid();
            var osId2 = Guid.NewGuid();
            var osystems = new List<Osystem>
            {
                new Osystem { Id = osId1, Name = "Windows 11 Pro", Version = 11, RegisterTime = DateTime.Now, UpdateTime = DateTime.Now },
                new Osystem { Id = osId2, Name = "Ubuntu Linux", Version = 22, RegisterTime = DateTime.Now, UpdateTime = DateTime.Now }
            };
            _context.Osystems.AddRange(osystems);
            await _context.SaveChangesAsync();

            // 3. Alkatrészek létrehozása
            var components = new List<Component>
            {
                // CPU (CategoryId: 1)
                new Component { CategoryId = 1, Name = "Intel Core i5-14600K", Price = 115000.00m, Description = "14 mag, 20 szál", ImageUrl = "https://example.com/i5.jpg", CreatedAt = DateTime.Now },
                new Component { CategoryId = 1, Name = "Intel Core i9-14900K", Price = 220000.00m, Description = "24 mag, 32 szál", ImageUrl = "https://example.com/i9.jpg", CreatedAt = DateTime.Now },
                new Component { CategoryId = 1, Name = "AMD Ryzen 7 7800X3D", Price = 160000.00m, Description = "Kiváló gamer processzor", ImageUrl = "https://example.com/ryzen7.jpg", CreatedAt = DateTime.Now },
                
                // RAM (CategoryId: 2)
                new Component { CategoryId = 2, Name = "16 GB DDR5 5600MHz", Price = 35000.00m, Description = "Kétcsatornás memória", ImageUrl = "https://example.com/ram16.jpg", CreatedAt = DateTime.Now },
                new Component { CategoryId = 2, Name = "32 GB DDR5 6000MHz", Price = 65000.00m, Description = "Nagy teljesítményű RAM", ImageUrl = "https://example.com/ram32.jpg", CreatedAt = DateTime.Now },

                // GPU (CategoryId: 3)
                new Component { CategoryId = 3, Name = "NVIDIA GeForce RTX 4070", Price = 250000.00m, Description = "12GB GDDR6X", ImageUrl = "https://example.com/rtx4070.jpg", CreatedAt = DateTime.Now },
                new Component { CategoryId = 3, Name = "NVIDIA GeForce RTX 4090", Price = 65000.00m, Description = "Csúcskategóriás videókártya", ImageUrl = "https://example.com/rtx4090.jpg", CreatedAt = DateTime.Now }
            };
            _context.Components.AddRange(components);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Az adatbázis sikeresen feltöltve tesztadatokkal!" });
        }
    }
}