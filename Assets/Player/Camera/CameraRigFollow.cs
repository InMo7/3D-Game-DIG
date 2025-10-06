using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position; // Remove offset here
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Comment out LookAt to avoid rotation conflicts
        // transform.LookAt(target.position);
    }
}