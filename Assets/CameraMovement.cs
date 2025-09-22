using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;    // The player cube
    public Vector3 offset = new Vector3(0, 5, -10); // Position offset
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Smoothly move the camera towards the target position + offset
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Look at the player
        transform.LookAt(target);
    }
}
