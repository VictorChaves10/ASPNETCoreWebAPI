using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ASP.NETCore_WebAPI.Domain;

public class Categoria
{
    public Categoria() => Produtos = [];   

    public int Id { get; set; }

	[Required]
	[StringLength(200)]
	public string Nome { get; set; }

    public string ImagemUrl { get; set; }

    [JsonIgnore]
    public ICollection<Produto> Produtos { get; set; }
}
