using ASP.NETCore_WebAPI.Context;
using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Filters;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ASP.NETCore_WebAPI.Controllers;


[Route("[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
	private readonly IUnitOfWork _repository;
	private readonly ILogger<CategoriasController> _logger;

    public CategoriasController(IUnitOfWork unitOfWork, ILogger<CategoriasController> logger)
    {
        _repository = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
	[ServiceFilter(typeof(ApiLogginFilter))]
	public ActionResult<IEnumerable<Categoria>> Get()
	{		
		var categorias = _repository.CategoriaRepository.GetAll();
		
		return categorias is null ? NotFound("Não foi encontrado nenhuma categoria..") 
								  : Ok(categorias);
    }

	[HttpGet("{id:int}", Name ="ObterCategoria")]
	public ActionResult<Categoria> Get(int id)
	{
		var categoria = _repository.CategoriaRepository.Get(c => c.Id == id);

		if(categoria is null)
		{
			_logger.LogWarning("Dados inválidos..");
			return BadRequest("Dados inválidos.");
		}

		return Ok(categoria);
	}

	[HttpPost]
	public ActionResult Post(Categoria categoria)
	{
		if (categoria is null)
		{
            _logger.LogWarning("Dados inválidos..");
            return BadRequest();
		}

		var novaCategoria = _repository.CategoriaRepository.Create(categoria);
		_repository.Commit();

		return new CreatedAtRouteResult("ObterCategoria",
			 new { id = novaCategoria.Id }, novaCategoria);
	}

	[HttpPut("{id:int}")]
	public ActionResult<Categoria> Put(int id, Categoria categoria)
	{
		
		if (categoria.Id != id)
			return BadRequest();

		_repository.CategoriaRepository.Update(categoria);
		_repository.Commit();
		
		return Ok(categoria);
	}

	[HttpDelete]
	public ActionResult Delete(int id)
	{
		var categoria = _repository.CategoriaRepository.Get(c => c.Id == id);
		
		if(categoria is null)
			return NotFound();

		_repository.CategoriaRepository.Delete(categoria);
        _repository.Commit();

        return Ok(categoria);
	
	}


}
