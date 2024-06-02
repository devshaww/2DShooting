
using UnityEngine;

public class Shell : MonoBehaviour
{
    private Rigidbody2D rb;
    private float hVelocity = 10f;
    private float vVelocity = 3f;
    private int direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        float hForce = Random.Range(0.5f * hVelocity, hVelocity);
        float vForce = Random.Range(0.5f * vVelocity, vVelocity);
        rb.velocity = new Vector2(direction * hForce, vForce);
    }

    public void Initialize(int dir)
    {
        direction = dir;
    }
}
