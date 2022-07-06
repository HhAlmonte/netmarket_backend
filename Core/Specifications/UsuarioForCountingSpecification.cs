using Core.Entities;

namespace Core.Specifications
{
    public class UsuarioForCountingSpecification : BaseSpecification<Usuario>
    {
        public UsuarioForCountingSpecification(UsuarioSpecificationsParams usuarioParams)
            : base(x =>
                (string.IsNullOrEmpty(usuarioParams.Search) || x.Nombre.Contains(usuarioParams.Search)) &&
                (string.IsNullOrEmpty(usuarioParams.Nombre) || x.Nombre.Contains(usuarioParams.Nombre)) &&
                (string.IsNullOrEmpty(usuarioParams.Apellido) || x.Apellido.Contains(usuarioParams.Apellido))
            )
        {  }
    }
}
