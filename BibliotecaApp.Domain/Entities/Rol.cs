namespace  BibliotecaApp.Domain.Entities;

public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public List<UsuarioRol> UsuarioRoles { get; set; } = new();
    public List<RolPermiso> RolPermisos { get; set; } = new();
}