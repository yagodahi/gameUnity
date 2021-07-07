using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Store : MonoBehaviour
{
    [SerializeField]
    private GameManager GM;
    [SerializeField]
    private Charc character;
    [SerializeField]
    private BgGenerator bgGen;
    [SerializeField]
    private LevelGenerator lvlGen;

    [Space]

    [SerializeField]
    private TextMeshProUGUI moneyText;

    [Space]
    
    [SerializeField]
    private GameObject contentField;
    [SerializeField]
    private GameObject slot;
    [SerializeField]
    private Sprite[] imagesOfPrice;

    private List<SkinStructSave> skinsData;
    private Vector3 iconSetupOffset;
    private float iconSetupOffsetConst = -5f;
    private float iconSetupOffsetValue;

    private void Awake()
    {
        if (SaveLoad.SaveExists("skins"))
        {
            skinsData = SaveLoad.Load<List<SkinStructSave>>("skins");
        }

        foreach (SkinStructSave item in skinsData)
        {
            iconSetupOffset = new Vector3(iconSetupOffsetValue, 0f, 0f);

            GameObject slotInst = Instantiate(slot, contentField.transform);
            Slot slotCS = slotInst.GetComponent<Slot>();

            foreach (GameObject skinPref in GV.skinsPrefabs)
            {
                if (skinPref.name == item.name)
                {
                    //slotCS.mesh = skinPref;
                    slotCS.skinPref = skinPref;
                    break;
                }
            }

            
            slotCS.store = this;
            slotCS.GM = GM;
            slotCS.character = character;
            slotCS.skinData = item;
            slotCS.iconSetupOffset = iconSetupOffset;

            iconSetupOffsetValue += iconSetupOffsetConst;

            switch (item.priceType)
            {
                case PriceType.onMoney:
                    slotCS.spritePrice = imagesOfPrice[0];
                    break;
                case PriceType.onLevel:
                    slotCS.iconPrice.gameObject.SetActive(false);
                    slotCS.SkinBuingForLevel();
                    break;
                case PriceType.onAd:
                    slotCS.spritePrice = imagesOfPrice[2];
                    break;
            }
        }
    }

    //update all slots
    public void SlotChanges()
    {
        Slot[] slots = transform.GetComponentsInChildren<Slot>();

        foreach (Slot item in slots)
        {
            item.SkinCondCheck();
        }
    }

    void Start()
    {
        GM.OnCoinPickup += MoneyOnScreen;

        MoneyOnScreen();
    }

    private void OnEnable()
    {
        MoneyOnScreen();
    }

    private void OnDisable()
    {
        SaveLoad.Save(GV.skinsData, "skins");
    }

    void MoneyOnScreen()
    {
        moneyText.text = GV.coins.ToString();
    }
}
