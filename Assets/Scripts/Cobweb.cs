using System.Collections;
using UnityEngine;

public class Cobweb : MonoBehaviour, IInteractable
{
    public void OnInteract(bool on)
    {
        GameManager.Instance.TogglePopup(GameManager.PopupSide.LEFT, false);
        GameManager.Instance.cobwebs.Remove(this.gameObject);
        StartCoroutine(GrowShrinkAnimation());
    }

    private IEnumerator GrowShrinkAnimation()
    {
        //increase model scale slightly
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float duration = 0.2f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //decrease scale to 0
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
