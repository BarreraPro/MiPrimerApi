# 🛠️ Taller Práctico: Creación y Consumo de APIs con ASP.NET Core

## 📌 ¿Qué es una API?

Una API (Application Programming Interface) es un intermediario que permite que dos aplicaciones se comuniquen.  
Por ejemplo: cuando abres Instagram y ves tu feed, estás consumiendo una API del servidor de Instagram.

## 🚀 ¿Qué haremos?

1. Crear una API propia con ASP.NET Core.
2. Organizar el código con buenas prácticas (Clean Code).
3. Conectarla a una base de datos.
4. Consumir una API externa desde nuestra app.

---

## PARTE 1: Crear tu propia API RESTful

### ✅ Paso 1: Crear el Proyecto

- Abrimos Visual Studio.
- Seleccionamos **ASP.NET Core Web API**.
- Marcamos “Controllers” (esto genera controladores por defecto).
- El proyecto se llama `MiPrimerAPI`.

### ✅ Paso 2: Organización de Carpetas - Clean Code

| Carpeta       | Contenido                    | Función                                                                 |
| ------------- | ---------------------------- | ----------------------------------------------------------------------- |
| Controllers/  | Controladores                | Manejan las solicitudes HTTP (GET, POST, PUT, DELETE).                  |
| DTOs/         | Data Transfer Objects        | Se usan para enviar o recibir datos sin exponer entidades directamente. |
| Models/       | Modelos de datos             | Representan las tablas de la base de datos.                             |
| Repositories/ | Interfaces y lógica de datos | Separan el acceso a datos del resto de la lógica.                       |
| Services/     | Servicios                    | Lógica de negocio y validaciones.                                       |
| Data/         | DbContext y configuración    | Define la conexión a la base de datos.                                  |

### ✅ Paso 3: Crear el Modelo

```csharp
// Models/Product.cs
public class Product
{
    public int Id { get; set; }        // Identificador único
    public string Name { get; set; }   // Nombre del producto
    public decimal Price { get; set; } // Precio del producto
}
```

### ✅ Paso 4: Conexión a la Base de Datos

```csharp
// Data/AppDbContext.cs
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Product> Products { get; set; }
}
```

**appsettings.json**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=MiApiDb;Trusted_Connection=True;"
}
```

**Program.cs**

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### ✅ Paso 5: Crear Repositorio y Servicio

**IProductRepository.cs**

```csharp
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
}
```

**ProductRepository.cs**

```csharp
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();
    public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);
    public async Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}
```

**ProductService.cs**

```csharp
public class ProductService
{
    private readonly IProductRepository _repo;
    public ProductService(IProductRepository repo) => _repo = repo;

    public Task<IEnumerable<Product>> GetAllProducts() => _repo.GetAllAsync();
}
```

**Program.cs**

```csharp
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();
```

### ✅ Paso 6: Crear el Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _service;
    public ProductController(ProductService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _service.GetAllProducts();
        return Ok(products);
    }
}
```

### ✅ Paso 7: Crear la base de datos

En la consola del administrador de paquetes:

```bash
Add-Migration InitialCreate
Update-Database
```

---

## PARTE 2: Consumir una API externa de Usuarios

### 🌐 Nueva API: https://reqres.in/api/users

### ✅ Servicio para consumir usuarios

```csharp
public class ExternalApiService
{
    private readonly HttpClient _httpClient;

    public ExternalApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync("https://reqres.in/api/users?page=1");

        if (!response.IsSuccessStatusCode)
            throw new Exception("Error al consumir la API externa de usuarios");

        return await response.Content.ReadAsStringAsync();
    }
}
```

**Program.cs**

```csharp
builder.Services.AddHttpClient<ExternalApiService>();
```

### ✅ Crear el controlador externo

```csharp
[ApiController]
[Route("api/[controller]")]
public class ExternalController : ControllerBase
{
    private readonly ExternalApiService _apiService;

    public ExternalController(ExternalApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _apiService.GetUsersAsync();
        return Ok(result);
    }
}
```

**Respuesta esperada (JSON):**

```json
{
  "page": 1,
  "per_page": 6,
  "data": [
    {
      "id": 1,
      "email": "george.bluth@reqres.in",
      "first_name": "George",
      "last_name": "Bluth",
      "avatar": "https://reqres.in/img/faces/1-image.jpg"
    }
  ]
}
```

---

## 🎁 BONUS: Cómo deserializar los datos

```csharp
public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string First_Name { get; set; }
    public string Last_Name { get; set; }
    public string Avatar { get; set; }
}

public class UserResponse
{
    public List<User> Data { get; set; }
}

public async Task<List<User>> GetUsersAsync()
{
    var response = await _httpClient.GetAsync("https://reqres.in/api/users?page=1");
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    var users = JsonSerializer.Deserialize<UserResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    return users?.Data ?? new List<User>();
}
```
