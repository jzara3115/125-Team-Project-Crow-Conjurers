using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    private float yaw;
    private float pitch = 90f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        yaw += mouseX;
        transform.rotation = Quaternion.Euler(90f, yaw, 0f);

        //float vertical = Input.GetAxis("Vertical");
        //Debug.Log(vertical);
        //Vector3 movementDirection = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad), 0, Mathf.Cos(yaw * Mathf.Deg2Rad)) * vertical;
        pitch += Input.GetAxis("Vertical");
        pitch = Mathf.Clamp(pitch, -90, 90f);
        transform.rotation = Quaternion.Euler(pitch/2 + 90f, yaw, 0f);
        Vector3 movementDirection = new Vector3(Mathf.Sin(yaw * Mathf.Deg2Rad), Mathf.Sin(pitch * Mathf.Deg2Rad) * -1f, Mathf.Cos(yaw * Mathf.Deg2Rad));
        transform.position += movementDirection * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ring"))
        {
            GameManager.Instance.NumHoops++;
            Destroy(other.transform.parent.gameObject);
        }
    }
}
