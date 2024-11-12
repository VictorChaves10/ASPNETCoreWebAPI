using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NETCore_WebAPI.DTOs.Produtos
{
    public class ProdutoDTO
    {
        public int Id { get; set; }

        public int CategoriaId { get; set; }

        [Required]
        [StringLength(200)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string Desricao { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        public string ImagemUrl { get; set; }
    }
}
