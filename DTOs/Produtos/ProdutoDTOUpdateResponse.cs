using ASP.NETCore_WebAPI.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ASP.NETCore_WebAPI.DTOs.Produtos
{
    public class ProdutoDTOUpdateResponse
    {
        public int Id { get; set; }

        public int CategoriaId { get; set; }

        public string Nome { get; set; }

        public string Desricao { get; set; }

        public decimal Preco { get; set; }

        public string ImagemUrl { get; set; }

        public float Estoque { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}
