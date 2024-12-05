using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float distanceFromPlayer = 5f;
    public float cameraHeight = 2f;
    public float rotationSpeed = 5f;

    private float currentYaw;
    private float currentPitch;

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = -Input.GetAxis("Mouse Y") * rotationSpeed;

        currentYaw += mouseX;
        currentPitch += mouseY;
        currentPitch = Mathf.Clamp(currentPitch, -20f, 45f);

        Vector3 targetPosition = player.position - Quaternion.Euler(currentPitch, currentYaw, 0) * Vector3.forward * distanceFromPlayer;
        targetPosition.y += cameraHeight;

        transform.position = targetPosition;
        transform.LookAt(player.position + Vector3.up * cameraHeight);
    }
}
