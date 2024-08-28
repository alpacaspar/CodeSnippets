using UnityEngine;

namespace EW.Interactions
{
    public abstract class InteractableBehaviour : MonoBehaviour
    {
        public Transform interactionPoint;
        
        public Transform InteractionPoint => interactionPoint ? interactionPoint : transform;
        
        /// <returns>Can this interactable interact with <paramref name="_other"/>?</returns>
        public abstract bool CanInteract(InteractableBehaviour _other);

        /// <param name="_interacting">The object interacting with this object.</param>
        public abstract void Interact(InteractableBehaviour _interacting);
    }
}
