using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.DTOs.Categorias;
using ASP.NETCore_WebAPI.Filters;
using ASP.NETCore_WebAPI.Pagination;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;


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
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
    {
        var categorias = await _repository.CategoriaRepository.GetAllAsync();

        if (categorias is null)
            NotFound("Não foi encontrado nenhuma categoria..");

        var categoriasDto = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

        return Ok(categoriasDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<Categoria>>> Get([FromQuery] CategoriaParameters categoriaParameters)
    {
        var categorias = await _repository.CategoriaRepository.GetCategoriasAsync(categoriaParameters);
         return ObterCategorias(categorias);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<Categoria>>> Get([FromQuery] CategoriaFiltroNome categoriaFiltro)
    {
        var categorias = await _repository.CategoriaRepository
                                          .GetCategoriasFiltroNomeAsync(categoriaFiltro);

        return ObterCategorias(categorias);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> Get(int id)
    {
        var categoria = await _repository.CategoriaRepository.GetAsync(c => c.Id == id);

        if (categoria is null)
        {
            _logger.LogWarning("Dados inválidos..");
            return BadRequest("Dados inválidos.");
        }

        var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
        return Ok(categoriaDto);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
    {
        if (categoriaDto is null)
        {
            _logger.LogWarning("Dados inválidos..");
            return BadRequest();
        }

        var categoria = _mapper.Map<Categoria>(categoriaDto);

        var novaCategoria = _repository.CategoriaRepository.Create(categoria);
        await _repository.CommitAsync();

        var novaCategoriaDTO = _mapper.Map<CategoriaDTO>(novaCategoria);

        return new CreatedAtRouteResult("ObterCategoria",
             new { id = novaCategoriaDTO.Id }, novaCategoriaDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
    {

        if (categoriaDto.Id != id)
            return BadRequest();

        var categoria = _mapper.Map<Categoria>(categoriaDto);
        
        var categoriaAtualizada = _repository.CategoriaRepository.Update(categoria);
        await _repository.CommitAsync();

        var categoriaAtualizadaDto = _mapper.Map<CategoriaDTO>(categoriaAtualizada);            

        return Ok(categoriaAtualizadaDto);
    }

    [HttpDelete]
    public async Task<ActionResult<CategoriaDTO>> Delete(int id)
    {
        var categoria = await _repository.CategoriaRepository.GetAsync(c => c.Id == id);

        if (categoria is null)
            return NotFound();

        var categoriaDeletada = _repository.CategoriaRepository.Delete(categoria);
        await _repository.CommitAsync();

        var categoriaDeletadaDto = _mapper.Map<CategoriaDTO>(categoriaDeletada);
        return Ok(categoriaDeletadaDto);
    }

    private ActionResult<IEnumerable<Categoria>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };

        Response.Headers.Append("x-pagination", JsonConvert.SerializeObject(metadata));
        var categoriasDTo = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);
        return Ok(categoriasDTo);
    }
}
