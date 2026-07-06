using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface IUsuarioRepository
{
    Usuario? ObtenerPorId(int id);
    Usuario? ObtenerPorEmail(string email);
    bool ExisteEmail(string email);
    Usuario Agregar(Usuario usuario);
    Usuario Actualizar(Usuario usuario);
    List<Usuario> ObtenerTodos();
}