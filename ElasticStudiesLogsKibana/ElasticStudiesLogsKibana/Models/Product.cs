using System.ComponentModel.DataAnnotations;

namespace ElasticStudiesLogsKibana.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Ex: "Roupas", "Calçados", "Acessórios"
        public string Brand { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Size { get; set; } = string.Empty; // Ex: "P", "M", "G", "42", "43"
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProductRequest
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "O nome deve ter entre 1 e 200 caracteres.")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "A descrição não pode ter mais de 1000 caracteres.")]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "A categoria não pode ter mais de 100 caracteres.")]
        public string Category { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "A marca não pode ter mais de 100 caracteres.")]
        public string Brand { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue, ErrorMessage = "O preço deve ser maior ou igual a zero.")]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque deve ser maior ou igual a zero.")]
        public int StockQuantity { get; set; }
        
        [StringLength(50, ErrorMessage = "O tamanho não pode ter mais de 50 caracteres.")]
        public string Size { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "A cor não pode ter mais de 50 caracteres.")]
        public string Color { get; set; } = string.Empty;
    }

    public class UpdateProductRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }
}

