using System;
using UnityEngine;
using System.Collections.Generic;


#if EFT_RUNTIME
using EFT;
#endif

namespace tarkin.Director.Shared
{
    [RequireComponent(typeof(Collider))]
    public class Treadmill : MonoBehaviour
    {
        [SerializeField] private Vector3 localDir;

#if EFT_RUNTIME
        HashSet<Player> passengers = new HashSet<Player>();
#endif

        void OnValidate()
        {
            gameObject.layer = 0;
            GetComponent<Collider>().isTrigger = true;
        }

        void LateUpdate()
        {
#if EFT_RUNTIME
            foreach (var passenger in passengers)
            {
                if (passenger != null && passenger.MovementContext != null)
                    passenger.MovementContext.PlatformMotion = transform.TransformDirection(localDir);
            }
#endif
        }

        void OnTriggerEnter(Collider col)
        {
#if EFT_RUNTIME
            if (col.gameObject.layer == LayerMaskClass.PlayerLayer &&
                col.gameObject.TryGetComponent<Player>(out Player player))
            {
                passengers.Add(player);
            }
#endif
        }

        void OnTriggerExit(Collider col)
        {
#if EFT_RUNTIME
            if (col.gameObject.layer == LayerMaskClass.PlayerLayer &&
                col.gameObject.TryGetComponent<Player>(out Player player))
            {
                passengers.Remove(player);
            }
#endif
        }
    }
}
