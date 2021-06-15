

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private static Character witch = new Character();
        public IActionResult Get() 
        {
            return Ok(witch);
        }
    }
}