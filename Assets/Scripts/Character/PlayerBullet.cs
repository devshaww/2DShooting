
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private GameObject hitEffect;

    private Rigidbody2D rb;
    private int direction = 1;

    public void SetDirection(int shootDirection)
    {
        direction = shootDirection;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.isDead) return;
            enemy.TakeDamage(damage, direction);
        } else
        {
            Instantiate(hitEffect, collision.ClosestPoint(transform.position), collision.transform.rotation);
        }
        BulletObjectPool.Instance.AddToPool(gameObject);
    }

    private void OnBecameInvisible()
    {
        BulletObjectPool.Instance.AddToPool(gameObject);
    }
}
