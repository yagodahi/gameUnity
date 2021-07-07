using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharRotationController : MonoBehaviour
{
    public GameManager gm;

    [SerializeField]
    private AnimationCurve CurveOnStart = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField]
    private AnimationCurve CurveUp = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField]
    private AnimationCurve CurveDown = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [HideInInspector]
    public GameObject rotatedObj;

    private RotationType rotType;
    private Vector3 axisOfRot;
    private float speed;
    private float tiltAroundZ = -90f;
    private bool rotOnDeltatime;

    private void Start()
    {
        if (gm != null)
        {
            gm.OnStartGame += RotationSwitch;
        }
    }

    private void OnEnable()
    {
        if (gm != null)
        {
            gm.OnStartGame += RotationSwitch;
        }
    }

    private void OnDisable()
    {
        if (gm != null)
        {
            gm.OnStartGame -= RotationSwitch;
        }
    }

    public void RotationSwitch()
    {

        rotType = GV.currentSkin.rotType;
        rotatedObj.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

        switch (rotType)
        {
            case RotationType.none:

                rotOnDeltatime = false;
                break;

            case RotationType.regularRight:

                rotOnDeltatime = true;
                axisOfRot = new Vector3(GV.currentSkin.axisOfRot[0], GV.currentSkin.axisOfRot[1], GV.currentSkin.axisOfRot[2]);
                speed = -GV.currentSkin.rotSpeed;

                break;

            case RotationType.regularLeft:

                rotOnDeltatime = true;
                axisOfRot = new Vector3(GV.currentSkin.axisOfRot[0], GV.currentSkin.axisOfRot[1], GV.currentSkin.axisOfRot[2]);
                speed = GV.currentSkin.rotSpeed;

                break;

            case RotationType.controled:

                rotOnDeltatime = false;
                ControledRotation(CurveOnStart, .5f);

                break;
        }
    }

    //call from ui
    public void Up()
    {
        if (!rotOnDeltatime && GV.inLvl)
        {
            ControledRotation(CurveUp, .3f);
        }
    }

    //call from ui
    public void Down()
    {
        if (!rotOnDeltatime && GV.inLvl)
        {
            ControledRotation(CurveDown, .3f);
        }
    }

    public void ControledRotation(AnimationCurve curwa, float duration)
    {
        
        StartCoroutine(Rot(duration, curwa));
        
    }

    private void FixedUpdate()
    {
        if (rotOnDeltatime)
        {
            rotatedObj.transform.Rotate(axisOfRot, speed);
        }
    }

    IEnumerator Rot(float duration, AnimationCurve curwa)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            rotatedObj.transform.rotation = Quaternion.Euler(0f, 0f, tiltAroundZ * curwa.Evaluate(1f - elapsed / duration));

            yield return null;
        }
    }
}
