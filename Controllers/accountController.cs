using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApi.Entities.Models;
using WebApi.Entities.DTO;
using WebApi2.Contracts;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Data.SqlTypes;
using WebApi2.Repository;
using WebApi2.Entities.DTO;
using WebApi2;

namespace WebApi.Controllers
{

    [ApiController]
    [Route("api/[Controller]/")]
    
    
    public class accountController : Controller
        {

          private readonly IUserService userContract;
        
        public accountController(IUserService userContract)
        {
            this.userContract = userContract;
        }
         /// <summary>
         /// For adding the new user
         /// </summary>
         /// <param name="user"></param>
         /// <returns>id of the user</returns>
        // create account
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDTO user)
        {
            try
            {
                var result = userContract.Create(user);
                if (result == "exist")
                    return Conflict("UserName already exist");
                if (result == "Email exist")
                    return Conflict("Email already exist");
                return Ok(result);

            }
            catch(KeyNotFoundException)
            {
              return Conflict("type is not valid");
            }
          
        }

        /// <summary>
        /// for getting all the user
        /// </summary>
        /// <returns></returns>
        // get all the user
       [HttpGet]
        [Authorize]
        public IActionResult GetAllUser([FromQuery] Pagination pagination)
        {

            string sub = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (userContract.usercheck(sub))
                return Ok(userContract.GetAll(pagination));
            
            return Unauthorized();
        }
        /// <summary>
        /// to get the count of the Address book
        /// </summary>
        /// <returns></returns>
        // get the count
            [HttpGet]
            [Route("[Action]")]
        [Authorize]
        public virtual IActionResult count()

        {
            string sub = HttpContext?.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            if (userContract.usercheck(sub))
                return Ok(userContract.GetCount());
            return Unauthorized();
        }
        /// <summary>
        /// to get the address book using id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //get by id
            [HttpGet]
            [Route("{id}")]
        [Authorize]

        public  IActionResult GetById( Guid id)
            {
            ClaimsIdentity claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var object1 = userContract.authorize(claimsIdentity, id);
            if(object1==null)
            {
                return Unauthorized();
            }
            GetDTO result= userContract.GetId(id);
            if (result == null)
                return NotFound();
            return Ok(result);
           
        }
        /// <summary>
        /// to update the address book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        //update
        [HttpPut]
        [Route("{id}")]
       [Authorize]
        public  IActionResult Update(Guid id, UserDTO body)
        {
          ClaimsIdentity claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
           var ob = userContract.authorize(claimsIdentity, id);
            if (ob == null)
           {
                return Unauthorized();
            }
            UpdateDTO user = new UpdateDTO();
            try
            {
                user = (UpdateDTO)userContract.Update(id, body);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch(KeyNotFoundException)
            {
                return Conflict("type is not valid");
            }
            catch(Exception)
            {
                return Conflict("email already exist");
            }
            
        }
        /// <summary>
        /// to delete the address book by using the id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //delete
          [HttpDelete]
          [Route("{id}")]
          [Authorize]
        public virtual IActionResult Delete(Guid id)
              {
            
           ClaimsIdentity claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            if (userContract.authorize(claimsIdentity, id))
           {
                var result = userContract.Delete(id);
                 if(result==null)   
                    return NotFound();
                 return Ok(result);
           }
            return Unauthorized();
              }

    }
}
