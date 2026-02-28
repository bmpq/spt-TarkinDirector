using Comfort.Common;
using EFT;
using UnityEngine;

namespace tarkin.Director
{
    internal class PlayerForceReact : MonoBehaviour
    {
        public bool applyOnEnable = true;
        public float strength = 1f;
        public float recoilHands = 2f;
        public float recoilCamera = 4f;

        void OnEnable()
        {
            if (applyOnEnable)
                ApplyForceToMainPlayer();
        }

        public void ApplyForceToMainPlayer()
        {
#if EFT_RUNTIME
            Singleton<GameWorld>.Instance.MainPlayer.ProceduralWeaponAnimation.ForceReact.AddForce(strength, recoilHands, recoilCamera);
#endif
        }
    }
}
