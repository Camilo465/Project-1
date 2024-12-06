using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepository _repository;
        public CategoriasController(ICategoriaRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        [ServiceFilter(typeof(ApiLogginFilter))]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            var categoria = _repository.GetCategorias();
            return Ok(categoria);
        }
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> GetId(int id)
        {
            var categoria = _repository.GetCategoria(id);
            if (categoria is null)
            {
                return NotFound($"Categoria com id={id} não encontrado");
            }
            return Ok(categoria);
        }      

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria is null)
                return BadRequest("Dados inválidos");

            var categoriaCriada = _repository.Create(categoria);
            

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = categoriaCriada.CategoriaId }, categoriaCriada);
        }
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId)
                return BadRequest("Dados inválidos.");

            _repository.Update(categoria);

            return Ok(categoria);
        }
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categoria = _repository.GetCategoria(id);
            if (categoria is null)
                return NotFound($"Categoria com id = {id} não encontrada");

            var categoriaExcluir = _repository.Delete(id);
            return Ok(categoriaExcluir);
        }
    }
}
