using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NETCore_WebAPI.Controllers;



[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{

	private readonly IUnitOfWork _repository;

	public ProdutosController(IUnitOfWork unitOfWork) => _repository = unitOfWork;

	[HttpGet]
	public ActionResult<IEnumerable<Produto>> Get()
	{
		var produtos = _repository.ProdutoRepository.GetAll();

		return produtos is null ? NotFound() : Ok(produtos);
	}

	[HttpGet("{id:int:min(1)}", Name ="ObterProduto")]
	public ActionResult<Produto> Get(int id)
	{
		var produto = _repository.ProdutoRepository.Get(p => p.Id == id);

		return produto is null ? NotFound() : Ok(produto);
	}

	[HttpPost]
	public IActionResult Post(Produto produto)
	{	
		if (produto is null)
			return BadRequest();

		var newProduto = _repository.ProdutoRepository.Create(produto);

		_repository.Commit();

		return new CreatedAtRouteResult("ObterProduto",
			new { id = newProduto.Id }, newProduto);			
	}

	[HttpPut("{id:int}")]
	public ActionResult Put(int id, Produto produto) 
	{ 
		if(id != produto.Id)
			return BadRequest();

		_repository.ProdutoRepository.Update(produto);
        _repository.Commit();

        return Ok(produto);
	}

	[HttpDelete("{id:int}")]
	public ActionResult Delete(int id) 
	{
		var produto = _repository.ProdutoRepository.Get(p => p.Id == id);
        
		if (produto is null)
            return NotFound();

        _repository.ProdutoRepository.Delete(produto);
        _repository.Commit();

        return Ok(produto);	
	}
	
}
