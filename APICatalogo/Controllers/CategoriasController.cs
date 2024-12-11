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
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            var categorias = await _uof.CategoriaRepository.GetAllAsync();

            if (categorias is null)
                return NotFound("Não existem categorias...");

            var categoriasDto = categorias.ToCategoriaDTOList();
            return Ok(categoriasDto);
        }
        [HttpGet("pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get([FromQuery]
                               CategoriasParameters categoriasParameters)
        {
            var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);

            return ObterCategorias(categorias);
        }
        [HttpGet("filter/nome/pagination")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradas(
                                   [FromQuery] CategoriasFiltroNome categoriasFiltro)
        {
            var categoriasFiltradas = await _uof.CategoriaRepository
                                         .GetCategoriasFiltroNomeAsync(categoriasFiltro);

            return ObterCategorias(categoriasFiltradas);

        }
        private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(PagedList<Categoria> categorias)
        {
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
        public async Task<ActionResult<CategoriaDTO>> GetId(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c=> c.CategoriaId == id);
            if (categoria is null)
            {
                return NotFound($"Categoria com id={id} não encontrado");
            }

            var categoriaDto = categoria.ToCategoriaDTO();

            return Ok(categoriaDto);
        }      

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
                return BadRequest("Dados inválidos");
            
            var categoria = categoriaDto.ToCategoria();
            
            var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
            await _uof.CommitAsync();

            var novaCategoriaDto = categoriaCriada.ToCategoriaDTO();

            return new CreatedAtRouteResult("ObterCategoria",
                new { id = novaCategoriaDto.CategoriaId }, novaCategoriaDto);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
                return BadRequest("Dados inválidos.");

            var categoria = categoriaDto.ToCategoria();

            var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
            await _uof.CommitAsync();

            var novaCategoriaDto = categoriaAtualizada.ToCategoriaDTO();

            return Ok(novaCategoriaDto);
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);
            if (categoria is null)
                return NotFound($"Categoria com id = {id} não encontrada");

            var categoriaExcluir = _uof.CategoriaRepository.Delete(categoria);
            await _uof.CommitAsync();

            var categoriaExcluidaDto = categoriaExcluir.ToCategoriaDTO();
            return Ok(categoriaExcluidaDto);
        }
    }
}
