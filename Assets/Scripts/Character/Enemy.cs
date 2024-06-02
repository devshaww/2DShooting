
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private FlashEffect flashEffect;
    [SerializeField]
    private GameObject explosionEffect;
    [SerializeField]
    private GameObject splatterEffect;
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float knockbackDistance = 0.1f;
    [SerializeField]
    private Vector2 knockbackForce = new Vector2(5, 5);
    [SerializeField]
    private float wallCheckDistance = 0.5f;
    [SerializeField]
    private float ledgeCheckDistance = 0.5f;
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private Transform wallCheck, ledgeCheck;
    [SerializeField]
    private LayerMask whatIsWall;
    [SerializeField]
    private LayerMask whatIsPlayer;

    private Rigidbody2D rb;
    private Animator anim;

    private int facingDirection = -1;
    private Coroutine sleepCoroutine;
    public bool isDead { get; private set; }
    private int hitDirection;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        anim.SetBool("isWalking", true);
    }

    private void CheckWall()
    {
        if (Physics2D.Raycast(wallCheck.position, Vector2.right * facingDirection, wallCheckDistance, whatIsWall))
        {
            Flip();
        }
    }

    private void CheckLedge()
    {
        if (!Physics2D.Raycast(ledgeCheck.position, Vector2.down, ledgeCheckDistance, whatIsWall))
        {
            Flip();
        }
    }

    void Update()
    {
        if (isDead) return;
        CheckOverlap();
        CheckWall();
        CheckLedge();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        ApplyMovement();
    }

    private void Flip()
    {
        facingDirection *= -1;
        rb.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void ApplyMovement()
    {
        rb.velocity = new Vector2(moveSpeed * facingDirection, rb.velocity.y);
    }

    private void CheckOverlap()
    {
        Collider2D hit = Physics2D.OverlapCapsule(transform.position, new Vector2(1.25f, 1.75f), CapsuleDirection2D.Vertical, 0f, whatIsPlayer);
        if (hit)
        {
            int direction = hit.transform.position.x - transform.position.x > 0 ? 1 : -1;
            hit.gameObject.GetComponent<Player>().TakeDamage(20, direction);
        }
    }

    private IEnumerator SleepCoroutine()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
    }

    private void Sleep()
    {
        if (sleepCoroutine != null)
        {
            StopCoroutine(sleepCoroutine);
        }
        sleepCoroutine = StartCoroutine(SleepCoroutine());
    }

    public void TakeStampedeDamage(int damageToGive)
    {
        if (isDead) return;
        health -= damageToGive;
        if (health <= 0)
        {
            anim.SetBool("isWalking", false);
            isDead = true;
            rb.velocity = Vector2.zero;
            rb.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else
        {
            Flash();
        }
    }

    // 受到攻击
    public void TakeDamage(int damageToGive, int direction)
    {
        if (isDead) return;
        hitDirection = direction;
        health -= damageToGive;
        if (health <= 0)  // die
        {
            anim.SetBool("isWalking", false);
            isDead = true;
            GameManager.Instance.UpdateData();
            Sleep();
            Explode();
            Splatter();
            Knockback();
        }
        else
        {
            Flash();
            SmallKnockback();
        }
    }

    private void Splatter()
    {
        int value = Random.Range(1, 10);
        if (value <= 2)
        {
            Instantiate(splatterEffect, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
        //Instantiate(splatterEffect, transform.position, Quaternion.Euler(0,0,Random.Range(0, 360)));
    }

    private void Flash()
    {
        flashEffect.Flash();
    }

    private void Explode()
    {
        int value = Random.Range(1, 10);
        if (value <= 3)
        {
            SoundManager.Instance.PlayExplosionSound(transform.position);
            GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
            explosion.GetComponent<ExplosionEffect>().SetDirection(hitDirection);
            CinemachineShake.Instance.ShakeCamera(8, .8f);
            Instantiate(splatterEffect, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360)));
        }
        //GameObject explosion = Instantiate(explosionEffect, transform.position, transform.rotation);
        //explosion.GetComponent<ExplosionEffect>().SetDirection(direction);
        //CinemachineShake.Instance.ShakeCamera(8, .8f);
    }

    private void SmallKnockback()
    {
        Vector3 knockbackVector = new Vector3(hitDirection * knockbackDistance, 0, 0);
        transform.position += knockbackVector;
    }

    private void Knockback()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(hitDirection * knockbackForce.x, knockbackForce.y), ForceMode2D.Impulse);
        //rb.transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -90), 1f);
        rb.transform.rotation = Quaternion.Euler(0, 0, -90*hitDirection);
    }


    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(wallCheck.position, wallCheck.position + new Vector3(facingDirection * wallCheckDistance, 0, 0));
    //    Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + new Vector3(0, -ledgeCheckDistance, 0));
    //}
}
