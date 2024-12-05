using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 5f;
    private float yaw;
    private float pitch = 90f;

    public GameObject bullet;
    public float shootForce, upwardForce;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    bool shooting, readyToShoot, reloading;
    public Camera fpsCam;
    public Transform attackPoint;
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    public bool allowInvoke = true;

    private Rigidbody rb;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody missing from player!");
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yaw = transform.eulerAngles.y;

        if (bullet != null)
        {
            Renderer bulletRenderer = bullet.GetComponent<Renderer>();
            if (bulletRenderer != null)
            {
                bulletRenderer.enabled = false;
            }
        }
    }

    void Update()
    {
        HandleRotation();
        MyInput();

        if (ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleRotation()
{
    float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
    yaw += mouseX;

    float lookUpSpeed = 2.5f; 
    float verticalInput = Input.GetAxis("Vertical") * lookUpSpeed;
    pitch += verticalInput;
    pitch = Mathf.Clamp(pitch, -90, 90f);

    transform.rotation = Quaternion.Euler(pitch / 2 + 90f, yaw, 0f);
}


    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 forwardDirection = new Vector3(
                Mathf.Sin(yaw * Mathf.Deg2Rad),
                Mathf.Sin(pitch * Mathf.Deg2Rad) * -1f,
                Mathf.Cos(yaw * Mathf.Deg2Rad)
            );

            rb.velocity = forwardDirection.normalized * moveSpeed;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ring"))
        {
            GameManager.Instance.NumHoops++;
            Destroy(other.transform.parent.gameObject);
        }
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();
        if (readyToShoot && shooting && !reloading && bulletsLeft <= 0) Reload();

        if (readyToShoot && shooting)
        {
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        Ray ray = fpsCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(75);

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        Renderer bulletRenderer = currentBullet.GetComponent<Renderer>();
        if (bulletRenderer != null)
        {
            bulletRenderer.enabled = true;
        }

        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        currentBullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse);

        
        Destroy(currentBullet, 3f);
        //Debug.Log("Destroying bullet: " + currentBullet.name);
        Destroy(currentBullet, 3f);

        if (muzzleFlash != null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        bulletsLeft--;
        bulletsShot++;

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        if (bulletsShot < bulletsPerTap && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
}


    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
