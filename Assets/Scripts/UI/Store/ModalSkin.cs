using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ModalSkin : MonoBehaviour
{
    public Slot slot;
    public RawImage itemIcon;
    public PriceType priceType;

    [SerializeField]
    private Button buy;
    [SerializeField]
    private TextMeshProUGUI buttonText;

    private Button close;

    private void Start()
    {
        close = GetComponent<Button>();
        close.onClick.AddListener(Destroing);

        switch (priceType)
        {
            case PriceType.onMoney:
                buy.onClick.AddListener(slot.SkinBuyingForMoney);
                buttonText.text = "BUY AND EQUIP";
                break;
            case PriceType.onLevel:
                //eto okno ne ispolzuet eto sostojanie
                break;
            case PriceType.onAd:
                buy.onClick.AddListener(slot.ShowAd);
                buttonText.text = "WATCH AD";
                break;
        }
    }

    public void Destroing()
    {
        Destroy(gameObject);
    }
}
