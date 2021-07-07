using System.Collections;
using UnityEngine;

public class TriggerOnBonus : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;

    private BoxCollider box;
    private Coroutine bonusDelay;

    private void Start()
    {
        GM.OnBonus += OnBonus;

        box = GetComponent<BoxCollider>();
        box.enabled = false;
    }

    public void OnBonus()
    {
        float duration = GV.bonusDuration;

        bonusDelay = null;
        bonusDelay = StartCoroutine(BonusDelay(duration));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Obs")
        {
            other.gameObject.GetComponent<ObstacleCrystal>().OnBonusTrigger();
        }
    }

    IEnumerator BonusDelay(float time)
    {
        box.enabled = true;

        yield return new WaitForSeconds(time);

        box.enabled = false;
    }
}
