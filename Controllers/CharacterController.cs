using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CharacterController : ControllerBase
    {
        private static Character witch = new Character();
        
        [HttpGet]
        public IActionResult Get() 
        {
            return Ok(witch);
        }
    }
}