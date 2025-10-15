using tarkin.SimpleTransformAnimation.Player;
using UnityEngine;

namespace tarkin.BSP.Shared.STA
{
    [RequireComponent(typeof(STAPlayer))]
    internal class STAAudioTrigger : MonoBehaviour
    {
        public PersistentAudioSource audioSource;

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
                audioSource.Play();
            }

            _previousTime = currentTime;
        }
    }
}
