using EFT;
using EFT.SynchronizableObjects;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;

namespace tarkin.Director.Patches
{
    internal class Patch_TripwireSynchronizableObject_SetupGrenade : ModulePatch
    {
        public static event Action<TripwireSynchronizableObject> OnPostfix;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TripwireSynchronizableObject), nameof(TripwireSynchronizableObject.SetupGrenade));
        }

        [PatchPostfix]
        private static void PatchPostfix(TripwireSynchronizableObject __instance)
        {
            OnPostfix?.Invoke(__instance);
        }
    }
}
