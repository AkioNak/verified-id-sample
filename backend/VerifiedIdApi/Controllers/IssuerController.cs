using Microsoft.AspNetCore.Mvc;
using VerifiedIdApi.Services;
using VerifiedIdApi.ViewModels;

namespace VerifiedIdApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuerController : ControllerBase
    {
        private readonly IssuerService IssuerService;

        public IssuerController(IssuerService issuerService)
        {
            IssuerService = issuerService;
        }

        /// <summary>
        /// 証明書の発行をリクエストする
        /// </summary>
        /// <returns></returns>
        [HttpPost("issuance-request")]
        public async Task<ActionResult<IssuanceRequestResultViewModel>> IssuanceRequest(
            ClaimViewModel vm)
        {
            try
            {
                return Ok(await IssuerService.IssuanceRequest(Request, vm));
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
        [HttpPost("issuance-callback")]
        public async Task<ActionResult> IssuanceCallback()
        {
            try
            {
                await IssuerService.IssuanceCallback(Request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("issuance-response")]
        public ActionResult<IssuanceResponseViewModel> IssuanceResponse()
        {
            try
            {
                var result = IssuerService.IssuanceResponse(Request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
