#pragma warning disable CS8981// Naming Styles
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace GunGame.Tools
{
    public class PlayerManager : CustomManager
    {
        public PlayerManager(GunGame plugin) : base(plugin)
        {

        }

        private readonly object lockObject = new();
        private readonly Dictionary<int, GGPlayer> playerMap = new();

        public GGPlayer? CreatePlayerBySlot(int slot)
        {
            if (slot < 0 || slot > Models.Constants.MaxPlayers)
                return null;

            GGPlayer player;
            lock (lockObject)
            {
                if (!playerMap.TryGetValue(slot, out player))
                {
                    player = new GGPlayer(slot, Plugin);
                    playerMap.Add(slot, player);
                }
            }
            if (player.Index == -1)
            {
                var pc = Utilities.GetPlayerFromSlot(slot);
                if (pc != null && pc.IsValid)
                {
                    player.UpdatePlayerController(pc);
                }
                else
                {
                    Logger.LogInformation($"Player slot {slot} - absent PlayerController");
                }
            }
            return player;
        }
        public GGPlayer? FindBySlot(int slot, string name = "")
        {
            if (playerMap.TryGetValue(slot, out GGPlayer? player))
            {
                return player;
            }
            else
            {
                Logger.LogInformation($"[GUNGAME] Can't find player slot {slot} in playerMap from {name}.");
                var pc = Utilities.GetPlayerFromSlot(slot);
                if (pc != null)
                {
                    Server.ExecuteCommand($"kickid {pc.UserId} NoSteamId");
                }
                return null;
            }
        }
        public bool PlayerExists(int slot)
        {
            return playerMap.TryGetValue(slot, out GGPlayer? player);
        }
        public GGPlayer? FindLeader()
        {
            int leaderId = -1;
            uint leaderLevel = 0;
            foreach (var player in playerMap)
            {
                if (player.Value.Level > leaderLevel)
                {
                    leaderLevel = player.Value.Level;
                    leaderId = player.Key;
                }
            }
            if (leaderId == -1)
                return null;
            else
                return FindBySlot(leaderId, "FindLeader");
        }
        public void ForgetPlayer(int slot)
        {
            lock (lockObject)
            {
                playerMap.Remove(slot);
            }
        }

        public double GetDistanceToClosestPlayer(int slot, Vector spawn)
        {
            double dist = double.MaxValue;

            foreach (var player in playerMap.Values)
            {
                if (player.Slot == slot)
                    continue;

                var pc = Utilities.GetPlayerFromSlot(player.Slot);
                if (pc?.PlayerPawn?.Value?.AbsOrigin == null)
                    continue;

                dist = Math.Min(dist, PlayerDistance(spawn, pc.PlayerPawn.Value.AbsOrigin));
            }

            return dist;
        }

        private static double PlayerDistance(Vector entity, Vector player)
        {
            // Calculate the squared distance to avoid the square root for performance reasons
            double Distance = Math.Sqrt((player.X - entity.X) * (player.X - entity.X) +
                                    (player.Y - entity.Y) * (player.Y - entity.Y) +
                                    (player.Z - entity.Z) * (player.Z - entity.Z));

            // Compare squared distances (since sqrt is monotonic, the comparison is equivalent)
            return Distance;
        }
    }
}
// Colors Available = "{default} {white} {darkred} {green} {lightyellow}" "{lightblue} {olive} {lime} {red} {lightpurple}"
//"{purple} {grey} {yellow} {gold} {silver}" "{blue} {darkblue} {bluegrey} {magenta} {lightred}" "{orange}"