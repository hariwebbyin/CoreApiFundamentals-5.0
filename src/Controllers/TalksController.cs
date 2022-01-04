using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using CoreCodeCamp.Data;
using AutoMapper;
using Microsoft.AspNetCore.Routing;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")] //since we're trying to create a controller that's related to another part of the rourte
                                         //(/api/camps/atl2016/talks), that's why we're specifying the route to our controller.
                                         //{moniker} will be mapped to perameter of the action.
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _repository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        public TalksController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker) //{moniker} gonna be bind with ("api/camps/{moniker}/talks")'s {moniker}
        {
            try
            {
                var talks = await _repository.GetTalksByMonikerAsync(moniker);
                return Ok(_mapper.Map<TalkModel[]>(talks));

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }

        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id) //moniker is the part of the route at controller level.
                                                                               //now we require id to get specific talk by id
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);

                return Ok(_mapper.Map<TalkModel>(talk));

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _repository.GetCampAsync(moniker); //since talk belongs to a valid camp, so we need that valid camp.
                if (camp == null) return BadRequest("moniker doesn't exist");

                var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("Speaker donesn't exist");

                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp; //we're saying, this talk belongs to this camp.
                talk.Speaker = speaker; //adding the whole speaker bases on the peaker id we got fro the request.

                _repository.Add(talk); //now we just we need to add this.



                if (await _repository.SaveChangesAsync())
                {
                    string location = _linkGenerator.GetPathByAction(HttpContext, //gets the HttpContext of the executing action.
                    "Get",
                    values: new { moniker = moniker, id = talk.TalkId }); //we need both of them for our resource location.
                                                                          //talk.TalkId will be created once the changes have been saved.

                    return Created(location, _mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("not enough data");
                }

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "something went wrong");
            }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int id, TalkModel model)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id, true);
                if (talk == null) return BadRequest("error");

                _mapper.Map(model, talk); //passing data from model to talk

                if (model.Speaker != null)
                {
                    var speaker = await _repository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if(speaker!=null)
                        talk.Speaker = speaker;

                }

                if (await _repository.SaveChangesAsync())
                    return Ok(_mapper.Map<TalkModel>(talk));
                else
                    return BadRequest("error");
            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "my bad");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(string moniker, int id)
        {
            try
            {
                var talk = await _repository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return BadRequest("talk not found.");

                _repository.Delete(talk);

                if( await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("error");
                }

            }
            catch (Exception)
            {

                return this.StatusCode(StatusCodes.Status500InternalServerError, "my bad");
            }

        }
    }
}
