using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Models
{
    public static class DbInitializer
    {
        public static void Initialize(SalvoContext context)
        {
            // Look for any Players.
            if (!context.Players.Any())
            {
                var players = new Player[]
                {
                    new Player{Email="j.bauer@ctu.gov"},
                    new Player{Email="c.obrian@ctu.gov"},
                    new Player{Email="kim_bauer@ctu.gov"},
                    new Player{Email="t.almeida@ctu.gov"}
                };

                foreach(Player p in players)
                {
                    //lo agrego al conexto (salvocontext)
                    context.Players.Add(p);
                }
                context.SaveChanges();
            }
            // Look for any Games.
            if (!context.Games.Any())
            {
                var games = new Game[]
                {
                    new Game{CreationDate=DateTime.Now},
                    //agregamos una hora
                    new Game{CreationDate=DateTime.Now.AddHours(1)},
                    //agregamos 2 horas
                    new Game{CreationDate=DateTime.Now.AddHours(2)},
                    //agregamos 3 horas
                    new Game{CreationDate=DateTime.Now.AddHours(3)}
                };
                //recorremos el arreglo game
                foreach (Game g in games)
                {
                    context.Games.Add(g);
                }
                //guardamos los cambios
                context.SaveChanges();
            }
            // Look for any GamePlayers.
            if (!context.GamePlayers.Any())
            {
                Game game1 = context.Games.Find(1L);
                Game game2 = context.Games.Find(2L);
                Game game3 = context.Games.Find(3L);
                Game game4 = context.Games.Find(4L);

                Player jbauer = context.Players.Find(1L);
                Player obrian = context.Players.Find(2L);
                Player kbauer = context.Players.Find(3L);
                Player almeida = context.Players.Find(4L);

                var gamesPlayers = new GamePlayer[]
                {
                    new GamePlayer {JoinDate=DateTime.Now, Game = game1, Player = jbauer},
                    new GamePlayer{JoinDate=DateTime.Now, Game = game1, Player = obrian},
                    new GamePlayer{JoinDate=DateTime.Now, Game = game2, Player = jbauer},
                    new GamePlayer{JoinDate=DateTime.Now, Game = game2, Player = obrian},
                    new GamePlayer{JoinDate=DateTime.Now, Game = game3, Player = obrian},
                    new GamePlayer{JoinDate=DateTime.Now, Game = game3, Player = almeida},
                    new GamePlayer{
                        JoinDate=DateTime.Now, Game = game4, Player = jbauer
                    },
                    new GamePlayer
                    {
                        JoinDate=DateTime.Now, Game = game4, Player = almeida
                    },
                };

                foreach(GamePlayer gp in gamesPlayers)
                {
                    context.GamePlayers.Add(gp);
                }

                context.SaveChanges();

            }
            // Look for any Ships.
            if (!context.Ships.Any())
            {
                GamePlayer gamePlayer1 = context.GamePlayers.Find(1L);
                GamePlayer gamePlayer2 = context.GamePlayers.Find(2L);
                GamePlayer gamePlayer3 = context.GamePlayers.Find(3L);
                GamePlayer gamePlayer4 = context.GamePlayers.Find(4L);
                GamePlayer gamePlayer5 = context.GamePlayers.Find(5L);
                GamePlayer gamePlayer6 = context.GamePlayers.Find(6L);
                GamePlayer gamePlayer7 = context.GamePlayers.Find(7L);
                GamePlayer gamePlayer8 = context.GamePlayers.Find(8L);

                var ships = new Ship[]
                {
                    new Ship{ 
                            Type="Destroyer", GamePlayer = gamePlayer1, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "H2"},
                            new ShipLocation { Location = "H3"},
                            new ShipLocation { Location = "H4"},
                            },

                    },
                    new Ship{
                            Type="Submarine", GamePlayer = gamePlayer1, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "E1"},
                            new ShipLocation { Location = "F1"},
                            new ShipLocation { Location = "G1"},
                            },

                    },
                    new Ship{
                            Type="PatroalBoat", GamePlayer = gamePlayer1, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "B4"},
                            new ShipLocation { Location = "B5"}
                            },

                    },
                     new Ship{
                            Type="Destroyer", GamePlayer = gamePlayer2, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "B5"},
                            new ShipLocation { Location = "C5"},
                            new ShipLocation { Location = "D5"},
                            },

                    },
                    new Ship{
                            Type="PatroalBoat", GamePlayer = gamePlayer2, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "F1"},
                            new ShipLocation { Location = "F2"}
                            },

                    },
                    new Ship{
                            Type="Destroyer", GamePlayer = gamePlayer3, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "B5"},
                            new ShipLocation { Location = "C5"},
                            new ShipLocation { Location = "D5"},
                            },

                    },
                    new Ship{
                            Type="PatroalBoat", GamePlayer = gamePlayer3, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "C6"},
                            new ShipLocation { Location = "C7"}
                            },

                    },
                     new Ship{
                            Type="Submarine", GamePlayer = gamePlayer4, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "A2"},
                            new ShipLocation { Location = "A3"},
                            new ShipLocation { Location = "A4"},
                            },

                    },
                    new Ship{
                            Type="PatroalBoat", GamePlayer = gamePlayer4, Locations = new ShipLocation[]{
                            new ShipLocation { Location = "G6"},
                            new ShipLocation { Location = "H6"}
                            },

                    },

                };

                foreach(Ship ship in ships)
                {
                    context.Ships.Add(ship);
                }

                context.SaveChanges();
            }
            // Look for any Salvos.
            if (!context.Salvos.Any()) 
            {
                GamePlayer gamePlayer1 = context.GamePlayers.Find(1L);
                GamePlayer gamePlayer2 = context.GamePlayers.Find(2L);
                GamePlayer gamePlayer3 = context.GamePlayers.Find(3L);
                GamePlayer gamePlayer4 = context.GamePlayers.Find(4L);
                GamePlayer gamePlayer5 = context.GamePlayers.Find(5L);
                GamePlayer gamePlayer6 = context.GamePlayers.Find(6L);
                GamePlayer gamePlayer7 = context.GamePlayers.Find(7L);
                GamePlayer gamePlayer8 = context.GamePlayers.Find(8L);

                var salvos = new Salvo[]
                {
                    //jbauer gp1
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer1, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "B5"},
                        new SalvoLocation { Location = "C5"},
                        new SalvoLocation { Location = "F1"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer1, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "F2"},
                        new SalvoLocation { Location = "F5"}
                   } },
                   //c.obrian gp2
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer2, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "B4"},
                        new SalvoLocation { Location = "B5"},
                        new SalvoLocation { Location = "B6"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer2, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "E1"},
                        new SalvoLocation { Location = "H3"},
                        new SalvoLocation { Location = "A2"}
                   } },
                   //jbauer gp3
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer3, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "A2"},
                        new SalvoLocation { Location = "A4"},
                        new SalvoLocation { Location = "G6"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer3, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "A3"},
                        new SalvoLocation { Location = "H6"}
                   } },
                   //c.obrian gp4
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer4, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "B5"},
                        new SalvoLocation { Location = "D5"},
                        new SalvoLocation { Location = "C7"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer4, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "C5"},
                        new SalvoLocation { Location = "C6"}
                   } },
                   //c.obrian gp5
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer5, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "G6"},
                        new SalvoLocation { Location = "H6"},
                        new SalvoLocation { Location = "A4"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer5, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "A2"},
                        new SalvoLocation { Location = "A3"},
                        new SalvoLocation { Location = "D8"}
                   } },
                   //t.almeida gp6
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer6, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "H1"},
                        new SalvoLocation { Location = "H2"},
                        new SalvoLocation { Location = "H3"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer6, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "E1"},
                        new SalvoLocation { Location = "F2"},
                        new SalvoLocation { Location = "G3"}
                   } },
                   //c.obrian gp7
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer7, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "A3"},
                        new SalvoLocation { Location = "A4"},
                        new SalvoLocation { Location = "F7"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer7, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "A2"},
                        new SalvoLocation { Location = "G5"},
                        new SalvoLocation { Location = "H6"}
                   } },
                   //jbauer gp8
                   new Salvo{ Turn = 1, GamePlayer = gamePlayer8, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "B5"},
                        new SalvoLocation { Location = "C6"},
                        new SalvoLocation { Location = "H1"},
                   } },
                   new Salvo{ Turn = 2, GamePlayer = gamePlayer8, Locations = new SalvoLocation[]{ 
                        //Datos de Locations 
                        new SalvoLocation { Location = "C5"},
                        new SalvoLocation { Location = "C7"},
                        new SalvoLocation { Location = "D5"}
                   } },

                };
                foreach (Salvo salvo in salvos)
                {
                    context.Salvos.Add(salvo);
                }

                context.SaveChanges();
            }
            // Look for any Score.
            if (!context.Scores.Any()) {
                //Game
                Game game1 = context.Games.Find(1L);
                Game game2 = context.Games.Find(2L);
                Game game3 = context.Games.Find(3L);
                Game game4 = context.Games.Find(4L);
                //Players
                Player jbauer = context.Players.Find(1L);
                Player obrian = context.Players.Find(2L);
                Player kbauer = context.Players.Find(3L);
                Player almeida = context.Players.Find(4L);

                var scores = new Score[] { 
                    //Game1
                    new Score{ 
                        Game = game1,
                        Player = jbauer,
                        FinishDate = DateTime.Now,
                        Point = 1
                    },
                    new Score{
                        Game = game1,
                        Player = obrian,
                        FinishDate = DateTime.Now,
                        Point = 0
                    },
                    //Game2
                    new Score{
                        Game = game2,
                        Player = jbauer,
                        FinishDate = DateTime.Now,
                        Point = 0.5
                    },
                    new Score{
                        Game = game2,
                        Player = obrian,
                        FinishDate = DateTime.Now,
                        Point = 0.5
                    },
                    //Game3
                    new Score{
                        Game = game3,
                        Player = obrian,
                        FinishDate = DateTime.Now,
                        Point = 1
                    },
                    new Score{
                        Game = game3,
                        Player = almeida,
                        FinishDate = DateTime.Now,
                        Point = 0
                    },
                    //Game4
                    new Score{
                        Game = game4,
                        Player = obrian,
                        FinishDate = DateTime.Now,
                        Point = 0.5
                    },
                    new Score{
                        Game = game4,
                        Player = jbauer,
                        FinishDate = DateTime.Now,
                        Point = 0.5
                    }

                };

                foreach (Score score in scores) {
                    context.Scores.Add(score);
                }
                context.SaveChanges();

            }
        }
    }
}
