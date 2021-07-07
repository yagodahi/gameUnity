using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    private GameObject Char;
    [SerializeField]
    private GameObject cam;
    [Header("-----")]
    [SerializeField]
    private Vector3 startCamPos;
    [Header("-----")]
    [SerializeField]
    private Vector3 standartCamPos;
    [SerializeField]
    private Vector3 standartCamRot;
    [Header("-----")]
    [SerializeField]
    private Vector3 onBonusCamPos;
    [SerializeField]
    private Vector3 onBonusCamRot;
    [Header("-----")]
    [SerializeField]
    private Vector3 onLevelUpCamPos;

    private Vector3 curCamPos;
    private Vector3 newCamPos;
    private Vector3 curCamRot;
    private Vector3 newCamRot;
    private Coroutine bonusDelay;
    private Coroutine bonusIn;
    private Coroutine bonusOut;

    private void Start()
    {
        GM.OnStartGame += OnGameStart;
        GM.OnGameOver += OnGameOver;
        GM.OnBonus += OnBonus;
        GM.OnLevelUp += OnLevelUp;
        GM.OnLevelUpEnd += OnLevelUpEnd;
        GM.OnSecondLife += OnGameStart;

        newCamPos = startCamPos;
        newCamRot = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Char.transform.position.x, 0, 0);
    }

    void OnGameStart()
    {
        curCamPos = cam.transform.localPosition;
        curCamRot = cam.transform.localRotation.eulerAngles;
        newCamPos = standartCamPos;
        newCamRot = standartCamRot;

        StartCoroutine(AnimTimer(1.5f));
    }

    void OnGameOver()
    {
        Stop(bonusIn);
        Stop(bonusOut);
        Stop(bonusDelay);

        cam.GetComponent<CameraShake>().Shake();
        curCamPos = cam.transform.localPosition;
        curCamRot = cam.transform.localRotation.eulerAngles;
        newCamPos = startCamPos;
        newCamRot = Vector3.zero;

        StartCoroutine(AnimTimer(1f));
    }

    void OnBonus()
    {
        float duration = GV.bonusDuration;

        curCamPos = cam.transform.localPosition;
        curCamRot = cam.transform.localRotation.eulerAngles;
        newCamPos = onBonusCamPos;
        newCamRot = onBonusCamRot;

        bonusDelay = StartCoroutine(OnBonusDelay(duration));
    }

    void OnLevelUp()
    {
        Stop(bonusIn);
        Stop(bonusOut);
        Stop(bonusDelay);

        curCamPos = cam.transform.localPosition;
        curCamRot = cam.transform.localRotation.eulerAngles;
        newCamPos = onLevelUpCamPos;
        newCamRot = standartCamRot;
        
        StartCoroutine(AnimTimer(1f));
    }

    void OnLevelUpEnd()
    {
        curCamPos = cam.transform.localPosition;
        newCamPos = standartCamPos;
        
        StartCoroutine(AnimTimer(1f));
    }

    //void OnSecondLife()
    //{
    //    alive = true;
    //}

    void Stop(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator OnBonusDelay(float time)
    {
        bonusIn = StartCoroutine(AnimTimer(time * .2f));

        yield return new WaitForSeconds(time * .8f);

        if (GV.inLvl)
        {
            curCamPos = cam.transform.localPosition;
            curCamRot = cam.transform.localRotation.eulerAngles;
            newCamPos = standartCamPos;
            newCamRot = standartCamRot;

            bonusOut = StartCoroutine(AnimTimer(time * .2f));

            yield return new WaitForSeconds(time * .2f);

            bonusDelay = null;
        }
    }

    IEnumerator AnimTimer(float duration)
    {
        float elapsed = 0f;

        while(elapsed < duration)
        {
            cam.transform.localPosition = Vector3.Lerp(curCamPos, newCamPos, elapsed / duration);
            cam.transform.localRotation = Quaternion.Euler(Vector3.Lerp(curCamRot, newCamRot, elapsed / duration));

            elapsed += Time.deltaTime;

            yield return null;
        }

        bonusIn = null;
        bonusOut = null;
        curCamPos = newCamPos;
        curCamRot = newCamRot;
    }
}
