using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EW.Player
{
    using Interactions;
    using Items;
    
    public partial class PlayerInteractable
    {
        /// <summary>
        /// Called when the list of interactables changes.
        /// </summary>
        public static event System.Action<IEnumerable<InteractableBehaviour>, InteractableBehaviour> InteractableSelectionChanged;
        
        
        [Range(0, 1)]
        [Tooltip("The fraction of the interaction sphere that is used for interaction.")]
        [SerializeField] private float interactionSphereFraction = 0.1f;
        
        private Player player;
        public Player Player
        {
            get
            {
                if(!player)
                {
                    player = GetComponentInParent<Player>();
                }

                return player;
            }
        }
        
        public Inventory Inventory => Player.Inventory;
        
        /// <summary>
        /// This hashset contains all interactables that are within the interaction sphere,
        /// these are added and removed by OnTriggerEnter and OnTriggerExit.
        /// </summary>
        private HashSet<InteractableBehaviour> interactablesInRange;
        private List<InteractableBehaviour> selectableInteractables;
        
        private InteractableBehaviour activeInteractable;
        private InteractableBehaviour lastActiveInteractable;
        
        private InputAction interactAction;
        
        private void Awake()
        {
            interactAction = GameManager.Instance.InputMaster.Player.Interact;
        }

        private void OnEnable()
        {
            interactAction.performed += OnInteract;
        }
        
        private void OnDisable()
        {
            interactAction.performed -= OnInteract;
        }
        
        private void OnInteract(InputAction.CallbackContext _context)
        {
            if (activeInteractable)
            {
                activeInteractable.Interact(this);
            }
        }

        private void Update()
        {
            // Remove any interactables that are no longer interactable.
            interactablesInRange?.RemoveWhere(InteractableNotInteractable);

            // Order the remaining interactables based on how close they are to the camera's forward vector.
            HashSet<InteractableBehaviour> potentialInteractables = interactablesInRange?
                .OrderByDescending(GetDotForward)
                .ToHashSet();
            
            // Pick the interactable that is closest to the camera's forward vector.
            activeInteractable = potentialInteractables?
                .Where(InteractableInFraction)
                .FirstOrDefault();
            
            // Call the event to notify any listeners that the list of interactables has changed.
            // Useful to update the UI.
            InteractableSelectionChanged?.Invoke(potentialInteractables, activeInteractable);
            
            return;
            
            bool InteractableNotInteractable(InteractableBehaviour _interactable)
            {
                return !_interactable.gameObject.activeSelf || !_interactable.CanInteract(this);
            }
            
            bool InteractableInFraction(InteractableBehaviour _interactable)
            {
                return GetDotForward(_interactable) > 1 - interactionSphereFraction;
            }
            
            float GetDotForward(InteractableBehaviour _interactable)
            {
                Vector3 interactablePosition = _interactable.transform.position;
                interactablePosition.y = 0;
                
                // Because the position variable is readonly, use a scalar to ignore the height instead.
                Vector3 scalar = new(1, 0, 1);
                Vector3 cameraToObject = interactablePosition - Vector3.Scale(Camera.main.transform.position, scalar);
                Vector3 playerToObject = interactablePosition - Vector3.Scale(transform.position, scalar);

                return Vector3.Dot(cameraToObject.normalized, playerToObject.normalized);
            }
        }
        
        private void OnTriggerEnter(Collider _other)
        {
            if (!_other.gameObject.activeSelf)
            {
                return;
            }
            
            // Unlike OnCollisionEnter, OnTriggerEnter does not get called on parent rigidbody of a collider.
            // To work around this limitation, we will use GetComponentInParent<T> instead of GetComponent<T>.
            InteractableBehaviour interactable = _other.transform.GetComponentInParent<InteractableBehaviour>();
            if (interactable is null)
            {
                return;
            }

            if (interactable.CanInteract(this))
            {
                AddInteractable(interactable);
            }
        }

        private void OnTriggerExit(Collider _other)
        {
            InteractableBehaviour interactable = _other.transform.GetComponentInParent<InteractableBehaviour>();
            if (interactable is null)
            {
                return;
            }

            if (interactable.CanInteract(this))
            {
                RemoveInteractable(interactable);
            }
        }

        /// <summary>
        /// Add an interactable to the list of available interactables.
        /// Useful when an interactable is further away than the default interaction range.
        /// </summary>
        public void AddInteractable(InteractableBehaviour _interactable)
        {
            interactablesInRange ??= new HashSet<InteractableBehaviour>();
            
            interactablesInRange.Add(_interactable);
        }

        /// <summary>
        /// Remove the interactable from the list of available interactables.
        /// Useful when an interactable is disabled or destroyed.
        /// </summary>
        public void RemoveInteractable(InteractableBehaviour _interactable)
        {
            // The interactablesInRange list should exist at this point.
            interactablesInRange.Remove(_interactable);
        }
    }

    public partial class PlayerInteractable : InteractableBehaviour
    {
        public override bool CanInteract(InteractableBehaviour _other)
        {
            return false;
        }

        public override void Interact(InteractableBehaviour _interacting)
        {
            throw new System.NotImplementedException();
        }
    }
}
