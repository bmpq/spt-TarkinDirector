#if EFT_RUNTIME
using EFT.Ballistics;
#endif

using UnityEngine;

namespace tarkin.Director
{
    [RequireComponent(typeof(Rigidbody))]
    internal class RigidbodyReactToBeingShot : MonoBehaviour
    {
        [SerializeField] private Transform ballisticCollidersParent;

        [SerializeField] private float hitForceFactor = 10f;

        Rigidbody rb;

#if EFT_RUNTIME
        private BallisticCollider[] subscribedBallisticColliders;

        void OnEnable()
        {
            rb = GetComponent<Rigidbody>();

            if (ballisticCollidersParent == null)
                subscribedBallisticColliders = transform.GetComponentsInChildren<BallisticCollider>();
            else
                subscribedBallisticColliders = ballisticCollidersParent.GetComponentsInChildren<BallisticCollider>();

            foreach (var ballisticCollider in subscribedBallisticColliders)
            {
                ballisticCollider.OnHitAction += OnBallisticHit;
            }
        }

        void OnBallisticHit(DamageInfoStruct dmgInfo)
        {
            rb.AddForceAtPosition(dmgInfo.Direction * hitForceFactor * dmgInfo.Damage, dmgInfo.HitPoint);
        }

        void OnDisable()
        {
            foreach (var ballisticCollider in subscribedBallisticColliders)
            {
                if (ballisticCollider == null)
                    continue;

                ballisticCollider.OnHitAction -= OnBallisticHit;
            }
        }
#endif
    }
}
