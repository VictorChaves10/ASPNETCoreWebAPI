namespace ASP.NETCore_WebAPI.Pagination
{
    public class ProdutoFiltroPreco : QueryStringParameter
    {
        public decimal? Preco { get; set; }
        public string PrecoCriterio { get; set; }
    }
}
