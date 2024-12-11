    using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(AppDbContext context) : base(context) { }
        public async Task<PagedList<Categoria>> GetCategoriasAsync(CategoriasParameters categoriasParams)
        {
            var categorias = await GetAllAsync();
            var categoriasOrdenadas = categorias.OrderBy(p => p.CategoriaId).AsQueryable();
            var resultados = PagedList<Categoria>.ToPagedList(categoriasOrdenadas, categoriasParams.PageNumber,
                categoriasParams.PageSize);

            return resultados;
        }
        public async Task<PagedList<Categoria>> GetCategoriasFiltroNomeAsync(CategoriasFiltroNome categoriasParams)
        {
            var categorias = await GetAllAsync();

            if (!string.IsNullOrEmpty(categoriasParams.Nome))
            {
                categorias = categorias.Where(c => c.Nome.Contains(categoriasParams.Nome));
            }

            var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categorias.AsQueryable(),
                                       categoriasParams.PageNumber, categoriasParams.PageSize);

            return categoriasFiltradas;
        }
    }
}
