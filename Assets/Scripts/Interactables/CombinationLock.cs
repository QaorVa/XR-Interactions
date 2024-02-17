using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class CombinationLock : MonoBehaviour
{
    public UnityAction UnlockAction;
    private void OnUnlocked() => UnlockAction?.Invoke();

    public UnityAction LockAction;
    private void OnLocked() => LockAction?.Invoke();
    public UnityAction ComboButtonPressed;
    private void OnComboButtonPress() => ComboButtonPressed?.Invoke();
    

    [SerializeField] XRButtonInteractable[] comboButtons;
    [SerializeField] TMP_Text infoText;
    private const String Start_String = "Enter 3 Digits Combo";
    private const String Reset_String = "Enter 3 Digits To Reset Combo";

    [SerializeField] TMP_Text inputText;

    [SerializeField] Image lockedPanel;
    [SerializeField] Color unlockedColor;
    [SerializeField] TMP_Text lockedText;

    [SerializeField] bool isLocked = true;
    [SerializeField] bool isResettable;
    private bool resetCombo;

    [SerializeField] int[] comboValues = new int[3];
    [SerializeField] int[] inputValues;
    [SerializeField] AudioClip lockComboClip;
    public AudioClip GetLockClip => lockComboClip;
    [SerializeField] AudioClip unlockComboClip;
    public AudioClip GetUnlockClip => unlockComboClip;
    [SerializeField] AudioClip comboButtonPressedClip;
    public AudioClip GetComboButtonClip => comboButtonPressedClip;
    private int maxButtonPresses;
    private int buttonPresses;

    // Start is called before the first frame update
    void Start()
    {
        maxButtonPresses = comboValues.Length;
        inputValues = new int[comboValues.Length];

        inputText.text = "";

        for(int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }

    }

    private void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {
        if (!isLocked && !isResettable)
        {
            return;
        }
        if (buttonPresses == 0)
        {
            inputText.text = "";
        }

        if(buttonPresses >= maxButtonPresses)
        {
            //Too many button presses
        } else
        {
            for (int i = 0; i < comboButtons.Length; i++)
            {

                if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
                {
                    Debug.Log("Combo button " + i + " pressed");

                    inputText.text += i.ToString();
                    inputValues[buttonPresses] = i;

                }
                else
                {
                    comboButtons[i].ResetColor();
                }

            }
            buttonPresses++;

            if(buttonPresses == maxButtonPresses)
            {
                CheckCombo();

            } else
            {
                OnComboButtonPress();
            }
        }

        
    }

    private void CheckCombo()
    {
        if (resetCombo)
        {
            resetCombo = false;
            LockCombo();
            return;
        }

        int matches = 0;

        for(int i = 0; i < maxButtonPresses; i++)
        {
            if (inputValues[i] == comboValues[i])
            {
                matches++;
            }

        }

        if(matches == maxButtonPresses)
        {
            Debug.Log("Combo success");
            UnlockCombo();
            
        } else
        {
            Debug.Log("Combo failed");
            inputText.text = "Wrong Code";
            ResetUserValue();
        }
    }

    private void UnlockCombo()
    {
        isLocked = false;
        lockedPanel.color = unlockedColor;
        lockedText.text = "Unlocked";
        OnUnlocked();
        
        if(isResettable)
        {
            ResetCombo();
        }
    }

    private void LockCombo()
    {
        isLocked = true;
        lockedPanel.color = Color.red;
        lockedText.text = "Locked";
        infoText.text = Start_String;
        OnLocked();
        for (int i = 0; i < maxButtonPresses; i++)
        {
            comboValues[i] = inputValues[i];
        }
        ResetUserValue();
    }

    private void ResetCombo()
    {
        infoText.text = Reset_String;
        ResetUserValue();
        resetCombo = true;
    }

    private void ResetUserValue()
    {
        if(isLocked)
        {
            OnLocked();
        }

        inputValues = new int[comboValues.Length];
        
        buttonPresses = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
