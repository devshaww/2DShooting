
using UnityEngine;

public class Splatter : MonoBehaviour
{
    public Sprite[] sprites;
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        int spriteIndex = Random.Range(0, sprites.Length);
        sr.sprite = sprites[spriteIndex];
        if (spriteIndex != sprites.Length - 1)
        {
            transform.localScale = new Vector3(Random.Range(3, 5), Random.Range(3, 5), 1);
        }
    }
}
