using System.ComponentModel.DataAnnotations;

namespace ASP.NETCore_WebAPI.DTOs.Categorias
{
    public class CategoriaDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Nome { get; set; }

        [StringLength(200)]
        public string ImagemUrl { get; set; }
    }
}
