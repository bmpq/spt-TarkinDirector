using Comfort.Common;
using EFT;
using tarkin.BSP.Shared;

namespace tarkin.BSP.Bep.Mediators
{
    internal class SpawnPointMediator
    {
        public SpawnPointMediator()
        {
            SpawnPoint.OnEnableEvent += SpawnPoint_OnEnableEvent;
        }

        private void SpawnPoint_OnEnableEvent(SpawnPoint point)
        {
            Singleton<GameWorld>.Instance.MainPlayer.Teleport(point.transform.position);
        }
    }
}
