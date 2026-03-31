using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public string Get()
    {
        return "Product API working";
    }

    [HttpPost]
public IActionResult Create(Product product)
{
    return Ok(product);
}
}