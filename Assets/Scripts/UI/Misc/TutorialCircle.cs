using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCircle : MonoBehaviour
{
    [SerializeField]
    private Vector3 handStartScale;
    [SerializeField]
    private Vector3 handEndScale;

    private Image circle;

    private void Start()
    {
        circle = GetComponent<Image>();
        Color32 a = new Color32(255, 255, 255, 255);
        Color32 b = new Color32(255, 255, 255, 0);
        StartCoroutine(Krug(1f, handStartScale, handEndScale, a, b));
    }

    IEnumerator Krug(float duration, Vector3 a, Vector3 b, Color32 ac, Color32 bc)
    {
        float elapsed = 0;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(a, b, elapsed / duration);
            circle.color = Color32.Lerp(ac, bc, elapsed / duration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
