using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarningPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageOnScreen;
    public float duration;
    [SerializeField]
    private bool withMoving;
    [SerializeField]
    private Vector3 stepMoving;

    private CanvasGroup cg;

    public void Reaction(string message)
    {
        cg = gameObject.GetComponent<CanvasGroup>();

        messageOnScreen.text = message;

        StartCoroutine(Life(duration));
    }

    IEnumerator Life(float duration)
    {
        StartCoroutine(Faded(0f, 1f, duration * .25f));

        if (withMoving)
        {
            StartCoroutine(Moving(transform.position, transform.position + stepMoving, duration * .25f));
        }
        
        yield return new WaitForSecondsRealtime(duration * .5f);

        StartCoroutine(Faded(1f, 0f, duration * .25f));

        if (withMoving)
        {
            StartCoroutine(Moving(transform.position, transform.position + stepMoving, duration * .25f));
        }

        yield return new WaitForSecondsRealtime(duration * .25f);

        Destroy(gameObject);
    }

    IEnumerator Faded(float a, float b, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(a, b, elapsed / duration);
            
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    IEnumerator Moving(Vector3 a, Vector3 b, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(a, b, elapsed / duration);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
