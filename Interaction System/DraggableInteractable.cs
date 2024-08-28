using UnityEngine;

namespace EW.Interactions
{
    using Extensions;
    using Player;
    
    /// <summary>
    /// An implementation of an interactable that can be dragged by the player. (such as boxes).
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class DraggableInteractable : InteractableBehaviour
    {
        // Only one object is allowed to be dragged at a time
        public static DraggableInteractable ActiveInteractable { get; private set; }
        
        [SerializeField] private float dragForce = 5f;
        [SerializeField] private float maxDistance = 4f;
        [SerializeField] private float breakForce = 100f;

        private new Rigidbody rigidbody;
        
        // A custom function that performs a GetComponent only if the component is null,
        // this way, you can avoid race conditions that the Awake function might cause.
        private Rigidbody Rigidbody => gameObject.GetAndCacheComponent(ref rigidbody);

        private Transform target;
        
        private float defaultMass;
    
        private bool isInteracting;

        private void FixedUpdate()
        {
            if (!isInteracting)
            {
                return;
            }

            Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 targetPosition = target.position + cameraForward * 2.0f;
            Vector3 currentPosition = Rigidbody.position;
            currentPosition.y = targetPosition.y;
            Vector3 delta = targetPosition - currentPosition;

            if (delta.sqrMagnitude > maxDistance * maxDistance)
            {
                Detach();
                return;
            }
            
            Rigidbody.velocity = delta * dragForce;
        }

        private void OnCollisionEnter(Collision _other)
        {
            if (_other.relativeVelocity.sqrMagnitude > breakForce * breakForce)
            {
                Detach();
            }
        }

        public override bool CanInteract(InteractableBehaviour _other)
        {
            return _other is PlayerInteractable;
        }

        public override void Interact(InteractableBehaviour _interacting)
        {
            if (_interacting is not PlayerInteractable playerInteractable)
            {
                return;
            }
        
            isInteracting = !isInteracting;
            
            if (isInteracting)
            {
                if (ActiveInteractable)
                {
                    ActiveInteractable.Detach();
                    return;
                }
                
                Attach(playerInteractable.transform);
            }
            else
            {
                Detach();
            }
        }

        private void Attach(Transform _target)
        {
            ActiveInteractable = this;
            target = _target;
        }

        private void Detach()
        {
            ActiveInteractable = null;
            target = null;
            
            isInteracting = false;
        }
    }
}
