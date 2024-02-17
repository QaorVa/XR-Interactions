using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class XRPhysicsButtonInteractable : XRSimpleInteractable
{
    public UnityEvent OnBaseEnter;
    public UnityEvent OnBaseExit;

    [SerializeField] Collider baseCollider;

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(baseCollider == null)
        {
            Debug.Log("Base Collider is null");
            return;
        }

        if(isHovered && other == baseCollider)
        {
            Debug.Log("Button Pressed Enter");
            OnBaseEnter?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (baseCollider == null)
        {
            Debug.Log("Base Collider is null");
            return;
        }

        if (other == baseCollider)
        {
            Debug.Log("Button Pressed Exit");
            OnBaseExit?.Invoke();
        }
    }
}
