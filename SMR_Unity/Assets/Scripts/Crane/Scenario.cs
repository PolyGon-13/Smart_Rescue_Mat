using UnityEngine;
using System.Collections;

public class VehicleController : MonoBehaviour
{
    public Transform[] wheels;
    public Transform verticalMovingObject;
    public Transform childToParent;
    public Transform newParent;

    [Header("Speed Settings")]
    public float xMoveSpeed = 5.0f;
    public float yMoveSpeed = 1.0f;
    public float wheelRotationSpeed = 500f;

    private Vector3 currentVelocityX = Vector3.zero;
    private Vector3 currentVelocityY = Vector3.zero;
    private Vector3 lastPosition;
    private Vector3 startPosition;
    private Vector3 initialVerticalPos;
    
    private bool isXMoving = false;
    private bool isYMoving = false;
    private bool isReturning = false;
    private Vector3 targetYPos;

    void Start()
    {
        startPosition = transform.position;
        lastPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isXMoving = true;
        }

        if (isXMoving)
        {
            Vector3 targetX = isReturning ? startPosition : new Vector3(-14.24f, transform.position.y, transform.position.z);
            float smoothTime = 1.0f / xMoveSpeed;
            transform.position = Vector3.SmoothDamp(transform.position, targetX, ref currentVelocityX, smoothTime);

            if (Vector3.Distance(transform.position, targetX) < 0.001f)
            {
                transform.position = targetX;
                isXMoving = false;
                if (!isReturning) StartCoroutine(WaitAndMoveY());
            }
        }

        if (isYMoving)
        {
            float smoothTime = 1.0f / yMoveSpeed;
            verticalMovingObject.position = Vector3.SmoothDamp(verticalMovingObject.position, targetYPos, ref currentVelocityY, smoothTime);

            if (Mathf.Abs(verticalMovingObject.position.y - targetYPos.y) < 0.001f)
            {
                verticalMovingObject.position = targetYPos;
                isYMoving = false;
                
                if (!isReturning)
                {
                    ReparentObject();
                    StartCoroutine(ReturnSequence());
                }
            }
        }

        RotateWheels();
        lastPosition = transform.position;
    }

    void RotateWheels()
    {
        float deltaX = transform.position.x - lastPosition.x;
        
        if (Mathf.Abs(deltaX) > 0.00001f)
        {
            foreach (Transform wheel in wheels)
            {
                if (wheel != null)
                {
                    wheel.Rotate(Vector3.up, deltaX * wheelRotationSpeed, Space.Self);
                }
            }
        }
    }

    IEnumerator WaitAndMoveY()
    {
        yield return new WaitForSeconds(2f);
        initialVerticalPos = verticalMovingObject.position;
        targetYPos = new Vector3(verticalMovingObject.position.x, verticalMovingObject.position.y - 1.13064f, verticalMovingObject.position.z);
        isYMoving = true;
    }

    IEnumerator ReturnSequence()
    {
        yield return new WaitForSeconds(1f);
        targetYPos = initialVerticalPos;
        isYMoving = true;
        isReturning = true;

        while (isYMoving) yield return null;

        yield return new WaitForSeconds(0.5f);
        isXMoving = true;
    }

    void ReparentObject()
    {
        if (childToParent != null && newParent != null)
        {
            childToParent.SetParent(newParent, true);
        }
    }
}