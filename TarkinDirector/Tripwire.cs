#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;

#if EFT_RUNTIME
using EFT.SynchronizableObjects;
using Comfort.Common;
using EFT.InventoryLogic;
using EFT;
using HarmonyLib;
using tarkin.Director.Bep.Patches;
#endif

namespace tarkin.Director
{
    public class Tripwire : MonoBehaviour
    {
        public Vector3 PosFrom => transform.position;
        public Vector3 PosTo;

        public string GrenadeGuid = "5e340dcdcb6d5863cc5e5efb";

#if EFT_RUNTIME
        TripwireSynchronizableObject realTripwire;

        void OnEnable()
        {
            Patch_TripwireSynchronizableObject_SetupGrenade.OnPostfix += Patch_TripwireSynchronizableObject_SetupGrenade_OnPostfix;

            Item item = Singleton<ItemFactoryClass>.Instance.GetPresetItem(GrenadeGuid);
            if (item == null)
            {
                return;
            }

            Singleton<GameWorld>.Instance.PlantTripwire(item, Singleton<GameWorld>.Instance.MainPlayer.ProfileId, PosFrom, PosTo);
        }

        private void Patch_TripwireSynchronizableObject_SetupGrenade_OnPostfix(TripwireSynchronizableObject realTripwire)
        {
            static bool ApproximatelyEquals(Vector3 self, Vector3 other, float epsilon)
            {
                if (epsilon < 0f) { epsilon = Mathf.Abs(epsilon); }

                return Mathf.Abs(self.x - other.x) <= epsilon &&
                       Mathf.Abs(self.y - other.y) <= epsilon &&
                       Mathf.Abs(self.z - other.z) <= epsilon;
            }

            // janky way to determine if the spawned tripwire is the one for this imposter
            if (ApproximatelyEquals(PosFrom, realTripwire.FromPosition, 0.001f))
            {
                this.realTripwire = realTripwire;
            }
        }

        void OnDisable()
        {
            Patch_TripwireSynchronizableObject_SetupGrenade.OnPostfix -= Patch_TripwireSynchronizableObject_SetupGrenade_OnPostfix;

            if (realTripwire == null)
                return;

            try
            {
                realTripwire.Reset();
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_firstStake").GetValue(realTripwire) as GameObject).SetActive(false);
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_firstStakeWireEnd").GetValue(realTripwire) as GameObject).SetActive(false);
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_secondStake").GetValue(realTripwire) as GameObject).SetActive(false);
                (AccessTools.Field(typeof(TripwireSynchronizableObject), "_line").GetValue(realTripwire) as TripwireProceduralMesh).gameObject.SetActive(false);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                realTripwire = null;
            }
        }
#endif

#if UNITY_EDITOR
        void Reset()
        {
            PosTo = PosFrom + new Vector3(1, 0, 0);
        }


        [DrawGizmo(GizmoType.Pickable | GizmoType.NonSelected)]
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            float height = 0.2f;

            Gizmos.DrawLine(PosFrom, PosFrom + new Vector3(0, height, 0));
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawDottedLine(PosFrom, PosTo, 1f);
            Gizmos.DrawLine(PosFrom + new Vector3(0, height, 0), PosTo + new Vector3(0, height, 0));
            Gizmos.DrawLine(PosTo, PosTo + new Vector3(0, height, 0));

            float crossSize = 0.05f;

            Gizmos.DrawLine(PosFrom - new Vector3(crossSize / 2f, 0, 0), PosFrom + new Vector3(crossSize / 2f, 0, 0));
            Gizmos.DrawLine(PosFrom - new Vector3(0, 0, crossSize / 2f), PosFrom + new Vector3(0, 0, crossSize / 2f));

            Gizmos.DrawLine(PosTo - new Vector3(crossSize / 2f, 0, 0), PosTo + new Vector3(crossSize / 2f, 0, 0));
            Gizmos.DrawLine(PosTo - new Vector3(0, 0, crossSize / 2f), PosTo + new Vector3(0, 0, crossSize / 2f));

            Gizmos.DrawWireSphere(PosFrom, 0.02f);
            Gizmos.DrawWireSphere(PosTo, 0.02f);

            Vector3 dirTo = (PosTo - PosFrom).normalized;
            Vector3 grenadePos = PosTo + new Vector3(0, height - 0.05f, 0) - dirTo * 0.05f;
            Gizmos.DrawCube(grenadePos, new Vector3(0.04f, 0.05f, 0.04f));
        }
#endif
    }
}