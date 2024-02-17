using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPlayerControl : MonoBehaviour
{
    [SerializeField] GrabMoveProvider[] grabMovers;
    [SerializeField] Collider[] grabColliders;

    private void OnTriggerEnter(Collider other)
    {
        for(int i = 0; i < grabColliders.Length; i++)
        {

            if(other == grabColliders[i])
            {
                Debug.Log("Collided with " + grabColliders[i].name);
                SetGrabMovers(true);
            }
        }
    }

    private void SetGrabMovers(bool value)
    {
        foreach (GrabMoveProvider grabMove in grabMovers)
        {
            grabMove.enabled = value;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < grabColliders.Length; i++)
        {
            if (other == grabColliders[i])
            {
                SetGrabMovers(false);
            }
        }
    }
}
