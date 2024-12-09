using APICatalogo.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Repositories;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoRepository _repository;
        public ProdutosController(IProdutoRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> GetProdutos()
        {
            var produtos = _repository.GetProdutos().ToList();
            if(produtos is null)
            {
                return NotFound();
            }
            return Ok(produtos);
        }
        [HttpGet("{id:int}", Name="ObterProduto")]
        public ActionResult<Produto> GetId(int id)
        {
            var produto = _repository.GetProduto(id);
            if (produto is null)
                return NotFound("Produto não encontrado...");
            
            return Ok(produto);
        }
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
                return BadRequest();
            
            var novoProduto = _repository.Create(produto);



            return new CreatedAtRouteResult("ObterProduto",
                new { id = novoProduto }, novoProduto);
        }
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            
            if (id != produto.ProdutoId)
                return BadRequest();

           bool atualizado = _repository.Update(produto);
            if (atualizado)
            {
                return Ok(produto);
            }
            else
            {
                return StatusCode(500, $"Falha ao atualizar o produto de id ={id}");
            }               
            
        }
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            bool produto = _repository.Delete(id);
            if(produto)
            {
                return Ok($"Produto com id={id} deletado.");
            }
            
            return StatusCode(500, $"Falha ao atualizar o excluir o produto de id{id}");
        }
    }
}
