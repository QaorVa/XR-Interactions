using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : SimpleHingeInteractable
{
    public UnityEvent OnOpen;

    [SerializeField] private Transform objectToMove;
    private Vector3 objectInitialPos;
    [SerializeField] private CombinationLock combinationLock;
    [SerializeField] private Vector3 rotationLimits;

    [SerializeField] Collider closedCollider;
    private bool isClosed;
    private Vector3 startRotation;

    [SerializeField] Collider openedCollider;
    private bool isOpened;
    [SerializeField] private Vector3 endRotation;

    private float startAngleX;
    protected override void Start()
    {
        base.Start();

        startRotation = transform.localEulerAngles;
        startAngleX = GetAngle(startRotation.x);

        if (combinationLock != null)
        {
            combinationLock.UnlockAction += UnlockHinge;
            combinationLock.LockAction += LockHinge;
        }
        objectInitialPos = objectToMove.localEulerAngles;
        
    }

    protected override void Update()
    {
        base.Update();

        if(objectToMove != null)
        {
            objectToMove.localEulerAngles = new Vector3(objectToMove.localEulerAngles.x, transform.localEulerAngles.y, objectToMove.localEulerAngles.z);

        }

        if(isSelected)
        {
            CheckLimits();
        }

        
    }

    private void CheckLimits()
    {
        isClosed = false;
        isOpened = false;

        float localAngleX = GetAngle(transform.localEulerAngles.x);

        if(localAngleX >= startAngleX + rotationLimits.x || localAngleX <= startAngleX - rotationLimits.x)
        {
            ReleaseHinge();
            
        }
    }

    private float GetAngle(float angle)
    {
        if (angle >= 180)
        {
            angle -= 360;
        }
        return angle;
    }

    protected override void LockHinge()
    {
        base.LockHinge();
        transform.localEulerAngles = objectInitialPos;
        objectToMove.localEulerAngles = objectInitialPos;
    }

    protected override void ResetHinge()
    {
        if (isClosed)
        {
            transform.localEulerAngles = startRotation;
        } else if (isOpened) 
        { 
            transform.localEulerAngles = endRotation;
            OnOpen?.Invoke();
        
        } else
        {
            transform.localEulerAngles = new Vector3(startAngleX, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other == closedCollider)
        {
            isClosed = true;
            ReleaseHinge();
        } else if (other == openedCollider) 
        {
            isOpened = true;
            ReleaseHinge();
        }
    }
}
