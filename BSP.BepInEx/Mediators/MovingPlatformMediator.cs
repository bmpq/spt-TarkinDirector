using EFT;
using System.Collections.Generic;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep.Mediators
{
    internal class MovingPlatformMediator
    {
        private Dictionary<MyMovingPlatform, HashSet<Player>> instances = [];

        public MovingPlatformMediator()
        {
            MyMovingPlatform.ActionOnTriggerEnter += OnTriggerEnter;
            MyMovingPlatform.ActionOnTriggerExit += OnTriggerExit;
            MyMovingPlatform.ActionLateUpdatePositionDelta += LateUpdatePositionDelta;
            MyMovingPlatform.ActionOnDestroy += RemovePlatform;
        }

        void OnTriggerEnter(MyMovingPlatform platform, Collider col)
        {
            if (col.gameObject.layer == LayerMaskClass.PlayerLayer && col.gameObject.TryGetComponent<Player>(out Player player))
            {
                if (!instances.ContainsKey(platform))
                    instances.Add(platform, new HashSet<Player>());

                instances[platform].Add(player);
            }
        }

        void OnTriggerExit(MyMovingPlatform platform, Collider col)
        {
            if (col.gameObject.layer == LayerMaskClass.PlayerLayer && col.gameObject.TryGetComponent<Player>(out Player player))
            {
                if (instances.ContainsKey(platform))
                    instances[platform].Remove(player);
            }
        }

        void RemovePlatform(MyMovingPlatform platform)
        {
            if (!instances.ContainsKey(platform))
                return;

            instances[platform].Clear();
            instances.Remove(platform);
        }

        void LateUpdatePositionDelta(MyMovingPlatform platform, Vector3 delta)
        {
            if (!instances.ContainsKey(platform))
                return;

            foreach (var passenger in instances[platform])
            {
                if (passenger != null && passenger.MovementContext != null)
                    passenger.MovementContext.PlatformMotion = delta;
            }
        }
    }
}