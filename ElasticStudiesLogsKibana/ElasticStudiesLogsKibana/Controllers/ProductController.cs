using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ElasticStudiesLogsKibana.Models;
using System.Linq;

namespace ElasticStudiesLogsKibana.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private static readonly List<Product> _products = new();
        private static readonly object _lock = new();

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] CreateProductRequest request)
        {
            // Gera um ID de rastreamento único para esta requisição
            var requestId = Guid.NewGuid();
            
            _logger.LogInformation(
                "Requisição POST recebida. RequestId={RequestId}, Path={Path}",
                requestId,
                HttpContext.Request.Path);

            // Validação de null do request
            if (request == null)
            {
                _logger.LogWarning("Requisição de criação de produto recebida com body nulo. RequestId={RequestId}", requestId);
                return BadRequest(new { message = "O corpo da requisição não pode ser nulo." });
            }

            // Validação do ModelState
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Requisição de criação de produto com dados inválidos. RequestId={RequestId}, Erros: {Errors}", 
                    requestId,
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            _logger.LogInformation(
                "Iniciando cadastro de produto. RequestId={RequestId}, Nome={ProductName}, Categoria={Category}, Marca={Brand}",
                requestId,
                request.Name,
                request.Category,
                request.Brand);

            try
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Category = request.Category,
                    Brand = request.Brand,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    Size = request.Size,
                    Color = request.Color,
                    CreatedAt = DateTime.UtcNow
                };

                lock (_lock)
                {
                    _products.Add(product);
                }

                _logger.LogInformation(
                    "Produto cadastrado com sucesso. RequestId={RequestId}, ProductId={ProductId}, Nome={ProductName}, Preço={Price}, Estoque={StockQuantity}, Categoria={Category}",
                    requestId,
                    product.Id,
                    product.Name,
                    product.Price,
                    product.StockQuantity,
                    product.Category);

                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao cadastrar produto. Nome={ProductName}, Categoria={Category}",
                    request.Name,
                    request.Category);
                return StatusCode(500, new { message = "Erro interno ao cadastrar produto." });
            }
        }

        [HttpGet]
        public IActionResult GetAllProducts([FromQuery] string? category = null, [FromQuery] string? brand = null)
        {
            _logger.LogInformation(
                "Listando produtos. Filtros: Categoria={Category}, Marca={Brand}",
                category ?? "Nenhum",
                brand ?? "Nenhum");

            try
            {
                lock (_lock)
                {
                    var filteredProducts = _products.AsEnumerable();

                    if (!string.IsNullOrWhiteSpace(category))
                    {
                        filteredProducts = filteredProducts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
                    }

                    if (!string.IsNullOrWhiteSpace(brand))
                    {
                        filteredProducts = filteredProducts.Where(p => p.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));
                    }

                    var result = filteredProducts.ToList();

                    _logger.LogInformation(
                        "Listagem de produtos concluída. TotalEncontrado={TotalProducts}, FiltrosAplicados={HasFilters}",
                        result.Count,
                        !string.IsNullOrWhiteSpace(category) || !string.IsNullOrWhiteSpace(brand));

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar produtos.");
                return StatusCode(500, new { message = "Erro interno ao listar produtos." });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(Guid id)
        {
            _logger.LogInformation("Buscando produto por ID. ProductId={ProductId}", id);

            try
            {
                lock (_lock)
                {
                    var product = _products.FirstOrDefault(p => p.Id == id);

                    if (product == null)
                    {
                        _logger.LogWarning("Produto não encontrado. ProductId={ProductId}", id);
                        return NotFound(new { message = "Produto não encontrado." });
                    }

                    _logger.LogInformation(
                        "Produto encontrado. ProductId={ProductId}, Nome={ProductName}, Categoria={Category}",
                        product.Id,
                        product.Name,
                        product.Category);

                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto. ProductId={ProductId}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar produto." });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
        {
            _logger.LogInformation("Iniciando atualização de produto. ProductId={ProductId}", id);

            try
            {
                lock (_lock)
                {
                    var product = _products.FirstOrDefault(p => p.Id == id);

                    if (product == null)
                    {
                        _logger.LogWarning("Tentativa de atualizar produto inexistente. ProductId={ProductId}", id);
                        return NotFound(new { message = "Produto não encontrado." });
                    }

                    var oldPrice = product.Price;
                    var oldStock = product.StockQuantity;

                    if (!string.IsNullOrWhiteSpace(request.Name))
                        product.Name = request.Name;
                    if (!string.IsNullOrWhiteSpace(request.Description))
                        product.Description = request.Description;
                    if (!string.IsNullOrWhiteSpace(request.Category))
                        product.Category = request.Category;
                    if (!string.IsNullOrWhiteSpace(request.Brand))
                        product.Brand = request.Brand;
                    if (request.Price.HasValue)
                        product.Price = request.Price.Value;
                    if (request.StockQuantity.HasValue)
                        product.StockQuantity = request.StockQuantity.Value;
                    if (!string.IsNullOrWhiteSpace(request.Size))
                        product.Size = request.Size;
                    if (!string.IsNullOrWhiteSpace(request.Color))
                        product.Color = request.Color;

                    product.UpdatedAt = DateTime.UtcNow;

                    _logger.LogInformation(
                        "Produto atualizado com sucesso. ProductId={ProductId}, Nome={ProductName}, PreçoAnterior={OldPrice}, PreçoNovo={NewPrice}, EstoqueAnterior={OldStock}, EstoqueNovo={NewStock}",
                        product.Id,
                        product.Name,
                        oldPrice,
                        product.Price,
                        oldStock,
                        product.StockQuantity);

                    return Ok(product);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar produto. ProductId={ProductId}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar produto." });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(Guid id)
        {
            _logger.LogInformation("Iniciando exclusão de produto. ProductId={ProductId}", id);

            try
            {
                lock (_lock)
                {
                    var product = _products.FirstOrDefault(p => p.Id == id);

                    if (product == null)
                    {
                        _logger.LogWarning("Tentativa de excluir produto inexistente. ProductId={ProductId}", id);
                        return NotFound(new { message = "Produto não encontrado." });
                    }

                    _products.Remove(product);

                    _logger.LogInformation(
                        "Produto excluído com sucesso. ProductId={ProductId}, Nome={ProductName}, Categoria={Category}",
                        product.Id,
                        product.Name,
                        product.Category);

                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir produto. ProductId={ProductId}", id);
                return StatusCode(500, new { message = "Erro interno ao excluir produto." });
            }
        }

        [HttpGet("stats")]
        public IActionResult GetProductStats()
        {
            _logger.LogInformation("Gerando estatísticas de produtos.");

            try
            {
                lock (_lock)
                {
                    var stats = new
                    {
                        TotalProducts = _products.Count,
                        TotalByCategory = _products
                            .GroupBy(p => p.Category)
                            .ToDictionary(g => g.Key, g => g.Count()),
                        TotalByBrand = _products
                            .GroupBy(p => p.Brand)
                            .ToDictionary(g => g.Key, g => g.Count()),
                        AveragePrice = _products.Any() ? _products.Average(p => p.Price) : 0,
                        TotalStock = _products.Sum(p => p.StockQuantity),
                        LowStockProducts = _products.Count(p => p.StockQuantity < 10)
                    };

                    _logger.LogInformation(
                        "Estatísticas geradas. TotalProdutos={TotalProducts}, PreçoMédio={AveragePrice}, EstoqueTotal={TotalStock}, ProdutosEstoqueBaixo={LowStockProducts}",
                        stats.TotalProducts,
                        stats.AveragePrice,
                        stats.TotalStock,
                        stats.LowStockProducts);

                    return Ok(stats);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar estatísticas de produtos.");
                return StatusCode(500, new { message = "Erro interno ao gerar estatísticas." });
            }
        }
    }
}

