using Comfort.Common;
using EFT;
using System.Collections.Generic;
using Systems.Effects;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep
{
    internal class SmokeGrenadeMediator
    {
        const string emissionEffect = "weapon_m18_world";

        public SmokeGrenadeMediator()
        {
            instances = new Dictionary<SmokeGrenadeImposter, GrenadeEmission>();

            SmokeGrenadeImposter.OnRequestStart += SmokeGrenadeEmitter_OnRequestStart;
            SmokeGrenadeImposter.OnRequestStop += SmokeGrenadeEmitter_OnRequestStop;
        }

        private Dictionary<SmokeGrenadeImposter, GrenadeEmission> instances;

        private void SmokeGrenadeEmitter_OnRequestStart(SmokeGrenadeImposter requester)
        {
            if (instances.ContainsKey(requester))
            {
                GameObject.Destroy(instances[requester]);
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

        private void SmokeGrenadeEmitter_OnRequestStop(SmokeGrenadeImposter requester)
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
