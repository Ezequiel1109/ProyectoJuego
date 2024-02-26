using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Models;
using Salvo.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private IPlayerRepository _repository;
        public PlayersController(IPlayerRepository repository)
        {
            _repository = repository;
        }
        // GET: api/<PlayersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<PlayersController>/5
        [HttpGet("{id}", Name ="GetPlayer")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<PlayersController>
        [HttpPost]
        public IActionResult Post([FromBody] PlayerDTO player)
        {
            try 
            {   //Validacion de los datos
                if (String.IsNullOrEmpty(player.Email) || String.IsNullOrEmpty(player.Password)) 
                {
                    return StatusCode(401, "Datos Inválidos");
                }
                //Buscar los datos
                Player dbPlayer = _repository.FindByEmail(player.Email);
                //si no existe tiene que devolver null
                if (dbPlayer != null) 
                {
                    return StatusCode(401, "El Email esta en uso");
                }
                Player players = new Player
                {
                    Email = player.Email,
                    Password = player.Password
                };
                _repository.Save(players);
                return StatusCode(201, players);
            } 
            catch (Exception ex) 
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<PlayersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<PlayersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
