using Microsoft.AspNetCore.Mvc;

namespace VerifiedIdDomain.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DidController : ControllerBase
    {
        [Route("/.well-known/did.json")]
        public ActionResult GetDid()
        {
            var json = "";

            using (StreamReader stream = new StreamReader("./did.json"))
            {
                json = stream.ReadToEnd();
            }
            return Ok(json);
        }

        [Route("/.well-known/did-configuration.json")]
        public ActionResult GetDidConfiguration()
        {
            var json = "";

            using (StreamReader stream = new StreamReader("./did-configuration.json"))
            {
                json = stream.ReadToEnd();
            }
            return Ok(json);
        }
    }
}