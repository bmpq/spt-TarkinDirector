using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using EFT.SynchronizableObjects;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tarkin.BSP.Bep.Patches;
using tarkin.BSP.Shared;
using UnityEngine;

namespace tarkin.BSP.Bep.Mediators
{
    internal class TripwireMediator
    {
        private Dictionary<Tripwire, TripwireSynchronizableObject> instances = [];

        public TripwireMediator() 
        {
            Tripwire.OnRequestSpawn += TripwireImposter_OnRequestSpawn;
            Tripwire.OnRequestRemove += TripwireImposter_OnRequestRemove;

            Patch_TripwireSynchronizableObject_SetupGrenade.OnPostfix += Patch_TripwireSynchronizableObject_SetupGrenade_OnPostfix;
        }

        private void Patch_TripwireSynchronizableObject_SetupGrenade_OnPostfix(TripwireSynchronizableObject realTripwire)
        {
            Tripwire imposter = null;
            foreach (var kvp in instances)
            {
                if (ApproximatelyEquals(kvp.Key.PosFrom, realTripwire.FromPosition, 0.001f))
                {
                    imposter = kvp.Key;
                    break;
                }
            }

            if (imposter != null)
            {
                instances[imposter] = realTripwire;
            }
        }

        public static bool ApproximatelyEquals(Vector3 self, Vector3 other, float epsilon)
        {
            if (epsilon < 0f) { epsilon = Mathf.Abs(epsilon); }

            return Mathf.Abs(self.x - other.x) <= epsilon &&
                   Mathf.Abs(self.y - other.y) <= epsilon &&
                   Mathf.Abs(self.z - other.z) <= epsilon;
        }

        private void TripwireImposter_OnRequestSpawn(Tripwire imposter)
        {
            Item item = Singleton<ItemFactoryClass>.Instance.GetPresetItem(imposter.GrenadeGuid);
            if (item == null)
            {
                return;
            }

            instances.Add(imposter, null);

            Singleton<GameWorld>.Instance.PlantTripwire(item, Singleton<GameWorld>.Instance.MainPlayer.ProfileId, imposter.PosFrom, imposter.PosTo);
        }

        private void TripwireImposter_OnRequestRemove(Tripwire imposter)
        {
            if (instances.ContainsKey(imposter))
            {
                TripwireSynchronizableObject tripwire = instances[imposter];

                tripwire.Reset();
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_firstStake").GetValue(tripwire) as GameObject)?.SetActive(false);
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_firstStakeWireEnd").GetValue(tripwire) as GameObject)?.SetActive(false);
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_secondStake").GetValue(tripwire) as GameObject)?.SetActive(false);
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_line").GetValue(tripwire) as TripwireProceduralMesh)?.gameObject.SetActive(false);


                instances.Remove(imposter);
            }
        }
    }
}
