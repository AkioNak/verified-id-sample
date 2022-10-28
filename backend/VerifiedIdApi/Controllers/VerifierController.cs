using Microsoft.AspNetCore.Mvc;
using VerifiedIdApi.Services;
using VerifiedIdApi.ViewModels;

namespace VerifiedIdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerifierController : ControllerBase
    {
        private readonly VerifierService VerifierService;

        public VerifierController(VerifierService verifierService)
        {
            VerifierService = verifierService;
        }

        /// <summary>
        /// 証明書の検証をリクエストする
        /// </summary>
        /// <returns></returns>
        [HttpGet("presentation-request")]
        public async Task<ActionResult<PresentationRequestResultViewModel>> PresentationRequest()
        {
            try
            {
                return Ok(await VerifierService.PresentationRequest(Request));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Verified IDからコールバックされた時のAPI
        /// 
        /// 以下のタイミングでコールバックされる
        /// ①QRコードが読み取られたとき
        /// ②証明書が承認されたとき
        /// </summary>
        /// <returns></returns>
        [HttpPost("presentation-callback")]
        public async Task<ActionResult> PresentationCallback()
        {
            try
            {
                await VerifierService.PresentationCallback(Request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("presentation-response")]
        public ActionResult<PresentationResponseViewModel> PresentationResponse()
        {
            try
            {
                var result = VerifierService.PresentationResponse(Request);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
