using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.DTOs.Categorias;
using ASP.NETCore_WebAPI.Filters;
using ASP.NETCore_WebAPI.Pagination;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace ASP.NETCore_WebAPI.Controllers;


[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _repository;
    private readonly ILogger<CategoriasController> _logger;
    private readonly IMapper _mapper;

    public CategoriasController(IUnitOfWork unitOfWork, ILogger<CategoriasController> logger, IMapper mapper)
    {
        _repository = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }


    [HttpGet]
    [ServiceFilter(typeof(ApiLogginFilter))]
    public ActionResult<IEnumerable<CategoriaDTO>> Get()
    {
        var categorias = _repository.CategoriaRepository.GetAll();

        if (categorias is null)
            NotFound("Não foi encontrado nenhuma categoria..");

        var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDto);

    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<Categoria>> Get([FromQuery] CategoriaParameters categoriaParameters)
    {
        var categorias = _repository.CategoriaRepository.GetCategorias(categoriaParameters);

        var metadata = new
        {
            categorias.TotalCount,
            categorias.PageSize,
            categorias.CurrentPage,
            categorias.TotalPages,
            categorias.HasNext,
            categorias.HasPrevious
        };

        Response.Headers.Append("x-pagination", JsonConvert.SerializeObject(metadata));
     
        var categoriasDTo = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        return Ok(categoriasDTo);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> Get(int id)
    {
        var categoria = _repository.CategoriaRepository.Get(c => c.Id == id);

        if (categoria is null)
        {
            _logger.LogWarning("Dados inválidos..");
            return BadRequest("Dados inválidos.");
        }

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
        return Ok(categoriaDto);
    }

    [HttpPost]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning("Dados inválidos..");
            return BadRequest();
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var novaCategoria = _repository.CategoriaRepository.Create(categoria);
        _repository.Commit();

        var novaCategoriaDTO = _mapper.Map<CategoriaDTO>(novaCategoria);

        return new CreatedAtRouteResult("ObterCategoria",
             new { id = novaCategoriaDTO.Id }, novaCategoriaDTO);
    }

    [HttpPut("{id:int}")]
    public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
    {

        if (categoriaDto.Id != id)
            return BadRequest();

        var categoria = _mapper.Map<Categoria>(categoriaDto);
        
        var categoriaAtualizada = _repository.CategoriaRepository.Update(categoria);
        _repository.Commit();

        var categoriaAtualizadaDto = _mapper.Map<CategoriaDTO>(categoriaAtualizada);            

        return Ok(categoriaAtualizadaDto);
    }

    [HttpDelete]
    public ActionResult<CategoriaDTO> Delete(int id)
    {
        var categoria = _repository.CategoriaRepository.Get(c => c.Id == id);

        if (categoria is null)
            return NotFound();

        var categoriaDeletada = _repository.CategoriaRepository.Delete(categoria);
        _repository.Commit();

        var categoriaDeletadaDto = _mapper.Map<CategoriaDTO>(categoriaDeletada);

        return Ok(categoriaDeletadaDto);

    }

}
