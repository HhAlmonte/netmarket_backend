using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class DefaultController : BaseApiController
    {
        [HttpGet]
        public string Get()
        {
            return "Aplicacion corriendo";
        }
    }
}
