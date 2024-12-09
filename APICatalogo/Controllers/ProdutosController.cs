using APICatalogo.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        
        private readonly IProdutoRepository _produtoRepository;
        public ProdutosController(IProdutoRepository produtoRepository)
        {
            
            _produtoRepository = produtoRepository;
        }
        [HttpGet("produtos/{id}")]
        public ActionResult <IEnumerable<Produto>> GetProdutosCategoria(int id)
        {
            var produtos = _produtoRepository.GetProdutosPorCategoria(id);
            if (produtos is null)
                return NotFound();

            return Ok(produtos);
        }
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> GetProdutos()
        {
            var produtos = _produtoRepository.GetAll();
            if(produtos is null)
            {
                return NotFound();
            }
            return Ok(produtos);
        }
        [HttpGet("{id:int}", Name="ObterProduto")]
        public ActionResult<Produto> GetId(int id)
        {
            var produto = _produtoRepository.Get(c => c.ProdutoId == id);
            if (produto is null)
                return NotFound("Produto não encontrado...");
            
            return Ok(produto);
        }
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
                return BadRequest();
            
            var novoProduto = _produtoRepository.Create(produto);

            return new CreatedAtRouteResult("ObterProduto",
                new { id = novoProduto }, novoProduto);
        }
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            
            if (id != produto.ProdutoId)
                return BadRequest();

           var atualizado = _produtoRepository.Update(produto);

            return Ok(atualizado);            
        }
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _produtoRepository.Get(p => p.ProdutoId == id);
            if(produto is null)            
                return NotFound();
            
            var produtoDeletado = _produtoRepository.Delete(produto);
            return Ok(produtoDeletado);
                        
        }
    }
}
