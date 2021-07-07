using UnityEngine;
using System.Collections.Generic;
using System;

public enum RotationType
{
    none,
    regularRight,
    regularLeft,
    controled
}

public enum PriceType
{
    onMoney,
    onLevel,
    onAd
}

public enum SkinCond
{
    forSale,
    sold
}

[Serializable]
public class SkinStructSave
{
    public string name;
    public PriceType priceType;
    public int price;
    public int adWatched;
    public SkinCond skinCond;
    public RotationType rotType;
    public float[] axisOfRot;
    public float rotSpeed;
    
}