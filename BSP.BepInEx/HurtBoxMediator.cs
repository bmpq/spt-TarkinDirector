using Comfort.Common;
using EFT;
using System.Linq;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep
{
    internal class HurtBoxMediator
    {
        public HurtBoxMediator()
        {
            HurtBox.OnHit += HurtBox_OnHit;
        }

        private void HurtBox_OnHit(HurtBox hurtBox, Collider col)
        {
            BodyPartCollider bodyPart = col.GetComponent<BodyPartCollider>();
            if (bodyPart == null)
            {
                return;
            }

            DamageInfoStruct dmg = new DamageInfoStruct
            {
                DamageType = (EFT.EDamageType)(int)hurtBox.DamageType,
                Damage = hurtBox.DamageAmount,
                PenetrationPower = 100f,
                Direction = (hurtBox.transform.position - bodyPart.Collider.transform.position).normalized,
                HitCollider = bodyPart.Collider,
                HitNormal = Vector3.zero,
                HitPoint = bodyPart.Collider.transform.position,
                HittedBallisticCollider = bodyPart,
                IsForwardHit = true
            };

            bodyPart.ApplyHit(dmg, ShotIdStruct.EMPTY_SHOT_ID);
        }

    }
}
