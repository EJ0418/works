
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldApi.Controllers
{
    [ApiController]
    public class HelloWorldApi
    {
        [HttpGet]
        [Route("helloworld")]
        public string GetHelloWorld()
        {
            return "Hello Wolrd by EJ";
        }
    }
}