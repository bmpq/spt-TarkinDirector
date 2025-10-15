using Comfort.Common;
using EFT;
using System.Collections.Generic;
using Systems.Effects;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep.Mediators
{
    internal class SmokeGrenadeMediator
    {
        private Dictionary<Shared.SmokeGrenade, GrenadeEmission> instances = [];

        const string emissionEffect = "weapon_m18_world";

        public SmokeGrenadeMediator()
        {
            Shared.SmokeGrenade.OnRequestStart += SmokeGrenadeEmitter_OnRequestStart;
            Shared.SmokeGrenade.OnRequestStop += SmokeGrenadeEmitter_OnRequestStop;
        }

        private void SmokeGrenadeEmitter_OnRequestStart(Shared.SmokeGrenade requester)
        {
            if (instances.ContainsKey(requester))
            {
                Object.Destroy(instances[requester]);
                instances.Remove(requester);
            }

            GrenadeEmission grenadeEmission = Singleton<Effects>.Instance.GetEmissionEffect(emissionEffect);
            if (grenadeEmission == null)
            {
                return;
            }

            grenadeEmission.AttachTo(requester.transform, Vector3.zero);
            grenadeEmission.SetFillParams(0f, requester.EmitTime);
            grenadeEmission.StartEmission(0f);

            instances.Add(requester, grenadeEmission);
        }

        private void SmokeGrenadeEmitter_OnRequestStop(Shared.SmokeGrenade requester)
        {
            if (instances.ContainsKey(requester))
            {
                GrenadeEmission emission = instances[requester];
                instances.Remove(requester);

                emission.StopEmission(null);
            }
        }
    }
}
