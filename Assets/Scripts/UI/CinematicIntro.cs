using System.Collections;
using UnityEngine;

public class CinematicIntro : MonoBehaviour
{
    public RectTransform topBlackBar;
    public RectTransform bottomBlackBar;
    public RectTransform introText;
    public float barMoveDuration = .5f; // Duration for the bars to move into place
    public float textSlideDuration = .5f; // Duration to display the text

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        // Start with bars off-screen
        topBlackBar.anchoredPosition = new Vector2(0, topBlackBar.rect.height);
        bottomBlackBar.anchoredPosition = new Vector2(0, -bottomBlackBar.rect.height);

        // Move bars into place
        yield return StartCoroutine(MoveBarsIntoPlace());

        // slide in text
        yield return StartCoroutine(TextSlideInCoroutine());

        yield return new WaitForSecondsRealtime(2f);

        yield return StartCoroutine(MoveBarsOutCoroutine());

        //// slide out text
        //yield return StartCoroutine(TextSlideOutCoroutine());
    }

    IEnumerator MoveBarsIntoPlace()
    {
        float elapsedTime = 0;
        Vector2 topStartPos = topBlackBar.anchoredPosition;
        Vector2 bottomStartPos = bottomBlackBar.anchoredPosition;
        Vector2 topEndPos = Vector2.zero;
        Vector2 bottomEndPos = Vector2.zero;

        while (elapsedTime < barMoveDuration)
        {
            topBlackBar.anchoredPosition = Vector2.Lerp(topStartPos, topEndPos, elapsedTime / barMoveDuration);
            bottomBlackBar.anchoredPosition = Vector2.Lerp(bottomStartPos, bottomEndPos, elapsedTime / barMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        topBlackBar.anchoredPosition = topEndPos;
        bottomBlackBar.anchoredPosition = bottomEndPos;
    }

    IEnumerator MoveBarsOutCoroutine() {
        float elapsedTime = 0;
        Vector2 topStartPos = topBlackBar.anchoredPosition;
        Vector2 bottomStartPos = bottomBlackBar.anchoredPosition;
        Vector2 topEndPos = new(0, topBlackBar.rect.height);
        Vector2 bottomEndPos = new(0, -bottomBlackBar.rect.height);

        while (elapsedTime < barMoveDuration)
        {
            topBlackBar.anchoredPosition = Vector2.Lerp(topStartPos, topEndPos, elapsedTime / barMoveDuration);
            bottomBlackBar.anchoredPosition = Vector2.Lerp(bottomStartPos, bottomEndPos, elapsedTime / barMoveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        topBlackBar.anchoredPosition = topEndPos;
        bottomBlackBar.anchoredPosition = bottomEndPos;
    }

    IEnumerator TextSlideInCoroutine() {
        float elapsedTime = 0;
        Vector2 startPos = introText.anchoredPosition;
        Vector2 endPos = Vector2.zero; // Center of the screen

        while (elapsedTime < textSlideDuration)
        {
            introText.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / textSlideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        introText.anchoredPosition = endPos;
    }

    //IEnumerator TextSlideOutCoroutine() {
    //    float elapsedTime = 0;
    //    Vector2 startPos = introText.anchoredPosition;
    //    Vector2 endPos = new(-introText.rect.width, 0); // Center of the screen

    //    while (elapsedTime < textSlideDuration)
    //    {
    //        introText.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / textSlideDuration);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    introText.anchoredPosition = endPos;
    //}


    //IEnumerator FadeTextToZeroAlpha(float duration)
    //{
    //    Color originalColor = text.color;
    //    float elapsedTime = 0;

    //    while (elapsedTime < duration)
    //    {
    //        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1, 0, elapsedTime / duration));
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    //}
}
