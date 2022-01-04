using Microsoft.AspNetCore.Mvc; // it has ControllerBase, Route classes, ApiVersions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Routing; //it has - LinkGenerator

namespace CoreCodeCamp.Controllers
{
    [Route("api/v{version:ApiVerion}/[Controller]")] // now it's a part of the url (v{version:ApiVerion})
    [ApiVersion("1.0")]
    //OR
    [Route("api/v2/[Controller]")] // now it's a part of the url and it is hard coded. (being used in OM), management decided it.

    [ApiController]
    public class CampsController : ControllerBase //it is specialized for web api,
                                                  //"Controller" class can be inherit by both, web api, and MVC
    {
        private readonly ICampRepository _repository; //this is gonna do crud for us
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator) //We're injecting this so we can call _mapper.Map later when we want.
                                                                                                        //(to convert the entiry to model).
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }
        [HttpGet] 
        [MapToApiVersion("1.0")] //now this action will server only 1.0 api requests
                                 //MapToApiVersion is also part of routing, it is part of the request.
        public async Task<ActionResult<CampModel[]>> GetCampts(bool IncludeTalks = false) // we're not gonna include talks by default for version 1.0
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(IncludeTalks); 

                CampModel[] models = _mapper.Map<CampModel[]>(results);

                return models;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"DB error");
            }
        }

        [HttpGet]
        [MapToApiVersion("1.1")]
        public async Task<ActionResult<CampModel[]>> GetCamps1(bool IncludeTalks = true) //we're gonna include talks by default for version 1.1
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(IncludeTalks); 

                CampModel[] models = _mapper.Map<CampModel[]>(results);

                return models;
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"DB error");
            }
        }


        //[HttpGet("{ID: int}")] //if I'm expeting any other premitive data type than string, then I gotta mention it here.
        [HttpGet("{moniker}")] // This ("{moniker}") is gonna map moniker input with our GetCampByMoniker's parameter.
                               // We added moniker as a part of the rout, that can be mapped to the input variable inside our database.
                               //By default the data type is string.
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);

                if (result == null)
                    return NotFound();

                return Ok(_mapper.Map<CampModel>(result));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "My bad.");
            }
        }


        [HttpGet("Search")] //Remember, it's not wrapped up in {}, It means, Search is a part our the rout (the hard coded "Search" variable
        public async Task<ActionResult<CampModel[]>> GetByEventData(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks); //the search is actually happening at EF level

                if (!results.Any()) return NotFound();

                return Ok(_mapper.Map<CampModel[]>(results));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "My bad.");
            }
        }

        public async Task<ActionResult<CampModel>> Post(CampModel model) 

        {
            try
            {
                //moniker should be unique
                var existingCamps = await _repository.GetCampAsync(model.Moniker);
                if (existingCamps != null)
                    return BadRequest("Moniker already exists"); //sine we want the moniker to be unique.


                var location = _linkGenerator.GetPathByAction("Get", "Camps", new { moniker = model.Moniker });
                if (string.IsNullOrWhiteSpace(location))
                    return BadRequest("cannot take this moniker name");

                //create new camp
                var cmp = _mapper.Map<Camp>(model);

                _repository.Add(cmp);

                if (await _repository.SaveChangesAsync())
                    return Created(location, _mapper.Map<CampModel>(cmp)); 
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "My bad.");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")] // we need some id to do the put (update) against.
        public async Task<ActionResult<CampModel>> put(string moniker, CampModel model) //we can't put on collection, we're gonna need a single camp here.
                                                                                        //we're getting the single
                                                                                        //camp using a {moniker}
        {
            try
            {
                var oldCamp = await _repository.GetCampAsync(moniker); //we'll need some old camp to update that.
                if (oldCamp == null) return NotFound($"no camp found with {moniker}"); 

                _mapper.Map(model, oldCamp); //this is the another place where mapper can be useful.
                                             //here, the .Map(source, destination) method id appying the changes from source object to desitnation object.

                if (await _repository.SaveChangesAsync()) // since _repository has reference to oldCamp, we just need to call SaveChangesAsync.
                    return _mapper.Map<CampModel>(oldCamp); // put doesn't require you to have any special status code to return,
                                                            // Ok/200 is the correct return type, because the way we're using ActionResult,
                                                            // we can just return _mapper.Map<CampModel>(oldCamp)
                                                            //and 200 will be returned.
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "something went wrong");
            }
            return BadRequest();
        }


        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker) //we're returing IActionResult, cause we're not returning any body(object) in delete,
                                                                //we're just returning a status code.
                                                                //dfg
        {
            try
            {
                var cmp = await _repository.GetCampAsync(moniker);
                if (cmp == null) return NotFound();

                _repository.Delete(cmp); //since we've got it from the db, we can actually just use the _repository and say _repository.Delete our cmp.
                                         //If we had any assiciated object with cmp, we might have to write some business logic to delete that,
                                         //sice it's a simple delete on a simple object, it will work.

                if (await _repository.SaveChangesAsync())
                    return Ok();

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong.");
            }
            return BadRequest();
        }

    }
}
