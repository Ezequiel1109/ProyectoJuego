using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public class GamePlayer
    {
        //los propios de la tabla Game player
        public long Id { get; set; }
        public DateTime JoinDate { get; set; }
        //foraneas
        //player
        public long PlayerId { get; set; }
        public Player Player { get; set; }
        //games
        public long GameId { get; set; }
        public Game Game { get; set; }
        //relacion con Ship
        public ICollection<Ship> Ships { get; set; }
        //agregar los salvos 
        public ICollection<Salvo> Salvos { get; set; }
        public Score GetScore() 
        {
            return Player.GetScore(Game);
        }
        //Metodo para obtener los oponentes
        public GamePlayer GetOpponet() 
        {
            return Game.GamePlayers.FirstOrDefault(gp => gp.Id != Id);
        }
        //get Hits 
        public ICollection<SalvoHitDTO> GetHits() 
        {
            return Salvos.Select(sl => new SalvoHitDTO
            {
                Turn = sl.Turn,
                Hits = GetOpponet()?.Ships.Select(sh => new ShipHitDTO
                {
                    Type = sh.Type,
                    Hits = sl.Locations
                    .Where(slocation => sh.Locations.Any(shlocation => shlocation.Location == slocation.Location))
                    .Select(slocation => slocation.Location).ToList()
                }).ToList()

            }).ToList();
        }
        //get Sunks
        public ICollection<string> GetSunks() 
        {
            //buscar e identificar el ultimo turno
            int lastTurn = Salvos.Count();
            /**
             * obtener un listado de salvo location
             * Debera entregar todos aquellos que sean ultimo turno <= al turn del salvo
             **/

            List<string> salvoLocations = GetOpponet()?.Salvos
                     .Where(sl => sl.Turn <= lastTurn)
                     .SelectMany(salvo => salvo.Locations.Select(locations => locations.Location)).ToList();

            return Ships?.Where(sh => sh.Locations.Select(shLocation => shLocation.Location)
                        .All(slLocation => salvoLocations != null ? salvoLocations.Any(shLocations => shLocations == slLocation) : false))
                        .Select(sh => sh.Type).ToList();
        }
        //Metodo para obtener el GameState
        public GameState GetGameState() 
        {
            //Iniciar el primer estado
            GameState gameState = GameState.ENTER_SALVO;
            /**
             * Si los barcos vienen nulos o count 0
             * se dejaran en place_ships ya que aun no estan posicionados
             * **/
            if (Ships == null || Ships?.Count() == 0)
            {
                gameState = GameState.PLACE_SHIPS;
            }
            /**
             * Verificar si el rival es null
             * si el rival es nulo debe quedar en espera (Wait)
             * **/
            else if (GetOpponet() == null)
            {
                //Salvos Count
                if (Salvos != null && Salvos?.Count() > 0)
                {
                    gameState = GameState.WAIT;
                }
            }
            else
            {
                GamePlayer opponent = GetOpponet();
                //Determinar Turno 
                int turn = Salvos != null ? Salvos.Count() : 0;
                //Determinar Turno del Rival
                int rivalTurn = opponent.Salvos != null ? opponent.Salvos.Count() : 0;
                // si mi turno es mayor al del oponente debemos esperar
                if (turn > rivalTurn)
                {
                    gameState = GameState.WAIT;
                }
                else if (turn == rivalTurn && turn != 0) 
                {
                    //Sunks
                    int playerSunks = GetSunks().Count();
                    //Sunks rival
                    int rivalSunks = opponent.GetSunks().Count();
                    //Se evalua si existe un empate
                    if (playerSunks == Ships.Count() && rivalSunks == opponent.Ships.Count())
                    {
                        gameState = GameState.TIE;
                    }
                    //Evaluar si Perdio 
                    else if (playerSunks == Ships.Count())
                    {
                        gameState = GameState.LOSS;
                    }
                    //Evaluar si Gano
                    else if (rivalSunks == opponent.Ships.Count()) 
                    {
                        gameState = GameState.WIN;
                    }
                }
            }

            return gameState;
        }
       
    }
}
