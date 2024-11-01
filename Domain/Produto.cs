using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ASP.NETCore_WebAPI.Domain;

public class Produto
{
    public int Id { get; set; }

    public int CategoriaId { get; set; }

	[Required]
	[StringLength(200)]
    public string Nome { get; set; }

	[StringLength(500)]
	public string Desricao { get; set; }

	[Required]
	[Column(TypeName ="decimal(10,2)")]
	public decimal Preco { get; set; }

    public string ImagemUrl { get; set; }

	[Required]
	public float Estoque { get; set; }

    public DateTime DataCadastro { get; set; }

	[JsonIgnore]
    public virtual Categoria Categoria { get; set; }

}
