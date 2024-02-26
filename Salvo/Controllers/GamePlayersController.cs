using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Salvo.Models;
using Salvo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Salvo.Controllers
{
    [Route("api/gamePlayers")]
    [ApiController]
    [Authorize("PlayerOnly")]
    public class GamePlayersController : ControllerBase
    {
        private IGamePlayerRepository _repository;
        private IPlayerRepository _playerRepository;
        private IScoreRepository _scoreRepository;
        public GamePlayersController(IGamePlayerRepository repository,
            IPlayerRepository playerRepository,
            IScoreRepository scoreRepository)
        {
            _repository = repository;
            _playerRepository = playerRepository;
            _scoreRepository = scoreRepository;
        }

        // GET: api/<GamePlayersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<GamePlayersController>/5
        [HttpGet("{id}", Name ="GetGameView")]
        public IActionResult GetGameView(int id)
        {
            try
            {
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";

                var gp = _repository.GetGamePlayerView(id);
                if (gp.Player.Email != email){
                    return Forbid();
                }
                var gameView = new GameViewDTO
                {
                    Id = gp.Id,
                    CreationDate = gp.Game.CreationDate,
                    Ships = gp.Ships.Select(ship => new ShipDTO
                    {
                        Id = ship.Id,
                        Type = ship.Type,
                        Locations = ship.Locations.Select(shipLocation => new ShipLocationDTO
                        {
                            Id = shipLocation.Id,
                            Location = shipLocation.Location
                        }).ToList()
                    }).ToList(),
                    GamePlayers = gp.Game.GamePlayers.Select(gps => new GamePlayerDTO
                    {
                        Id = gps.Id,
                        JoinDate = gps.JoinDate,
                        Player = new PlayerDTO
                        {
                            Id = gps.Player.Id,
                            Email = gps.Player.Email
                        }
                    }).ToList(),
                    Salvos = gp.Game.GamePlayers.SelectMany(gps => gps.Salvos.Select(salvo => new SalvoDTO {
                        Id = salvo.Id,
                        Turn = salvo.Turn,
                        Player = new PlayerDTO {
                            Id = gps.Player.Id,
                            Email = gps.Player.Email
                        },
                        Locations = salvo.Locations.Select(
                                salvoLocation => new SalvoLocationDTO {
                                    Id = salvoLocation.Id,
                                    Location = salvoLocation.Location
                                }).ToList()
                    })).ToList(),
                    Hits = gp.GetHits(),
                    HitsOpponent = gp.GetOpponet()?.GetHits(),
                    Sunks = gp.GetSunks(),
                    SunksOpponent = gp.GetOpponet()?.GetSunks(),
                    GameState = Enum.GetName(typeof(GameState), gp.GetGameState())
                        
                };

                return Ok(gameView);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        // POST api/GamePlayers/id/ships
        [HttpPost("{id}/ships")]
        public IActionResult Post(int id,[FromBody] List<ShipDTO> ships)
        {
            try {
                //Autentificar email
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";
                //Buscar al jugador por email
                Player player = _playerRepository.FindByEmail(email);
                //Buscar al jugador por id
                GamePlayer gamePlayer = _repository.FindById(id);
                if (gamePlayer == null){
                    return StatusCode(403, "No existe el juego");
                }

                //El usuario no se encuentra en le juego
                //Se compara el player.id vs gamePlayer.Player.Id
                if (gamePlayer.Player.Id != player.Id){
                    return StatusCode(403, "El usuario no se encuentra en el juego");
                }

                //validacion de los barcos que esten posicionados
                //Gameplayer debria tener 5 barcos
                if (gamePlayer.Ships.Count == 5){
                    return StatusCode(403, "Ya se han Posicionado los barcos");
                }
                //Insertar los barcos al gameplayer
                gamePlayer.Ships = ships.Select(sh => new Ship
                {
                    GamePlayerId = gamePlayer.Id,
                    Type = sh.Type,
                    Locations = sh.Locations.Select(
                            loc => new ShipLocation
                            {
                                ShipId = sh.Id,
                                Location = loc.Location
                            }).ToList()
                }).ToList();
                //Actualizar informacion 
                _repository.Save(gamePlayer);
                return StatusCode(201);

            } catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/GamePlayers/id/salvos
        [HttpPost("{id}/salvos")]
        public IActionResult Post(int id,[FromBody] SalvoDTO salvo) 
        {
            try {
                int playerTurn = 0;
                int rivalTurn = 0;
                //Obtener el email del player
                string email = User.FindFirst("Player") != null ? User.FindFirst("Player").Value : "Guest";
                //Traer el Player autentificado
                Player player = _playerRepository.FindByEmail(email);
                //Traer al jugador por id
                GamePlayer gamePlayer = _repository.FindById(id);
                if (gamePlayer == null)
                {
                    return StatusCode(403, "No existe el juego");
                }
                if (gamePlayer.Player.Id != player.Id)
                {
                    return StatusCode(403, "El usuario no se encuentra en el juego");
                }

                //Traer al rival
                GamePlayer rival = gamePlayer.GetOpponet();
                //rival = _repository.FindById(int.Parse(rival.Id));
                //Verificar si el rival existe
                if (rival == null) 
                {
                    return StatusCode(403, "No se encontro oponente");
                }
                //Verificar si los ships estan posicionados 
                if (rival != null) 
                {
                    List<Ship> shipsRival = _repository.FindById((int)rival.Id).Ships.ToList();
                    if (shipsRival == null || shipsRival.Count == 0) 
                    {
                        return StatusCode(403, "Tu oponente no ha posicionado sus barcos!!!");
                    }
                }
                GameState gameState = gamePlayer.GetGameState();
                if (gameState == GameState.LOSS || gameState == GameState.WIN  || gameState == GameState.TIE)
                {
                    return StatusCode(403, "El juego ya termino!!!");
                }
                //Se determina el turno del jugador actual 
                playerTurn = gamePlayer.Salvos != null ? gamePlayer.Salvos.Count() + 1 : 1;
                rivalTurn = rival.Salvos != null ? rival.Salvos.Count() : 0;
                //Validar si el rival ya existe
                if (gamePlayer.JoinDate < rival.JoinDate && (playerTurn - rivalTurn) != 1)
                {
                    return StatusCode(403, "No se puede adelantar al turno");
                }
                if (gamePlayer.JoinDate > rival.JoinDate && (playerTurn - rivalTurn) != 0)
                {
                    return StatusCode(403, "No se puede adelantar al turno");
                }

                //validar para ver si se esta adelantando al turno que le corresponde
                //if ((playerTurn - rivalTurn) < -1 || (playerTurn - rivalTurn) > 1) {
                //    return StatusCode(403, "No se puede adelantar el turno");
                //}
                //Guardar Datos 
                gamePlayer.Salvos.Add(new Salvo.Models.Salvo
                {
                    GamePlayerId = gamePlayer.Id,
                    Turn = playerTurn,
                    Locations = salvo.Locations.Select(location => new SalvoLocation
                    {
                        SalvoId = salvo.Id,
                        Location = location.Location
                    }).ToList()
                });
                _repository.Save(gamePlayer);
                //Guardar Score
                gameState = gamePlayer.GetGameState();
                if (gameState == GameState.WIN)
                {
                    #region Win
                    Score score = new Score
                    {
                        FinishDate = DateTime.Now,
                        GameId = gamePlayer.GameId,
                        PlayerId = gamePlayer.PlayerId,
                        Point = 1
                    };
                    _scoreRepository.Save(score);

                    Score scoreRival = new Score
                    {
                        FinishDate = DateTime.Now,
                        GameId = gamePlayer.GameId,
                        PlayerId = rival.PlayerId,
                        Point = 0
                    };
                    _scoreRepository.Save(scoreRival);
                    #endregion
                }
                else if (gameState == GameState.LOSS)
                {
                    #region Loss
                    Score score = new Score
                    {
                        FinishDate = DateTime.Now,
                        GameId = gamePlayer.GameId,
                        PlayerId = gamePlayer.PlayerId,
                        Point = 0
                    };
                    _scoreRepository.Save(score);

                    Score scoreRival = new Score
                    {
                        FinishDate = DateTime.Now,
                        GameId = gamePlayer.GameId,
                        PlayerId = rival.PlayerId,
                        Point = 1
                    };
                    _scoreRepository.Save(scoreRival);
                    #endregion
                }
                else if (gameState == GameState.TIE)
                {
                    #region Tie
                    Score score = new Score
                    {
                        FinishDate = DateTime.Now,
                        GameId = gamePlayer.GameId,
                        PlayerId = gamePlayer.PlayerId,
                        Point = 0.5
                    };
                    _scoreRepository.Save(score);

                    Score scoreRival = new Score
                    {
                        FinishDate = DateTime.Now,
                        GameId = gamePlayer.GameId,
                        PlayerId = rival.PlayerId,
                        Point = 0.5
                    };
                    _scoreRepository.Save(scoreRival);
                    #endregion
                }
                return StatusCode(201);
            } catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

      
    }
}
