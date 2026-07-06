using BibliotecaApp.Domain.Entities;

namespace BibliotecaApp.Domain.Interfaces;

public interface IUsuarioRolRepository
{
    void Agregar(UsuarioRol usuarioRol);
    List<UsuarioRol> ObtenerPorUsuario(int usuarioId);
}