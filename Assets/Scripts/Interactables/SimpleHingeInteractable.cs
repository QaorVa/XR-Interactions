using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class SimpleHingeInteractable : XRSimpleInteractable
{
    public UnityEvent<SimpleHingeInteractable> OnHingeSelected;

    [SerializeField] private Vector3 positionLimits;

    private Transform grabHand;
    private Collider hingeCollider;
    private Vector3 hingePositions;

    [SerializeField] bool isLocked = true;
    [SerializeField] AudioClip hingeMoveClip;
    public AudioClip GetHingeMoveClip => hingeMoveClip;

    private const string Default_Layer = "Default";
    private const string Grab_Layer = "Grab";
    // Start is called before the first frame update
    protected virtual void Start()
    {
        hingeCollider = GetComponent<Collider>();
        
    }

    protected void UnlockHinge()
    {
        isLocked = false;
    }
    protected virtual void LockHinge()
    {
        isLocked = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(grabHand != null)
        {
            TrackHand();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if(!isLocked)
        {
            base.OnSelectEntered(args);
            grabHand = args.interactorObject.transform;
            OnHingeSelected?.Invoke(this);
        }

    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        grabHand = null;

        ChangeLayerMask(Grab_Layer);
        ResetHinge();
    }

    private void TrackHand()
    {
        transform.LookAt(grabHand, transform.forward);
        hingePositions = hingeCollider.bounds.center;
        if(grabHand.position.z >= hingePositions.z + positionLimits.z || grabHand.position.z <= hingePositions.z - positionLimits.z)
        {
            ReleaseHinge();
            Debug.Log("***Release Hinge On Z");
        } else if(grabHand.position.y >= hingePositions.y + positionLimits.y || grabHand.position.y <= hingePositions.y - positionLimits.y)
        {
            ReleaseHinge();
            Debug.Log("***Release Hinge On Y");
        } else if(grabHand.position.x >= hingePositions.x + positionLimits.x || grabHand.position.x <= hingePositions.x - positionLimits.x)
        {
            ReleaseHinge();
            Debug.Log("***Release Hinge On X");
        }
    }

    public void ReleaseHinge()
    {
        ChangeLayerMask(Default_Layer);
    }

    protected abstract void ResetHinge();
    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
