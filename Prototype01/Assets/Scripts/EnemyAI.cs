using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

    public GameObject explosionPrefab;

    private int HP = 100;
    public Slider healthBar;

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

        if (healthBar != null)
        {
            healthBar.maxValue = HP;
            healthBar.value = HP;
        }
    }

    void Update()
    {
        behaviorTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;

        if (behaviorTimer <= 0f)
        {
            currentBehavior = Random.Range(0, 10);
            behaviorTimer = behaviorSwitchInterval;
            behaviorStartPos = transform.position;
        }

        if (currentBehavior < 2)
        {
            MoveUpAndDown();
        } else if(currentBehavior < 4)
        {
            MoveLeftAndRight();
        } else
        {
            FollowPlayer();
        }   

        if (shootTimer <= 0f)
        {
            DecideToShoot();
            shootTimer = shootDecisionInterval;
        }

        if (healthBar != null)
        {
            healthBar.value = HP;
        }
    }

    void MoveUpAndDown()
    {
        float newY = behaviorStartPos.y + Mathf.Sin(Time.time * speed) * verticalAmplitude;
        Vector3 newPosition = new Vector3(transform.position.x, newY, transform.position.z);

        FaceDirection(newPosition - transform.position); 
        transform.position = newPosition;
    }

    void MoveLeftAndRight()
    {
        float newX = behaviorStartPos.x + Mathf.Sin(Time.time * speed) * horizontalAmplitude;
        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);

        FaceDirection(newPosition - transform.position); 
        transform.position = newPosition;
    }

    void FollowPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            FaceDirection(direction); 
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void FaceDirection(Vector3 direction)
    {
        if (direction.magnitude > 0.01f) 
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
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

        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, Quaternion.identity);

        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.velocity = direction * bulletSpeed;
        }

        
        Destroy(bullet, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            HP -= 10; 
            if (healthBar != null)
            {
                healthBar.value = HP; 
                Debug.Log("Enemy hit!");

            }

            Destroy(other.gameObject); 
            if (HP <= 0)
            {
                Die();
            }
        }
    }

    void Die()
{
    if (explosionPrefab != null)
    {
        Debug.Log("Explosion prefab instantiated!");
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }

    Destroy(gameObject); // Destroy the enemy after spawning the explosion
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
