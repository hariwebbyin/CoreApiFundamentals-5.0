using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IConfiguration _config;
        public OperationsController(IConfiguration config)
        {
            _config = config;
        }


        [HttpOptions("ReloadConfig")] //We're not using any CRUD verb, cause we're not doing any CRUD
                                      //I can assume it as a custome verm which seems to be good option for Functional API
                                      //where we're not doing any CRUD
        public IActionResult ReloadConfig()
        {
            try
            {
                //do whatever you want (but for any operation on data, i.e. CRUD)

                var root = (IConfigurationRoot)_config;
                root.Reload();
                return Ok();

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
