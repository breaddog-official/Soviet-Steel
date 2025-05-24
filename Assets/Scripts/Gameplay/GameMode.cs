using System.Linq;
using Scripts.Network;
using Scripts.UI;

namespace Scripts.Gameplay
{
    public class GameMode
    {
        public int rounds = 1;
        public int bots = 5;

        public string mapHash;

        public Map Map => string.IsNullOrEmpty(mapHash) ? null : NetworkManagerExt.GetMap(mapHash);
    }
}
