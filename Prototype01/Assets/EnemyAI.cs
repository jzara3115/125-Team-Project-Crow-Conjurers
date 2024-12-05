using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float shootDecisionInterval = 1f;
    public float behaviorSwitchInterval = 5f;
    public float speed = 2f;
    public float verticalAmplitude = 1f;
    public float horizontalAmplitude = 1f;

    private Vector3 behaviorStartPos;
    private float behaviorTimer;
    private float shootTimer;
    private int currentBehavior;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        behaviorStartPos = transform.position;
        behaviorTimer = behaviorSwitchInterval;
        shootTimer = shootDecisionInterval;
        currentBehavior = Random.Range(0, 3);
    }

    void Update()
    {
        behaviorTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;

        if (behaviorTimer <= 0f)
        {
            currentBehavior = Random.Range(0, 3);
            behaviorTimer = behaviorSwitchInterval;
            behaviorStartPos = transform.position;
        }

        switch (currentBehavior)
        {
            case 0:
                MoveUpAndDown();
                break;
            case 1:
                MoveLeftAndRight();
                break;
            case 2:
                FollowPlayer();
                break;
        }

        if (shootTimer <= 0f)
        {
            DecideToShoot();
            shootTimer = shootDecisionInterval;
        }
    }

    void MoveUpAndDown()
    {
        float newY = behaviorStartPos.y + Mathf.Sin(Time.time * speed) * verticalAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void MoveLeftAndRight()
    {
        float newX = behaviorStartPos.x + Mathf.Sin(Time.time * speed) * horizontalAmplitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void DecideToShoot()
    {
        int[] weights = { 50, 10, 10, 10, 10, 10 };
        int decision = GetWeightedRandom(weights);

        switch (decision)
        {
            case 1:
                StartCoroutine(ShootPattern(3, Vector3.zero));
                break;
            case 2:
                StartCoroutine(ShootPattern(2, Vector3.left));
                break;
            case 3:
                StartCoroutine(ShootPattern(2, Vector3.right));
                break;
            case 4:
                StartCoroutine(ShootPattern(2, Vector3.up));
                break;
            case 5:
                StartCoroutine(ShootPattern(2, Vector3.down));
                break;
            default:
                break;
        }
    }

    IEnumerator ShootPattern(int shots, Vector3 offset)
    {
        for (int i = 0; i < shots; i++)
        {
            Shoot(offset);
            yield return new WaitForSeconds(1f);
        }
    }

    void Shoot(Vector3 offset)
    {
        if (player == null || bulletPrefab == null) return;

        Vector3 targetPosition = player.position + offset;
        Vector3 direction = (targetPosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * bulletSpeed;
        }
    }

    int GetWeightedRandom(int[] weights)
    {
        int totalWeight = 0;
        foreach (int weight in weights) totalWeight += weight;

        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight) return i;
        }

        return 0;
    }
}
