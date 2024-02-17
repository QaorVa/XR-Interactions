using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class XRButtonInteractable : XRSimpleInteractable
{

    [SerializeField] Image buttonImage;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightedColor;
    [SerializeField] private Color pressedColor;
    [SerializeField] private Color selectedColor;

    private bool isPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        ResetColor();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        isPressed = false;
        buttonImage.color = highlightedColor;
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        if(!isPressed)
        {
            buttonImage.color = normalColor;
        }
        
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        Debug.Log("***Button pressed");
        buttonImage.color = pressedColor;
        isPressed = true;

    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        buttonImage.color = selectedColor;
    }

    public void ResetColor()
    {
        buttonImage.color = normalColor;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
