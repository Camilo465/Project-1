using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        public CategoriasController(IUnitOfWork uof)
        {
            _uof = uof;
        }
        [HttpGet]
        [ServiceFilter(typeof(ApiLogginFilter))]
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            var categorias = _uof.CategoriaRepository.GetAll();
            if (categorias is null)
                return NotFound("Não existem categorias...");

            var categoriasDto = categorias.ToCategoriaDTOList();
            return Ok(categoriasDto);
        }
        [HttpGet("pagination")]
        public ActionResult<IEnumerable<CategoriaDTO>> Get([FromQuery] 
                                                CategoriasParameters categoriasParameters)
        {
            var categorias = _uof.CategoriaRepository.GetCategorias(categoriasParameters);

            var metadata = new
            {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var categoriasDto = categorias.ToCategoriaDTOList();

            return Ok(categoriasDto);

        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> GetId(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c=> c.CategoriaId == id);
            if (categoria is null)
            {
                return NotFound($"Categoria com id={id} não encontrado");
            }

            var categoriaDto = categoria.ToCategoriaDTO();

            return Ok(categoriaDto);
        }      

        [HttpPost]
        public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
                return BadRequest("Dados inválidos");
            
            var categoria = categoriaDto.ToCategoria();
            
            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            _uof.Commit();

            var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
        }
        [HttpPut("{id:int}")]
        public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
                return BadRequest("Dados inválidos.");

            var categoria = categoriaDto.ToCategoria();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
            _uof.Commit();

            var novaCategoriaDto = categoriaAtualizada.ToCategoriaDTO();

            return Ok(novaCategoriaDto);
        }
        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);
            if (categoria is null)
                return NotFound($"Categoria com id = {id} não encontrada");

            var categoriaExcluir = _uof.CategoriaRepository.Delete(categoria);
            _uof.Commit();

            var categoriaExcluidaDto = categoriaExcluir.ToCategoriaDTO();
            return Ok(categoriaExcluidaDto);
        }
    }
}
