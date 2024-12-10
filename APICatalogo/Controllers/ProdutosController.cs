using APICatalogo.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Repositories.Interfaces;
using APICatalogo.DTOs;
using AutoMapper;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        public ProdutosController(IUnitOfWork uof, IMapper mapper)
        {
            _mapper = mapper;
            _uof = uof;
        }
        [HttpGet("produtos/{id}")]
        public ActionResult <IEnumerable<ProdutoDTO>> GetProdutosCategoria(int id)
        {
            var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);
            if (produtos is null)
                return NotFound();

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }
        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutos()
        {
            var produtos = _uof.ProdutoRepository.GetAll();
            if(produtos is null)
            {
                return NotFound();
            }
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }
        [HttpGet("{id:int}", Name="ObterProduto")]
        public ActionResult<ProdutoDTO> GetId(int id)
        {
            var produto = _uof.ProdutoRepository.Get(c => c.ProdutoId == id);
            if (produto is null)
                return NotFound("Produto não encontrado...");

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);
            return Ok(produtoDto);
        }
        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);
            var novoProduto = _uof.ProdutoRepository.Create(produto);
            _uof.Commit();

            var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);
            
            return new CreatedAtRouteResult("ObterProduto",
                new { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
        }
        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDto)
        {
            
            if (id != produtoDto.ProdutoId)
                return BadRequest();

            var produto = _mapper.Map<Produto>(produtoDto);
            var atualizado = _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(atualizado);

            return Ok(produtoAtualizadoDto);            
        }
        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
            if(produto is null)            
                return NotFound();

            
            
            var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();

            var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDto);
                        
        }
    }
}
