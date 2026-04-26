using UnityEngine;

public class Meatball : MonoBehaviour
{
    public float damage = 20f;
    public float lifetime = 10f;

    private float spawnTime;
    private bool isDestroying = false;

    void Start()
    {
        spawnTime = Time.time;
        Debug.Log($"Meatball spawned! Will destroy at {spawnTime + lifetime}");

        // Setup rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(Random.Range(-2f, 2f), -10f, Random.Range(-2f, 2f));
        }

        // Make sure collider is set up for damage
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = false;
            Debug.Log($"Meatball collider found: {col.GetType().Name}");
        }

        // Start destruction timer
        Invoke("DestroyMeatball", lifetime);
    }

    void Update()
    {
        // Backup destruction check
        if (!isDestroying && Time.time >= spawnTime + lifetime)
        {
            DestroyMeatball();
        }

        // Debug countdown in last 3 seconds
        float timeLeft = (spawnTime + lifetime) - Time.time;
        if (timeLeft > 0 && timeLeft <= 3f)
        {
            if (Mathf.FloorToInt(timeLeft) != Mathf.FloorToInt(timeLeft + Time.deltaTime))
            {
                Debug.Log($"⏰ Meatball will disappear in {Mathf.CeilToInt(timeLeft)} seconds!");
            }
        }
    }

    void DestroyMeatball()
    {
        if (isDestroying) return;
        isDestroying = true;

        Debug.Log($"DESTROYING MEATBALL at {Time.time}!");
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        Debug.Log($"Meatball DESTROYED at {Time.time}!");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Meatball collided with: {collision.gameObject.name}");

        Character character = collision.gameObject.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(damage);
            Debug.Log($"DAMAGE! {collision.gameObject.name} took {damage} damage!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Meatball trigger with: {other.gameObject.name}");

        Character character = other.gameObject.GetComponent<Character>();
        if (character != null)
        {
            character.TakeDamage(damage);
            Debug.Log($"DAMAGE! {other.gameObject.name} took {damage} damage!");
        }
    }
}