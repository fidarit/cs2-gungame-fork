
namespace GunGame.API
{
    public interface IAPI
    {
        event Action<WinnerEventArgs> KnifeStealEvent;
        event Action<KillEventArgs> KillEvent;
        event Action<WinnerEventArgs> WinnerEvent;
        event Action<LevelChangeEventArgs> LevelChangeEvent;
        event Action<PointChangeEventArgs> PointChangeEvent;
        event Action<WeaponFragEventArgs> WeaponFragEvent;
        event Action RestartEvent;

        public int GetMaxLevel();
        public int GetPlayerLevel(int slot);
        public int GetMaxCurrentLevel();
        public bool IsWarmupInProgress();
    }
}