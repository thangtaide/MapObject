using System;
using System.Collections.Generic;

[Serializable]
public class ItemDefineData
{
    public List<ItemDefine> Items;
}

[Serializable]
public class ItemDefine
{
    public int DefineId;
    public int ItemRarity;
    public int ItemType;
    public int IconId;
    public bool CanSell;
    public int SellPrice;
    public bool HaveStack;
    public int MaxStack;
    public string NameItemProcessor;
    public int ItemCategoryType;
    public int MapObjectId;
}