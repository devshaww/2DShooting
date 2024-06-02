
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private int explosionDamage = 60;

    private int direction;

    public void SetDirection(int dir)
    {
        direction = dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TakeDamage(explosionDamage, direction);
        }
    }

    private void OnAnimationCompleted()
    {
        Destroy(gameObject);
    }
}
