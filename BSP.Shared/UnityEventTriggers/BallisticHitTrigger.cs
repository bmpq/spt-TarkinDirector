#if ASSDEP
using EFT.Ballistics;
#endif

using UnityEngine;
using UnityEngine.Events;

namespace tarkin.BSP.Shared.UnityEventTriggers
{
    internal class BallisticHitTrigger : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onHit;

#if ASSDEP
        private BallisticCollider ballisticCollider;

        private void Awake()
        {
            ballisticCollider = GetComponent<BallisticCollider>();
        }

        private void OnEnable()
        {
            if (ballisticCollider != null)
            {
                ballisticCollider.OnHitAction += HandleHit;
            }
        }

        private void OnDisable()
        {
            if (ballisticCollider != null)
            {
                ballisticCollider.OnHitAction -= HandleHit;
            }
        }

        private void HandleHit(DamageInfoStruct damageInfo)
        {
            if (onHit != null)
            {
                onHit.Invoke();
            }
        }
#endif
    }
}
