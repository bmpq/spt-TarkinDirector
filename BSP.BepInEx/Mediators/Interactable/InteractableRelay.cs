using EFT.Interactive;
using System;
using System.Collections.Generic;
using System.Linq;
using tarkin.BSP.Shared.Interactable;
using System.Text;
using System.Threading.Tasks;
using EFT;
using UnityEngine;

namespace tarkin.BSP.Bep.Mediators.Interactable
{
    public class InteractableRelay : InteractableObject
    {
        private InteractableLogic _logic;

        private readonly HashSet<PlayerOwner> _observers = new HashSet<PlayerOwner>();

        void NotifyStateChanged()
        {
            foreach (var owner in _observers)
            {
                if (owner != null && owner.Player != null)
                {
                    owner.Player.UpdateInteractionCast();
                }
            }
        }

        void Awake()
        {
            _logic = GetComponent<InteractableLogic>();
            if (_logic == null)
            {
                Plugin.Log.LogError($"InteractableRelay could not find its logic component on {gameObject.name}!");
                this.enabled = false;
            }

            _logic.OnStateChanged += NotifyStateChanged;

            
        }

        public ActionsReturnClass GetAvailableActions(PlayerOwner owner)
        {
            if (_logic == null) return null;

            if (owner != null && !_observers.Contains(owner))
            {
                _observers.Add(owner);
            }

            var actionsReturn = new ActionsReturnClass
            {
                Actions = new List<ActionsTypesClass>()
            };

            foreach (var action in _logic.GetActions())
            {
                actionsReturn.Actions.Add(new ActionsTypesClass
                {
                    Name = action.Name,
                    Action = new Action(() =>
                    {
                        Action updateCallback = () => owner.Player.UpdateInteractionCast();
                        _logic.ExecuteAction(action.Name, updateCallback);
                        owner.Player.SetInteractInHands((EInteraction)action.HandsAnimation);
                    }),
                    Disabled = action.IsDisabled,
                    TargetName = action.TargetName
                });
            }

            return actionsReturn.Actions.Any() ? actionsReturn : null;
        }


        // dogshit alert!!!
        private float _nextCleanupTime;
        private const float CleanupInterval = 1.0f;
        void Update()
        {
            if (Time.time > _nextCleanupTime)
            {
                _observers.RemoveWhere(owner =>
                    owner == null ||
                    owner.Player == null ||
                    owner.Player.InteractableObject != this
                );

                _nextCleanupTime = Time.time + CleanupInterval;
            }
        }
    }
}
