using System;
using System.Collections.Generic;


[Serializable]
public class EquipmentDefineData
{
    public List<EquipmentDefine> Equipments;
}

[Serializable]
public class EquipmentDefine
{
    public int DefineId;
    public bool IsGolden;
    public int EquipmentType;
    public int WeaponType;
    public int Level;
    public bool CanSell;
    public int SellPrice;
    public int RepairPrice;
    public int OutfitId;
    public int IconId;
    public bool HasGenderRequirement;
    public int GenderRequirement;
    public bool HasElementRequirement;
    public int ElementRequirement;
    public bool HasSectRequirement;
    public int SectRequirement;
    public bool HasDurability;
    public int Durability;
    public int SetId;
    public List<RoleAttributeOptionDefine> BasicAttributes;
    public List<RoleAttributeOptionDefine> RequiredAttributes;
    public List<RoleAttributeOptionDefine> GoldAttributes;
    public int ElementDefault;
    public int MapObjectId;
}

[Serializable]
public class RoleAttributeOptionDefine
{
    public int Type;
    public int Value;
}