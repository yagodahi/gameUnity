using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private Image krug;
    [SerializeField]
    private Image hand;
    [SerializeField]
    private GameObject point;
    [SerializeField]
    private GameObject reaction;

    [Space]

    [SerializeField]
    private Vector3 handStartScale;
    [SerializeField]
    private Vector3 handEndScale;
    [SerializeField]
    private Vector3 krugStartPos;
    [SerializeField]
    private Vector3 krugEndPos;

    private Coroutine reactCirc;

    private void Start()
    {
        Firebase.Analytics.FirebaseAnalytics.LogEvent("tutorial_begin");
        StartCoroutine(Timer(3f));
    }

    IEnumerator Timer(float timer)
    {
        yield return new WaitForSeconds(timer * .1f);

        StartCoroutine(Hand(timer * .1f, handStartScale, handEndScale));

        yield return new WaitForSeconds(timer * .1f);

        StartCoroutine(Krug(timer * .12f, krugStartPos, krugEndPos));
        reactCirc = StartCoroutine(Reaction(timer * .1f));

        yield return new WaitForSeconds(timer * .5f);

        StopCoroutine(reactCirc);

        yield return new WaitForSeconds(timer * .1f);

        StartCoroutine(Krug(timer * .12f, krugEndPos, krugStartPos));
        StartCoroutine(Hand(timer * .1f, handEndScale, handStartScale));

        yield return new WaitForSeconds(timer * .6f);

        StartCoroutine(Timer(3f));
    }

    IEnumerator Krug(float duration, Vector3 a, Vector3 b)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            krug.transform.localPosition = Vector3.Lerp(a, b, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Hand(float duration, Vector3 a, Vector3 b)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            hand.transform.localScale = Vector3.Lerp(a, b, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Reaction(float timer)
    {
        Instantiate(reaction, point.transform);
        yield return new WaitForSeconds(timer);

        reactCirc = null;
        reactCirc = StartCoroutine(Reaction(timer));
    }
}
