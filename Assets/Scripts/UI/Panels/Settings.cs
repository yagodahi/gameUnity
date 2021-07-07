using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup cg;
    [SerializeField]
    private Image vibroMutedImg;
    [SerializeField]
    private Image soundMutedImg;

    // Start is called before the first frame update
    void Start()
    {
        VibroBool();
        SoundBool();

        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void Open()
    {
        cg.DOFade(1f, .3f);
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void Close()
    {
        cg.DOFade(0f, .3f);
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void VibroSettings()
    {
        if (vibroMutedImg.enabled)
        {
            GV.vibro = true;
            Vibration.Vibrate(50L);
        }
        else
        {
            GV.vibro = false;
        }

        VibroBool();
        SaveLoad.Save(new GameDataSave(), "game");
    }

    void VibroBool()
    {
        if (GV.vibro)
        {
            vibroMutedImg.enabled = false;
        }
        else
        {
            vibroMutedImg.enabled = true;
        }
    }

    //call from ui
    public void SoundSettings()
    {
        if (soundMutedImg.enabled)
        {
            GV.sound = true;
        }
        else
        {
            GV.sound = false;
        }

        SoundBool();
        SaveLoad.Save(new GameDataSave(), "game");
    }

    void SoundBool()
    {
        if (GV.sound)
        {
            soundMutedImg.enabled = false;
        }
        else
        {
            soundMutedImg.enabled = true;
        }
    }
}
