using System;
using System.Collections.Generic;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService clientService;

        public ClientsController(IClientService clientService)
        {
            this.clientService = clientService;
        }

        [HttpGet]
        public IEnumerable<Client> All() => clientService.All();

        [HttpGet("{id}")]
        public Client ById(Guid id) => clientService.ById(id);

        [HttpPost]
        public ActionResult Insert([FromBody] Client client)
        {
            clientService.Insert(client);
            return Ok();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Client client)
        {
            clientService.Update(client);
            return Ok();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(Guid id)
        {
            clientService.Delete(id);
            return Ok();
        }
    }
}
