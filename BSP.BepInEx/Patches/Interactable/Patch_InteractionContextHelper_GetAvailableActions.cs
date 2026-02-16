using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using tarkin.BSP.Bep.Mediators.Interactable;

using InteractionContextHelper = GetActionsClass;
using IInteractive = GInterface177;

namespace tarkin.BSP.Bep.Patches.Interactable
{
    internal class Patch_InteractionContextHelper_GetAvailableActions : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(InteractionContextHelper), nameof(InteractionContextHelper.GetAvailableActions), [typeof(GamePlayerOwner), typeof(IInteractive)]);
        }

        [PatchPrefix]
        private static bool PatchPrefix(ref ActionsReturnClass __result, HideoutPlayerOwner owner, IInteractive interactive)
        {
            InteractableRelay relay = interactive as InteractableRelay;
            if (relay == null)
                return true;

            __result = relay.GetAvailableActions(owner);

            return false;
        }
    }
}
