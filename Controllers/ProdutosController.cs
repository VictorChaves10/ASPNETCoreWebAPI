using ASP.NETCore_WebAPI.Domain;
using ASP.NETCore_WebAPI.DTOs.Produtos;
using ASP.NETCore_WebAPI.Pagination;
using ASP.NETCore_WebAPI.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using X.PagedList;


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
    //[Authorize(Policy="UserOnly")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
    {
        var produtos = await _repository.ProdutoRepository.GetAllAsync();

        if (produtos is null)
            return NotFound();

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters parameters)
    {
        var produtos = await _repository.ProdutoRepository.GetProdutosAsync(parameters);
        return ObterProdutos(produtos);  
    }

    [HttpGet("filter/preco/pagination/")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutoFiltroPreco filtroPreco)
    {
        var produtos = await _repository.ProdutoRepository
                                        .GetProdutosFiltroPrecoAsync(filtroPreco);
        return ObterProdutos(produtos);
    }

    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> Get(int id)
    {
        var produto = await _repository.ProdutoRepository.GetAsync(p => p.Id == id);

        if (produto is null)
            return NotFound();

        var produtoDto = _mapper.Map<ProdutoDTO>(produto);
        return Ok(produtoDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
    {
        if (produtoDto is null)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var newProduto = _repository.ProdutoRepository.Create(produto);
        await _repository.CommitAsync();

        var newProdutoDto = _mapper.Map<ProdutoDTO>(newProduto);

        return new CreatedAtRouteResult("ObterProduto",
            new { id = newProdutoDto.Id }, newProdutoDto);
    }

    [HttpPatch("{id}/UpdatePartial")]
    public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id,
        JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
    {
        if(patchProdutoDTO is null || id <= 0)
            return BadRequest();

        var produto = await _repository.ProdutoRepository.GetAsync(x => x.Id == id);

        if (produto is null)
            return NotFound();

        var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

        patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);
        if(!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            return BadRequest(ModelState);

        _mapper.Map(produtoUpdateRequest, produto);
        _repository.ProdutoRepository.Update(produto);
        await _repository.CommitAsync();

        return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Put(int id, ProdutoDTO produtoDto)
    {
        if (id != produtoDto.Id)
            return BadRequest();

        var produto = _mapper.Map<Produto>(produtoDto);

        var produtoAtualizado = _repository.ProdutoRepository.Update(produto);
        await _repository.CommitAsync();

        var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
        return Ok(produtoAtualizadoDto);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ProdutoDTO>> Delete(int id)
    {
        var produto = await _repository.ProdutoRepository.GetAsync(p => p.Id == id);

        if (produto is null)
            return NotFound();

       var produtoDeletado =  _repository.ProdutoRepository.Delete(produto);
        await _repository.CommitAsync();

        var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);
        return Ok(produtoDeletadoDto);
    }

    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(IPagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.Count,
            produtos.PageSize,
            produtos.PageCount,
            produtos.TotalItemCount,
            produtos.HasNextPage,
            produtos.HasPreviousPage
        };


        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }

}
