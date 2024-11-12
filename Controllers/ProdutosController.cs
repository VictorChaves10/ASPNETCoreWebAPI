using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.DTOs.Produtos;
using ASP.NETCore_WebAPI.Pagination;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace ASP.NETCore_WebAPI.Controllers;



[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{

    private readonly IUnitOfWork _repository;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = unitOfWork;
        _mapper = mapper;
    }


    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> Get()
    {
        var produtos = _repository.ProdutoRepository.GetAll();

        if (produtos is null)
            return NotFound();

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    [HttpGet("pagination")]
    public ActionResult<IEnumerable<ProdutoDTO>> Get([FromQuery] ProdutosParameters parameters)
    {
        var produtos = _repository.ProdutoRepository.GetProdutos(parameters);

        var metadata = new
        {
            produtos.TotalCount,
            produtos.PageSize,
            produtos.CurrentPage,
            produtos.TotalPages,
            produtos.HasNext,
            produtos.HasPrevious,
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    
    }


    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult<ProdutoDTO> Get(int id)
    {
        var produto = _repository.ProdutoRepository.Get(p => p.Id == id);

        if (produto is null)
            return NotFound();

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);

        return Ok(produtoDto);
    }

    [HttpPost]
    public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var newProduto = _repository.ProdutoRepository.Create(produto);
        _repository.Commit();

        var newProdutoDto = _mapper.Map<ProdutoDTO>(newProduto);

        return new CreatedAtRouteResult("ObterProduto",
            new { id = newProdutoDto.Id }, newProdutoDto);
    }

    [HttpPatch("{id}/UpdatePartial")]
    public ActionResult<ProdutoDTOUpdateResponse> Patch(int id,
        JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if(patchProdutoDTO is null || id <= 0)
            return BadRequest();

        var produto = _repository.ProdutoRepository.Get(x => x.Id == id);

        if (produto is null)
            return NotFound();

        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

        if(!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(produtoUpdateRequest, produto);
        _repository.ProdutoRepository.Update(produto);
        _repository.Commit();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));

    }


    [HttpPut("{id:int}")]
    public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.Id)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _repository.ProdutoRepository.Update(produto);
        _repository.Commit();

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);

        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<ProdutoDTO> Delete(int id)
    {
        var produto = _repository.ProdutoRepository.Get(p => p.Id == id);

        if (produto is null)
            return NotFound();

       var produtoDeletado =  _repository.ProdutoRepository.Delete(produto);
        _repository.Commit();

        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

        return Ok(produtoDeletadoDto);
    }

}
