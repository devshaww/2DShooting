
using System.Collections;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float duration;
    [SerializeField] private SpriteRenderer sr;

    private Material originalMaterial;
    private Coroutine flashCoroutine;

    void Start()
    {
        originalMaterial = sr.material;
    }

    private IEnumerator FlashCoroutine()
    {
        sr.material = flashMaterial;
        yield return new WaitForSeconds(duration);
        sr.material = originalMaterial;
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashCoroutine());
    }
}
