using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#if EFT_RUNTIME
using EFT.Interactive;
using EFT;
#endif

namespace tarkin.Director.Interactable
{
    [RequireComponent(typeof(Collider))]
    public abstract class CustomInteractable
#if EFT_RUNTIME
        : InteractableObject
#else
        : MonoBehaviour
#endif
    {

#if EFT_RUNTIME
        private readonly HashSet<PlayerOwner> observers = new HashSet<PlayerOwner>();

        private void Awake() 
        {
            gameObject.layer = LayerMask.NameToLayer("Interactive");

            NotifyStateChanged();
        }

        public ActionsReturnClass Injected_GetAvailableActions(PlayerOwner owner)
        {
            if (owner != null && !observers.Contains(owner))
                observers.Add(owner);

            var actionsReturn = new ActionsReturnClass
            {
                Actions = new List<ActionsTypesClass>()
            };

            foreach (var action in GetCustomActions())
            {
                actionsReturn.Actions.Add(new ActionsTypesClass
                {
                    Name = action.Name,
                    Action = new Action(() =>
                    {
                        Action updateCallback = () => owner.Player.UpdateInteractionCast();
                        ExecuteAction(action.Name, updateCallback);
                        owner.Player.SetInteractInHands((global::EInteraction)action.HandsAnimation);
                    }),
                    Disabled = action.IsDisabled,
                    TargetName = action.TargetName
                });
            }

            return actionsReturn.Actions.Any() ? actionsReturn : null;
        }

        // dogshit alert!!! works tho
        private float _nextCleanupTime;
        private const float CleanupInterval = 1.0f;
        void Update()
        {
            if (Time.time > _nextCleanupTime)
            {
                observers.RemoveWhere(owner =>
                    owner == null ||
                    owner.Player == null ||
                    owner.Player.InteractableObject != this
                );

                _nextCleanupTime = Time.time + CleanupInterval;
            }
        }
#endif

        public void NotifyStateChanged()
        {
#if EFT_RUNTIME
            foreach (var owner in observers)
            {
                if (owner != null && owner.Player != null)
                {
                    owner.Player.UpdateInteractionCast();
                }
            }
#endif
        }

        public abstract IEnumerable<CustomInteractableAction> GetCustomActions();
        public abstract void ExecuteAction(string actionName, Action finishCallback);
    }
}
