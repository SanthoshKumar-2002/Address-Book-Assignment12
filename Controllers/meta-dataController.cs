using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi2.Contracts;

namespace WebApi2.Controllers
{
    [Route("api/[controller]/")]
    [ApiController]
    public class meta_dataController : ControllerBase
    {
        private readonly IUserService userContract;

        public meta_dataController(IUserService userContract)
        {
            this.userContract = userContract;
        }
        /// <summary>
        /// for getting the meta data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        // meta data
        [HttpGet]
        [Route("[Action]/{key}")]
        [Authorize]
        public IActionResult ref_Set(int key)
        {
            string sub = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (userContract.usercheck(sub))
                return Ok(userContract.GetMetadata(key));
            return Unauthorized();  
        }
    }
}
