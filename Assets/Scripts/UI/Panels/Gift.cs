using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Gift : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;
    [Space]

    [SerializeField]
    private Image giftImg;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private GameObject textReaction;
    [SerializeField]
    private UIParticleSystem coinParticles;

    private Coroutine giftAnim;
    private Sequence mainSeq;

    // Start is called before the first frame update
    void Start()
    {
        levelText.text = "Level " + (GV.level - 1).ToString() + " is passed!";

        mainSeq = DOTween.Sequence();
        mainSeq.SetLoops(-1);

        GiftAnim();

        textReaction.GetComponent<WarningPanel>().duration = 2f;
    }

    public void Confirm()
    {
        GM.CoinPickup(50);
        mainSeq.Kill();

        Sequence seq = DOTween.Sequence();
        seq.Append(giftImg.transform.DOScale(new Vector3(1f, 1f, 0f), .3f))
            .Append(giftImg.GetComponent<RectTransform>().DOShakePosition(1.5f, 50f, 100, 90f, false, true));

        this.Delay(PlayCoin, .3f);
        this.Delay(() => TextReaction("+50"), .3f);
        this.Delay(Close, 2.5f);
    }

    void Close()
    {
        gameObject.SetActive(false);
    }

    void TextReaction(string message)
    {
        GameObject rcInst = Instantiate(textReaction, coinParticles.transform);

        rcInst.GetComponent<WarningPanel>().Reaction(message);
    }

    void PlayCoin()
    {
        coinParticles.Play();
    }

    void GiftAnim()
    {

        mainSeq.Append(giftImg.transform.DOScale(new Vector3(1.2f, .8f, 0f), .6f))
            .Append(giftImg.transform.DOScale(new Vector3(.8f, 1.2f, 0f), .6f));
    }
}
