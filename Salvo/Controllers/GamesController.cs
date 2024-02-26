using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        /// <summary>
        /// miembro privado IGameRepository
        /// </summary>
        private IGameRepository _repository;
        private IPlayerRepository _playerRepository;
        private IGamePlayerRepository _gamePlayerRepository;
        //constructor
        public GamesController(IGameRepository repository,
            IPlayerRepository playerRepository, 
            IGamePlayerRepository gamePlayerRepository)
        {
            _repository = repository;
            _playerRepository = playerRepository;
            _gamePlayerRepository = gamePlayerRepository;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            try
            {
                GameListDTO gameList = new GameListDTO
                {
                    Email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest",
                    Games = _repository.GetAllGamesWithPlayers()
                    .Select(g => new GameDTO
                    {
                        Id = g.Id,
                        CreationDate = g.CreationDate,
                        GamePlayers = g.GamePlayers.Select(gp => new GamePlayerDTO
                        {
                            Id = gp.Id,
                            JoinDate = gp.JoinDate,
                            Player = new PlayerDTO
                            {
                                Id = gp.Player.Id,
                                Email = gp.Player.Email
                            },
                            //operador  ternario
                            Point = gp.GetScore() != null ? (double?)gp.GetScore().Point : null
                        }).ToList().OrderByDescending(p => p.Point).ToList()

                    }).ToList()
                };
                return Ok(gameList);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/<GamesController>
        /// <summary>
        /// este get no nos sirve ya que usaremos IActionResult
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<GamesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<GamesController>
        [HttpPost]
        public IActionResult Post()
        {
            try {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                //se busca el player por el email correspondiente
                Player player = _playerRepository.FindByEmail(email);
                //creacion de un nuevo player
                GamePlayer gamePlayer = new GamePlayer
                {
                    PlayerId = player.Id,
                    JoinDate = DateTime.Now,
                    Game = new Game{
                        CreationDate = DateTime.Now
                    }
                };
                _gamePlayerRepository.Save(gamePlayer);
                return StatusCode(201, gamePlayer.Id);
            } catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{id}/players")]
        public IActionResult Join(int id) {
            try {
                //Con el email podemos recibir la informacion
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";
                //Obtener player
                Player player = _playerRepository.FindByEmail(email);
                //Buscar el game por email
                Game game = _repository.FindById(id);
                //Si no se encuentra el game 
                if (game == null){
                    return StatusCode(403, "No existe el juego");
                }
                //Verificar que el juego tenga solo un player
                if (game.GamePlayers.Where(gp => gp.Player.Id == player.Id).FirstOrDefault() != null){
                    return StatusCode(403, "Ya se encuentra el jugador en el juego");
                }
                //Verificar que el game tenga un solo player
                if (game.GamePlayers.Count > 1){
                    return StatusCode(403, "Juego lleno ");
                }
                //Crear un Game Player
                GamePlayer gamePlayer = new GamePlayer
                {
                    GameId = game.Id,
                    PlayerId = player.Id,
                    JoinDate = DateTime.Now
                };
                //Recuperar los cambios
                _gamePlayerRepository.Save(gamePlayer);
                //retornar el status 
                return StatusCode(201, gamePlayer.Id);
            } catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
        // PUT api/<GamesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GamesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
