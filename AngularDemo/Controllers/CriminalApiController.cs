using AngularDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AngularDemo.Controllers
{
    [RoutePrefix("api/criminal")]
    public class CriminalApiController : ApiController
    {
        private static IList<Criminal> _criminalRepo;

        static CriminalApiController()
        {
            _criminalRepo = new List<Criminal>
            {
                new Criminal
                {
                    ID = Guid.NewGuid(),
                    Name = "Al Capone",
                    Description = "Gangster",
                    Reward = 1000000.00M
                },
                new Criminal
                {
                    ID = Guid.NewGuid(),
                    Name = "Charles Manson",
                    Description = "Leader of the Manson Family",
                    Reward = 3000000.00M
                },
                new Criminal
                {
                    ID = Guid.NewGuid(),
                    Name = "Ted Kaczynski",
                    Description = "Terrorist",
                    Reward = 2500000.00M
                },
                new Criminal
                {
                    ID = Guid.NewGuid(),
                    Name = "CJ",
                    Description = "Bicycle thief",
                    Reward = 250000.00M
                }
            };
        }

        [Route("")]
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            return Ok(_criminalRepo.ToArray());
        }

        [Route("")]
        [HttpPost]
        public HttpResponseMessage Create(Criminal criminal)
        {
            criminal.ID = Guid.NewGuid();
            _criminalRepo.Add(criminal);

            return CreateResponseWithGetLocationUri(HttpStatusCode.Created, criminal.ID);
        } 

        [Route("")]
        [HttpPut]
        public HttpResponseMessage Replace(Criminal criminal)
        {
            var entity = _criminalRepo.FirstOrDefault(c => c.ID == criminal.ID);

            if (entity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            entity.Name = criminal.Name;
            entity.Description = criminal.Description;
            entity.Reward = criminal.Reward;
            
            return CreateResponseWithGetLocationUri(HttpStatusCode.OK, criminal.ID);
        }

        [Route("")]
        [HttpDelete]
        public IHttpActionResult DeleteAll()
        {
            _criminalRepo.Clear();

            return Ok();
        }

        [Route("{id:Guid}")]
        [HttpGet]
        public IHttpActionResult Get(Guid id)
        {
            var criminal = _criminalRepo.FirstOrDefault(c => c.ID == id);

            if (criminal == null)
            {
                return NotFound();
            }

            return Ok(criminal);
        }

        [Route("{id:Guid}")]
        [HttpDelete]
        public IHttpActionResult Delete(Guid id)
        {
            var criminal = _criminalRepo.FirstOrDefault(c => c.ID == id);

            if (criminal == null)
            {
                return NotFound();
            }

            _criminalRepo.Remove(criminal);

            return Ok();
        }

        private HttpResponseMessage CreateResponseWithGetLocationUri(HttpStatusCode httpStatusCode, Guid id)
        {
            var response = Request.CreateResponse(httpStatusCode, id.ToString());
            response.Headers.Location = CreateAbsoluteGetUri(id);

            return response;
        }

        private Uri CreateAbsoluteGetUri(Guid id)
        {
            return new Uri(Request.RequestUri.GetLeftPart(UriPartial.Authority)
                            + "/api/criminal/" + id.ToString());
        }
    }
}
