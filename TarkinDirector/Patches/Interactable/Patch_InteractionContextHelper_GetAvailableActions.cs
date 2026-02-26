using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using tarkin.Director.Interactable;

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
        private static bool PatchPrefix(ref ActionsReturnClass __result, GamePlayerOwner owner, IInteractive interactive)
        {
            CustomInteractable customInteractable = interactive as CustomInteractable;
            if (customInteractable == null)
                return true;

            __result = customInteractable.Injected_GetAvailableActions(owner);

            return false;
        }
    }
}
