using Microsoft.AspNetCore.Mvc;

namespace MyWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        // GET: api/hello
        [HttpGet]
        public IActionResult GetMessage()
        {
            return Ok(new { message = "Hello, World!" });
        }
    }

    [Route("/[controller]")]
    [ApiController]
    public class ServerRunning : ControllerBase{
        [HttpGet]
        public IActionResult GetServerStatus(){
            return Ok(new { message = "Server is running!" });
        }
    }
}
