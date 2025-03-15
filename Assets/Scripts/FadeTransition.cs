using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FadeTransition : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float fadeDuration = 1f;

    public IEnumerator FadeInOut()
    {
        GameManager.Instance.transitionInProgress = true;

        yield return StartCoroutine(Fade(0, 1));

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(1, 0));

        GameManager.Instance.transitionInProgress = false;
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        Color textColor = text.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            textColor.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color;
            text.color = textColor;
            yield return null;
        }

        color.a = endAlpha;
        textColor.a = endAlpha;
    }
}


