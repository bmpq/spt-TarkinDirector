#if STA
using UnityEngine;
using tarkin.SimpleTransformAnimation.Player;

namespace tarkin.BSP.Shared.STA
{
    [RequireComponent(typeof(STAPlayer))]
    internal class STATrigger : AnimatorAction
    {
        public float triggerTime;

        private STAPlayer _staPlayer;
        private float _previousTime;

        void Awake()
        {
            _staPlayer = GetComponent<STAPlayer>();
        }

        void Update()
        {
            float currentTime = _staPlayer.CurrentTime;

            if (_previousTime <= triggerTime && currentTime > triggerTime)
            {
                Invoke();
            }

            _previousTime = currentTime;
        }
    }
}
#endif