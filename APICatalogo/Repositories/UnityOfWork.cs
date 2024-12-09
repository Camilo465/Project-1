using APICatalogo.Context;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Repositories
{
    public class UnityOfWork : IUnitOfWork
    {
        private IProdutoRepository _produtoRepository;
        private ICategoriaRepository categoriaRepository;

        public AppDbContext _context;
        public UnityOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IProdutoRepository ProdutoRepository
        {
            get
            {
                return _produtoRepository = _produtoRepository ?? new ProdutoRepository(_context);
            }
        }
        public ICategoriaRepository CategoriaRepository
        {
            get
            {
                return categoriaRepository = categoriaRepository ?? new CategoriaRepository(_context);
            }
        }
        public void Commit()
        {
            _context.SaveChanges();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
