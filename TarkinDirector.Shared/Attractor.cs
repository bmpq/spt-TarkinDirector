using System;
using UnityEngine;
using System.Collections.Generic;

#if EFT_RUNTIME
using EFT;

using IPlatformTransportee = EFT.MovingPlatforms.MovingPlatform.GInterface459;
#endif

namespace tarkin.Director.Shared
{
    [RequireComponent(typeof(Collider))]
    public class Attractor : MonoBehaviour
    {
        [Tooltip("The base speed at which the player is pulled.")]
        [SerializeField] private float pullSpeed = 5f;

        [Tooltip("Prevents infinite speed at the exact center. Lower = faster at the center.")]
        [SerializeField] private float minDistanceClamp = 0.5f;

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
                {
                    Vector3 offset = transform.position - passenger.Position;
                    float distance = offset.magnitude;

                    // Only apply pull if they aren't exactly on the center pixel
                    if (distance > 0.01f)
                    {
                        // 1. Get the direction without distance scaling it
                        Vector3 direction = offset / distance; // mathematically same as offset.normalized

                        // 2. Inverse Distance: closer distance = higher multiplier
                        // We use Mathf.Max to stop the multiplier from shooting to infinity at distance 0
                        float strengthMultiplier = 1f / Mathf.Max(distance, minDistanceClamp);

                        // 3. Apply formula: Direction * Speed * DistanceMultiplier * FrameTime
                        passenger.MovementContext.PlatformMotion = direction * pullSpeed * strengthMultiplier * Time.deltaTime;
                    }
                    else
                    {
                        // Player is perfectly centered, stop pulling
                        passenger.MovementContext.PlatformMotion = Vector3.zero;
                    }
                }
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
                if (passengers.Contains(player))
                {
                    // Clean up PlatformMotion when exiting the field
                    if (player.MovementContext != null)
                    {
                        player.MovementContext.PlatformMotion = Vector3.zero;
                    }
                    passengers.Remove(player);
                }
            }
        }

        void OnDestroy()
        {
            passengers.Clear();
#endif
        }
    }
}