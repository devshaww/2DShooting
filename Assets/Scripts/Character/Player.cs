
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[SerializeField]
    //private float standardMoveSpeed;
    [SerializeField]
    private LayerMask whatIsEnemy;
    [SerializeField]
    private FlashEffect flashEffect;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    public Transform firePoint;
    [SerializeField]
    public Transform groundCheck;
    [SerializeField]
    private float timeBetweenShots;
    [SerializeField]
    private float knockbackDistance = 0.1f;
    [SerializeField]
    private GameObject shellPrefab;
    [SerializeField]
    private Transform shellFirePoint;
    [SerializeField]
    private Transform stampedePoint;
    [SerializeField]
    private LayerMask whatIsStampedable;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private int health = 100;
    [SerializeField]
    private Vector2 knockbackForce = new Vector2(10, 20);

    private Rigidbody2D rb;
    private Animator anim;

    public int movingDirection = 1;
    //public int shootDirection = 1;
    private int xInput;
    public float groundCheckDistance = 0.5f;   //
    private bool jumpInput;
    private float timer;
    private bool isDead;
    private float hitCooldown = .5f;
    private float hitTimer;
    private int maxJumpCount = 2;
    private int remainingJump = 2;

    public static Player Instance;

    // 是否处于自动射击状态
    //private bool isFiring;
    //public float moveSpeed;
    //private float moveSpeedWhenFiring;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        Instance = this;
    }

    private void Start()
    {
        //moveSpeed = standardMoveSpeed;
        //moveSpeedWhenFiring = standardMoveSpeed * 0.8f;
    }

    private IEnumerator SlowMotionCoroutine()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(3);
        Time.timeScale = 1f;
        GameManager.Instance.GameOver();
    }

    public void TakeDamage(int damage, int direction)
    {
        if (isDead) return;
        hitTimer -= Time.deltaTime;
        if (hitTimer < 0)
        {
            hitTimer = hitCooldown;
            health -= damage;
            if (health <= 0)
            {
                SoundManager.Instance.PlayGameLoseSound(transform.position);
                isDead = true;
                rb.drag = 1;
                DeathKnockback(direction);
                StartCoroutine(SlowMotionCoroutine());
            }
            else
            {
                SoundManager.Instance.PlayPlayerHurtSound(transform.position);
                flashEffect.Flash();
            }
        }
    }

    private void CheckInput()
    {
        xInput = (int)Input.GetAxisRaw("Horizontal");
    }

    private void CheckStampede()
    {
        Collider2D hit = Physics2D.OverlapCapsule(stampedePoint.position, new Vector2(0.6f, 0.078f), CapsuleDirection2D.Horizontal, 0f, whatIsStampedable);
        if (hit)
        {
            Enemy enemy = hit.gameObject.GetComponentInParent<Enemy>();
            if (enemy.isDead) return;
            SoundManager.Instance.PlayStampedeSound(transform.position);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            enemy.TakeStampedeDamage(10);
        }
    }

    private void CheckGrounded() {
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround)) {
            remainingJump = maxJumpCount;
        }
    }

    void Update()
    {
        if (isDead) return;
        if (GameManager.Instance.IsGameOver()) return;
        CheckInput();
        CheckGrounded();
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        CheckIfShouldFlip();
        CheckFire();
        CheckStampede();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (GameManager.Instance.IsGameOver())
        {
            rb.velocity = Vector2.zero;
            return;
        }
        ApplyMovement();
    }

    private void UpdateAnimation()
    {
        if (xInput != 0)
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    private void CheckIfShouldFlip()
    {
        //if (xInput != 0 && (xInput != movingDirection || (!isFiring && movingDirection != shootDirection)))
        //{
        //    Flip();
        //}
        if (xInput != 0 && xInput != movingDirection)
        {
            Flip();
        }
    }

    private void ApplyMovement()
    {
        if (xInput != 0)
        {
            rb.velocity = new Vector2(moveSpeed * movingDirection, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void Flip()
    {
        movingDirection *= -1;
        rb.transform.Rotate(0.0f, 180.0f, 0.0f);
        //Debug.Log(firePoint.rotation.eulerAngles);
        //if (movingDirection < 0)
        //{
        //    rb.transform.rotation = Quaternion.Euler(0, 180, 0);
        //} else
        //{
        //    rb.transform.rotation = Quaternion.Euler(0, 0, 0);
        //}
        //if (!isFiring)
        //{
        //    shootDirection = movingDirection;
        //    if (movingDirection < 0)
        //    {
        //        rb.transform.rotation = Quaternion.Euler(0, 180, 0);
        //    } else
        //    {
        //        rb.transform.rotation = Quaternion.Euler(0, 0, 0);
        //    }
        //    //rb.transform.Rotate(0.0f, 180.0f, 0.0f);
        //} else
        //{
        //    shootDirection = -movingDirection;
        //}
    }

    private void Fire()
    {
        SoundManager.Instance.PlayShootSound(transform.position);
        SpawnBullets();
        timer = timeBetweenShots;
        anim.SetTrigger("Shoot");
        CinemachineShake.Instance.ShakeCamera(2f, .2f);
        Knockback();
        GameObject gameObject = Instantiate(shellPrefab, shellFirePoint.position, Quaternion.Euler(0, 0, Random.Range(120f, 160f)));
        gameObject.GetComponent<Shell>().Initialize(-movingDirection);
    }

    private void SpawnBullets()
    {
        if (GameManager.Instance.mode == GameManager.Mode.normal)
        {
            GameObject bullet = BulletObjectPool.Instance.GetFromPool();
            bullet.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
            bullet.GetComponent<PlayerBullet>().SetDirection(movingDirection);
        } else
        {
            for (int i = -1; i <= 1; i++)
            {
                GameObject bullet = BulletObjectPool.Instance.GetFromPool();
                Quaternion targetRotation = Quaternion.Euler(0, 0, i * 15f);
                bullet.transform.SetPositionAndRotation(firePoint.position, targetRotation * firePoint.rotation);
                bullet.GetComponent<PlayerBullet>().SetDirection(movingDirection);
            }
        }
    }

    private void DeathKnockback(int hitDirection)
    {
        //rb.velocity = new Vector2(hitDirection * 20, 10);
        rb.AddForce(new Vector2(hitDirection * knockbackForce.x, knockbackForce.y), ForceMode2D.Impulse);
        //rb.AddForce(new Vector2(hitDirection * knockbackForce.x, knockbackForce.y), ForceMode2D.Impulse);
        rb.transform.rotation = Quaternion.Euler(0, 0, -90 * hitDirection);
        anim.enabled = false;
    }

    private void Knockback()
    {
        //if (!isFiring)
        //{
        //    moveSpeed = standardMoveSpeed;
        //    transform.position = new Vector3(transform.position.x - shootDirection * knockbackDistance, transform.position.y, transform.position.z);
        //} else
        //{
        //    moveSpeed = moveSpeedWhenFiring;
        //}

        //rb.velocity = Vector2.zero;
        //rb.AddForce(new Vector2(-movingDirection * knockbackForce, 0), ForceMode2D.Impulse);
        transform.position = new Vector3(transform.position.x - movingDirection * knockbackDistance, transform.position.y, transform.position.z);
    }

    private void CheckFire()
    {
        if (Input.GetMouseButtonDown(0))    // 0 leftbutton 1 middle 2 rightbutton
        {
            Fire();
        }

        // 自动
        if (Input.GetMouseButton(0))
        {
            //isFiring = true;
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Fire();
            }
        }
        //else {
        //    isFiring = false;
        //}

    }

    private bool canJump() {
        return remainingJump >= 1;
    }

    private void Jump()
    {
        if (!canJump()) return;
        SoundManager.Instance.PlayJumpSound(transform.position);
        remainingJump -= 1;
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        //rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        anim.SetTrigger("GunDelay");
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance, 0));
    //}
}
