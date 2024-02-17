using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;

public class ProgressControl : MonoBehaviour
{
    public UnityEvent<string> OnStartGame;
    public UnityEvent<string> OnChallengeComplete;

    [Header("Start Button")]
    [SerializeField] XRButtonInteractable startButton;
    [SerializeField] private GameObject keyLight;

    [Header("Drawer Interactable")]
    [SerializeField] DrawerInteractable drawer;
    XRSocketInteractor drawerSocket;

    [Header("Combo Lock")]
    [SerializeField] CombinationLock comboLock;

    [Header("The Wall")]
    [SerializeField] TheWall wall;
    XRSocketInteractor wallSocket;

    [Header("Library")]
    [SerializeField] SimpleSliderControl librarySlider;

    [Header("The Robot")]
    [SerializeField] NavMeshRobot robot;

    [Header("Challenge Settings")]
    [SerializeField] string startGameString;
    [SerializeField] string challengeCompleteString;
    [SerializeField] string[] challengeStrings;
    [SerializeField] int wallCubesToDestroy;
    private int wallCubesDestroyed;
    private bool startGameBool;
    private bool challengesCompletedBool;
    [SerializeField] private int challengeNumber;

    // Start is called before the first frame update
    void Start()
    {
        if (startButton != null)
        {
            startButton.selectEntered.AddListener(OnStartButtonPressed);
        }
        OnStartGame?.Invoke(startGameString);
        SetDrawerInteractable();
        if(comboLock != null)
        {
            comboLock.UnlockAction += OnComboUnlocked;
        }

        if(wall != null)
        {
            SetWall();
        }

        if(librarySlider != null)
        {
            librarySlider.OnSliderActive.AddListener(LibrarySliderActive);
        }

        if(robot != null)
        {
            robot.OnDestroyWallCube.AddListener(OnDestroyWallCube);
        }
    }

    

    private void ChallengeComplete()
    {
        challengeNumber++;
        if (challengeNumber < challengeStrings.Length)
        {
            OnChallengeComplete?.Invoke(challengeStrings[challengeNumber]);
        }
        else if(challengeNumber >= challengeStrings.Length)
        {
            OnChallengeComplete?.Invoke(challengeCompleteString);
        }
        
    }

    private void OnStartButtonPressed(SelectEnterEventArgs arg0)
    {
        if(!startGameBool)
        {
            startGameBool = true;
            Debug.Log("***Start button pressed");
            if (keyLight != null)
            {
                keyLight.SetActive(true);
            }
            if(challengeNumber < challengeStrings.Length && challengeNumber == 0)
            {
                OnStartGame?.Invoke(challengeStrings[challengeNumber]);
            }

        }
        
        
        
    }
    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        if(challengeNumber == 0)
        {
            ChallengeComplete();
        }
        
    }

    private void OnDrawerDetach()
    {
        if (challengeNumber == 1)
        {
            ChallengeComplete();
        }
    }

    private void OnComboUnlocked()
    {
        if (challengeNumber == 2)
        {
            ChallengeComplete();
        }
    }
    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        if (challengeNumber == 3)
        {
            ChallengeComplete();
        }
    }
    private void OnDestroyWall()
    {
        if (challengeNumber == 4)
        {
            ChallengeComplete();
        }
    }
    private void LibrarySliderActive()
    {
        if (challengeNumber == 5)
        {
            ChallengeComplete();
        }
    }

    private void OnDestroyWallCube()
    {
        wallCubesDestroyed++;
        if (wallCubesDestroyed >= wallCubesToDestroy && !challengesCompletedBool && challengeNumber == 6)
        {
            challengesCompletedBool = true;
            ChallengeComplete();
        }
    }

    private void SetDrawerInteractable()
    {
        drawer.OnDrawerDetached.AddListener(OnDrawerDetach);
        if(drawer != null)
        {
            drawerSocket = drawer.GetKeySocket;
            if(drawerSocket != null)
            {
                drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
                
            }
            
        }
    }

    private void SetWall()
    {
        wall.OnDestroy.AddListener(OnDestroyWall);

        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }



}
