using eCommerce.API.Models;

namespace eCommerce.API.Repositories
{
    public interface IUsuarioRepository
    {
        public List<Usuario> GetAll();
        public Usuario Get(int id);
        public void Insert(Usuario usuario);
        public void Update(Usuario usuario);
        public void Delete(int id);
    }
}
