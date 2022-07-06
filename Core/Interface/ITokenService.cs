using Core.Entities;
using System.Collections.Generic;


namespace Core.Interface
{
    public interface ITokenService
    {
        string CreateToken(Usuario usuario, IList<string> roles);
    }
}
