using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace tarkin.BSP.Shared.Interactable
{
    public class Lootable : InteractableLogic
    {
        public string itemName;

        Coroutine routine;

        public override void ExecuteAction(string actionName, Action finishCallback)
        {
            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(Loot(finishCallback));
        }

        IEnumerator Loot(Action finishCallback)
        {
            yield return new WaitForSeconds(0.5f);

            gameObject.SetActive(false);

            finishCallback?.Invoke();
        }

        public override IEnumerable<InteractableAction> GetActions()
        {
            yield return InteractableAction.LootItem(itemName);
        }
    }
}
