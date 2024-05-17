namespace GunGame.API
{
    public class PointChangeEventArgs : EventArgs
    {
        public int Killer { get; }
        public int Kills { get; }
        public int Victim { get; }
        public bool Result { get; set; }

        public PointChangeEventArgs(int killer, int kills, int victim)
        {
            Killer = killer;
            Kills = kills;
            Victim = victim;
            Result = true;
        }
    }
}
