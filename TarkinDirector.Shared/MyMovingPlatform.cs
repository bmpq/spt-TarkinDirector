using System;
using UnityEngine;
using System.Collections.Generic;


#if EFT_RUNTIME
using EFT;
#endif

namespace tarkin.Director
{
    [RequireComponent(typeof(Collider))]
    public class MyMovingPlatform : MonoBehaviour
    {
        Vector3 prevPos;

#if EFT_RUNTIME
        HashSet<Player> passengers = new HashSet<Player>();
#endif

        void Start()
        {
            prevPos = transform.position;

            GetComponent<Collider>().isTrigger = true;
        }

        void LateUpdate()
        {
            Vector3 delta = transform.position - prevPos;

#if EFT_RUNTIME
            foreach (var passenger in passengers)
            {
                if (passenger != null && passenger.MovementContext != null)
                    passenger.MovementContext.PlatformMotion = delta;
            }
#endif

            prevPos = transform.position;
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
