using System;
using UnityEngine;

namespace tarkin.Director
{
    public class SpawnPoint : MonoBehaviour
    {
        void OnEnable()
        {
#if EFT_RUNTIME
            Comfort.Common.Singleton<EFT.GameWorld>.Instance.MainPlayer.Teleport(transform.position);
#endif
        }
    }
}
