using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.DTOs.Categorias;
using ASP.NETCore_WebAPI.DTOs.Produtos;
using AutoMapper;

namespace ASP.NETCore_WebAPI.DTOs.Mappings
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile() 
        {
            
            CreateMap<Categoria, CategoriaDTO>().ReverseMap();

            CreateMap<Produto, ProdutoDTO>().ReverseMap();
            CreateMap<Produto, ProdutoDTOUpdateRequest>().ReverseMap();
            CreateMap<Produto, ProdutoDTOUpdateResponse>().ReverseMap();
                 
        }
    }
}
