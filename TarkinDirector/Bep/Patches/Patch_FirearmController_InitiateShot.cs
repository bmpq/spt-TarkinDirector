using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Linq;
using System.Reflection;

namespace tarkin.Director.Bep.Patches
{
    internal class Patch_FirearmController_InitiateShot : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Player.FirearmController), nameof(Player.FirearmController.InitiateShot));
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance, IWeapon weapon, AmmoItemClass ammo)
        {
            if (Singleton<GameWorld>.Instance.MainPlayer.HandsController != __instance)
                return;

            Weapon weap = weapon as Weapon;
            if (weap == null)
                return;

            var currentMagazine = weap.GetCurrentMagazine();

            if (currentMagazine is CylinderMagazineItemClass cylinder)
            {
                foreach (var camora in cylinder.Camoras)
                {
                    if (camora.ContainedItem == null)
                    {
                        if (TryCreateItem(ammo.TemplateId, out Item newItem))
                        {
                            camora.AddWithoutRestrictions(newItem);
                        }
                    }
                }
            }
            else if (currentMagazine != null && currentMagazine.Cartridges != null)
            {
                var topAmmoItem = currentMagazine.Cartridges.Items.LastOrDefault();
                if (topAmmoItem != null)
                {
                    topAmmoItem.StackObjectsCount = Math.Min(topAmmoItem.Template.StackMaxSize, currentMagazine.MaxCount);
                }
                else
                {
                    if (TryCreateItem(ammo.TemplateId, out Item newItem))
                    {
                        newItem.StackObjectsCount = currentMagazine.MaxCount;
                        currentMagazine.Cartridges.Add(newItem, simulate: false);
                    }
                }
            }
            else
            {
                foreach (var chamber in weap.Chambers)
                {
                    if (chamber.ContainedItem == null)
                    {
                        if (TryCreateItem(ammo.TemplateId, out Item newItem))
                        {
                            chamber.AddWithoutRestrictions(newItem);
                        }
                    }
                }
            }
        }

        private static bool TryCreateItem(string templateId, out Item newItem)
        {
            newItem = null;

            if (!Singleton<ItemFactoryClass>.Instantiated)
                return false;

            if (!Singleton<ItemFactoryClass>.Instance.ItemTemplates.ContainsKey(templateId))
                return false;

            newItem = Singleton<ItemFactoryClass>.Instance.CreateItem(MongoID.Generate(), templateId, itemDiff: null);

            return newItem != null;
        }
    }
}
