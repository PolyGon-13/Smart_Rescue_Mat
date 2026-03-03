using UnityEngine;

public class PushAndFollow : MonoBehaviour
{
    public Rigidbody targetRigidbody;
    public float pushForce = 10f;
    public float smoothTime = 0.3f;
    
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 initialTargetPos;
    private bool isTracking = false;
    private float fixedY;

    void Start()
    {
        if (targetRigidbody != null)
        {
            initialTargetPos = targetRigidbody.transform.position;
        }
        fixedY = transform.position.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            PushTarget();
        }

        if (!isTracking && targetRigidbody != null)
        {
            float distanceX = Mathf.Abs(targetRigidbody.transform.position.x - initialTargetPos.x);
            if (distanceX >= 15f)
            {
                isTracking = true;
            }
        }
    }

    void LateUpdate()
    {
        if (isTracking && targetRigidbody != null)
        {
            Vector3 targetPosition = new Vector3(targetRigidbody.position.x, fixedY, targetRigidbody.position.z);
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                targetPosition, 
                ref currentVelocity, 
                smoothTime
            );
        }
    }

    void PushTarget()
    {
        if (targetRigidbody != null)
        {
            targetRigidbody.AddForce(Vector3.left * pushForce, ForceMode.Impulse);
        }
    }
}