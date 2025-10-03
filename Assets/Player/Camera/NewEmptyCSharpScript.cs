using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Assign the player in the Inspector
    public Vector3 offset = new Vector3(0, 2, -5); // Adjust for third-person view
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = new Vector3(smoothedPosition.x, desiredPosition.y, smoothedPosition.z); // Lock Y to offset Y
        transform.LookAt(target.position);
    }
}