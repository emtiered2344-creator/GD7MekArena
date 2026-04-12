using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5;
    private Vector3 direction;
    private float damage;
    private float speed;
    private Transform target;
    private float turnSpeed = 10f;

    void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            if (target == null)
            {
                target = null;
            }
            else
            {
                Vector3 desired = (target.position - transform.position).normalized;
                direction = Vector3.RotateTowards(direction, desired, turnSpeed * Time.deltaTime, 0f);
            }
        }

        if(target == null)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    public void SetTarget(Transform newTarget, float bulletDamage, float bulletSpeed, float turnSpeed)
    {
        Debug.Log("Target Set");
        target = newTarget;
        damage = bulletDamage;
        speed = bulletSpeed;
        this.turnSpeed = turnSpeed;
        if (target != null)
            direction = (target.position - transform.position).normalized;
    }

    void OnTriggerEnter(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if(character != null && character.isDashing == false)
        {
            character.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
