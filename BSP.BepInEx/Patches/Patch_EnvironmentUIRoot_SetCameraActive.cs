using EFT;
using EFT.UI;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Reflection;
using UnityEngine;

namespace tarkin.BSP.Bep.Patches
{
    internal class Patch_EnvironmentUIRoot_SetCameraActive : ModulePatch
    {
        public static Transform CameraContainer { get; private set; }

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(EnvironmentUIRoot), nameof(EnvironmentUIRoot.SetCameraActive));
        }

        [PatchPostfix]
        private static void PatchPostfix(EnvironmentUIRoot __instance, bool value, Transform ___CameraContainer)
        {
            Plugin.Log.LogInfo($"{___CameraContainer}: {value}");
            CameraContainer = value ? ___CameraContainer : null;
        }
    }
}
