using UnityEngine;

public class Follow_Drone : MonoBehaviour
{
    public Transform target;
    [Range(0f, 1f)]
    public float smoothTime = 0.3f;
    
    private Vector3 currentVelocity = Vector3.zero;
    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, fixedY, target.position.z);

        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref currentVelocity, 
            smoothTime
        );
    }
}