using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace tarkin.BSP.Bep.Patches
{
    internal class Patch_AudioRoomTracker_RegisterAllRooms : ModulePatch
    {
        public static GClass1122 CurrentAudioRoomTracker { get; private set; }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass1122), nameof(GClass1122.method_0));
        }

        [PatchPostfix]
        private static void PatchPostfix(GClass1122 __instance)
        {
            CurrentAudioRoomTracker = __instance;
        }

        // for future assembly changes, and roslyn validation (compile-time check)
        private struct Signature
        {
            Signature(GClass1122 validate)
            {
                var _ = (
                    validate.ListenerCurrentOutdoorRoomID,
                    validate.ListenerCurrentRoom,
                    validate.ListenerCurrentRoomID
                    );

                validate.RemovePlayerCurrentRoom(new SpatialAudioRoom(), new Player());
                validate.GetOtherPlayerCurrentRoom(0);
            }
        }
    }
}
