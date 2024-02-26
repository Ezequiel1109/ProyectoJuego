using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salvo.Models;

namespace Salvo.Repositories
{
    public interface IGamePlayerRepository
    {
        GamePlayer GetGamePlayerView(int idGamePlayer);
        void Save(GamePlayer gamePlayer);
        GamePlayer FindById(int id);
    }
}
