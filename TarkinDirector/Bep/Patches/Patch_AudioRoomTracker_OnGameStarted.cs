using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;


#if SPT3_11
using AudioRoomTrackerClass = GClass1068;
#else
using AudioRoomTrackerClass = GClass1122;
#endif

namespace tarkin.Director.Bep.Patches
{
    internal class Patch_AudioRoomTracker_RegisterAllRooms : ModulePatch
    {
        public static AudioRoomTrackerClass CurrentAudioRoomTracker { get; private set; }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(AudioRoomTrackerClass), nameof(AudioRoomTrackerClass.method_0));
        }

        [PatchPostfix]
        private static void PatchPostfix(AudioRoomTrackerClass __instance)
        {
            CurrentAudioRoomTracker = __instance;
        }

        // for future assembly changes, and roslyn validation (compile-time check)
        private struct Signature
        {
            Signature(AudioRoomTrackerClass validate)
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
