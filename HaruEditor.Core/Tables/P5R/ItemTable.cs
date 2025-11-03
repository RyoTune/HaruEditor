using System.ComponentModel;
using HaruEditor.Core.Common;
using HaruEditor.Core.Tables.Common;
using HaruEditor.Core.Tables.P5R.Models;

namespace HaruEditor.Core.Tables.P5R;

public class ItemTable : IReadWrite
{
    public ItemTable() {}

    public ItemTable(string file) : this(File.OpenRead(file), true) {}
    
    public ItemTable(Stream stream, bool ownsStream)
    {
        using var reader = new BigEndianBinaryReader(stream, ownsStream);
        Read(reader);
    }

    public ItemAccessorySegment ItemAccessorySegment { get; set; } = [];
    public ItemArmorSegment ItemArmorSegment { get; set; } = [];
    public ItemConsumableSegment ItemConsumableSegment { get; set; } = [];
    public ItemKeyItemSegment ItemKeyItemSegment { get; set; } = [];
    public ItemMaterialSegment ItemMaterialSegment { get; set; } = [];
    public ItemMeleeWeaponSegment ItemMeleeWeaponSegment { get; set; } = [];
    public ItemOutfitSegment ItemOutfitSegment { get; set; } = [];
    public ItemSkillCardSegment ItemSkillCardSegment { get; set; } = [];
    public ItemRangedWeaponSegment ItemRangedWeaponSegment { get; set; } = [];
    
    [Browsable(false)]
    public UnknownSegment UnknownSegment { get; set; } = [];
    
    public void Read(BinaryReader reader)
    {
        ItemAccessorySegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemArmorSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemConsumableSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemKeyItemSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemMaterialSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemMeleeWeaponSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemOutfitSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemSkillCardSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        ItemRangedWeaponSegment.Read(reader);
        reader.BaseStream.AlignStream();
        
        UnknownSegment.Read(reader);
        reader.BaseStream.AlignStream();
    }

    public void Write(BinaryWriter writer)
    {
        ItemAccessorySegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemArmorSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemConsumableSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemKeyItemSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemMaterialSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemMeleeWeaponSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemOutfitSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemSkillCardSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        ItemRangedWeaponSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        UnknownSegment.Write(writer);
        writer.BaseStream.AlignStream();
        
        writer.BaseStream.SetLength(writer.BaseStream.Position);
    }
}

public class ItemAccessorySegment : BaseSegment<ItemAccessory>
{
    public override uint ItemSize { get; } = 0x40;
}

public class ItemArmorSegment : BaseSegment<ItemArmor>
{
    public override uint ItemSize { get; } = 0x30;
}

public class ItemConsumableSegment : BaseSegment<ItemConsumable>
{
    public override uint ItemSize { get; } = 0x30;
}

public class ItemKeyItemSegment : BaseSegment<ItemKeyItem>
{
    public override uint ItemSize { get; } = 0xC;
}

public class ItemMeleeWeaponSegment : BaseSegment<ItemMeleeWeapon>
{
    public override uint ItemSize { get; } = 0x30;
}

public class ItemOutfitSegment : BaseSegment<ItemOutfit>
{
    public override uint ItemSize { get; } = 0x20;
}

public class ItemSkillCardSegment : BaseSegment<ItemSkillCard>
{
    public override uint ItemSize { get; } = 0x18;
}

public class ItemRangedWeaponSegment : BaseSegment<ItemRangedWeapon>
{
    public override uint ItemSize { get; } = 0x84;
}

public class ItemMaterialSegment : BaseSegment<ItemMaterial>
{
    public override uint ItemSize { get; } = 0x2C;
}

public class ItemMaterial : IReadWrite
{
    public ItemType ItemKind { get; set; }
    public uint MenuSorting { get; set; }
    public ushort Flag { get; set; }
    public ushort Value { get; set; }

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public Month MonthAvailable { get; set; }
    public byte DayAvailable { get; set; }

    public ushort RESERVE_16 { get; set; }

    public uint[] Material { get; set; } = new uint[5];

    public ItemMaterial() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();
        MenuSorting = reader.ReadUInt32();
        Flag = reader.ReadUInt16();
        Value = reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();

        MonthAvailable = (Month)reader.ReadByte();
        DayAvailable = reader.ReadByte();

        RESERVE_16 = reader.ReadUInt16();

        for (int i = 0; i < 5; i++)
            Material[i] = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(MenuSorting);
        writer.Write(Flag);
        writer.Write(Value);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
        writer.Write((byte)MonthAvailable);
        writer.Write(DayAvailable);
        writer.Write(RESERVE_16);
        for (int i = 0; i < 5; i++)
            writer.Write(Material[i]);
    }
}

public class ItemRangedWeapon : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public ushort WeaponFieldMenuSorting { get; set; }
    public ushort WeaponShopMenuSorting { get; set; }

    public EquippableUsers Users { get; set; }
    public RangedWeaponShop IsItAvailable2 { get; set; }

    public ElementalType Attribute { get; set; }
    public byte RESERVE_0F { get; set; }

    public ushort Attack { get; set; }
    public ushort Accuracy { get; set; }
    public ushort Rounds { get; set; }

    public Stats Stats { get; set; } // assume constructor exists
    public byte RESERVE_STAT { get; set; }

    public GearEffect[] Effect { get; set; } = new GearEffect[3];

    public ushort FIELD_22 { get; set; }
    public ushort IwaisUpgradeRank { get; set; }
    public ushort RESERVE_26 { get; set; }

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public Month MonthAvailable { get; set; }
    public byte DayAvailable { get; set; }

    public GunEnhancement LongBarrelEnhancement { get; set; }
    public GunEnhancement GigaBarrelEnhancement { get; set; }
    public GunEnhancement PowerReceiver { get; set; }
    public GunEnhancement HighPowerReceiver { get; set; }
    public GunEnhancement MegaPowerReceiver { get; set; }
    public GunEnhancement FireCamo { get; set; }
    public GunEnhancement ElectricCamo { get; set; }
    public GunEnhancement IceCamo { get; set; }

    public ushort RESERVE_82 { get; set; }

    public ItemRangedWeapon() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();

        WeaponFieldMenuSorting = reader.ReadUInt16();
        WeaponShopMenuSorting = reader.ReadUInt16();

        Users = (EquippableUsers)reader.ReadUInt32();
        IsItAvailable2 = (RangedWeaponShop)reader.ReadUInt16();

        Attribute = (ElementalType)reader.ReadByte();
        RESERVE_0F = reader.ReadByte();

        Attack = reader.ReadUInt16();
        Accuracy = reader.ReadUInt16();
        Rounds = reader.ReadUInt16();

        Stats = new(reader);
        RESERVE_STAT = reader.ReadByte();

        for (int i = 0; i < 3; i++)
            Effect[i] = (GearEffect)reader.ReadUInt16();

        FIELD_22 = reader.ReadUInt16();
        IwaisUpgradeRank = reader.ReadUInt16();
        RESERVE_26 = reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();

        MonthAvailable = (Month)reader.ReadByte();
        DayAvailable = reader.ReadByte();

        LongBarrelEnhancement = new(reader);
        GigaBarrelEnhancement = new(reader);
        PowerReceiver = new(reader);
        HighPowerReceiver = new(reader);
        MegaPowerReceiver = new(reader);
        FireCamo = new(reader);
        ElectricCamo = new(reader);
        IceCamo = new(reader);

        RESERVE_82 = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(WeaponFieldMenuSorting);
        writer.Write(WeaponShopMenuSorting);
        writer.Write((uint)Users);
        writer.Write((ushort)IsItAvailable2);
        writer.Write((byte)Attribute);
        writer.Write(RESERVE_0F);
        writer.Write(Attack);
        writer.Write(Accuracy);
        writer.Write(Rounds);
        Stats.Write(writer);
        writer.Write(RESERVE_STAT);
        for (int i = 0; i < 3; i++)
            writer.Write((ushort)Effect[i]);
        writer.Write(FIELD_22);
        writer.Write(IwaisUpgradeRank);
        writer.Write(RESERVE_26);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
        writer.Write((byte)MonthAvailable);
        writer.Write(DayAvailable);

        LongBarrelEnhancement.Write(writer);
        GigaBarrelEnhancement.Write(writer);
        PowerReceiver.Write(writer);
        HighPowerReceiver.Write(writer);
        MegaPowerReceiver.Write(writer);
        FireCamo.Write(writer);
        ElectricCamo.Write(writer);
        IceCamo.Write(writer);

        writer.Write(RESERVE_82);
    }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class GunEnhancement
{
    public RangedItemUpgradable RangedItemUpgradable2 { get; set; }
    public ushort Attack1 { get; set; }
    public ushort Accuracy1 { get; set; }
    public ushort Rounds1 { get; set; }
    public GearEffect Effect { get; set; }

    public GunEnhancement() { }

    public GunEnhancement(BinaryReader reader)
    {
        RangedItemUpgradable2 = (RangedItemUpgradable)reader.ReadUInt16();
        Attack1 = reader.ReadUInt16();
        Accuracy1 = reader.ReadUInt16();
        Rounds1 = reader.ReadUInt16();
        Effect = (GearEffect)reader.ReadUInt16();
    }
    
    public void Write(BinaryWriter writer)
    {
        writer.Write((ushort)RangedItemUpgradable2);
        writer.Write(Attack1);
        writer.Write(Accuracy1);
        writer.Write(Rounds1);
        writer.Write((ushort)Effect);
    }
}

public class ItemSkillCard : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public uint MenuSorting { get; set; }
    public ushort Flag { get; set; }        // hex format

    public BattleSkill Skill { get; set; }  // assumed ushort enum

    public ushort Level { get; set; }
    public ushort Value { get; set; }

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public ItemSkillCard() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();
        MenuSorting = reader.ReadUInt32();
        Flag = reader.ReadUInt16();

        Skill = (BattleSkill)reader.ReadUInt16();

        Level = reader.ReadUInt16();
        Value = reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(MenuSorting);
        writer.Write(Flag);
        writer.Write((ushort)Skill);
        writer.Write(Level);
        writer.Write(Value);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
    }
}

public class ItemOutfit : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public ushort FieldMenuSorting { get; set; }
    public ushort ShopMenuSorting { get; set; }

    public EquippableUsers Users { get; set; }

    public GearEffect[] Effect { get; set; } = new GearEffect[3]; // assume GearEffect constructor exists

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public Month MonthAvailable { get; set; } // assume ushort enum
    public byte DayAvailable { get; set; }

    public ushort RESERVE_1 { get; set; }
    public ushort RESERVE_2 { get; set; }

    public ItemOutfit() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();

        FieldMenuSorting = reader.ReadUInt16();
        ShopMenuSorting = reader.ReadUInt16();

        Users = (EquippableUsers)reader.ReadUInt32();

        for (int i = 0; i < 3; i++)
            Effect[i] = (GearEffect)reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();

        MonthAvailable = (Month)reader.ReadByte();
        DayAvailable = reader.ReadByte();

        RESERVE_1 = reader.ReadUInt16();
        RESERVE_2 = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(FieldMenuSorting);
        writer.Write(ShopMenuSorting);
        writer.Write((uint)Users);
        for (int i = 0; i < 3; i++)
            writer.Write((ushort)Effect[i]);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
        writer.Write((byte)MonthAvailable);
        writer.Write(DayAvailable);
        writer.Write(RESERVE_1);
        writer.Write(RESERVE_2);
    }

}

public class ItemMeleeWeapon : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public ushort WeaponFieldMenuSorting { get; set; }  // Higher = top
    public ushort WeaponShopMenuSorting { get; set; }   // Lower = top

    public EquippableUsers Users { get; set; }
    public MeleeWeaponShop IsItAvailable { get; set; } // assumed ushort enum

    public ushort RESERVE { get; set; }

    public ushort Attack { get; set; }
    public ushort Accuracy { get; set; }

    public Stats Stats { get; set; }  // assume constructor exists
    public byte RESERVE_STAT { get; set; }

    public GearEffect[] Effect { get; set; } = new GearEffect[3]; // assume GearEffect constructor exists

    public ushort Level { get; set; }
    public ushort Value { get; set; }

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public Month MonthAvailable { get; set; } // assumed ushort enum
    public byte DayAvailable { get; set; }

    public ushort UNKNOWN_2E { get; set; }

    public ItemMeleeWeapon() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();

        WeaponFieldMenuSorting = reader.ReadUInt16();
        WeaponShopMenuSorting = reader.ReadUInt16();

        Users = (EquippableUsers)reader.ReadUInt32();
        IsItAvailable = (MeleeWeaponShop)reader.ReadUInt16();

        RESERVE = reader.ReadUInt16();

        Attack = reader.ReadUInt16();
        Accuracy = reader.ReadUInt16();

        Stats = new(reader); // assume constructor exists
        RESERVE_STAT = reader.ReadByte();

        for (int i = 0; i < 3; i++)
            Effect[i] = (GearEffect)reader.ReadUInt16(); // assume constructor exists

        Level = reader.ReadUInt16();
        Value = reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();

        MonthAvailable = (Month)reader.ReadByte();
        DayAvailable = reader.ReadByte();

        UNKNOWN_2E = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(WeaponFieldMenuSorting);
        writer.Write(WeaponShopMenuSorting);
        writer.Write((uint)Users);
        writer.Write((ushort)IsItAvailable);
        writer.Write(RESERVE);
        writer.Write(Attack);
        writer.Write(Accuracy);
        Stats.Write(writer);
        writer.Write(RESERVE_STAT);
        for (int i = 0; i < 3; i++)
            writer.Write((ushort)Effect[i]);
        writer.Write(Level);
        writer.Write(Value);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
        writer.Write((byte)MonthAvailable);
        writer.Write(DayAvailable);
        writer.Write(UNKNOWN_2E);
    }

}

public class ItemKeyItem : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public uint MenuSorting { get; set; }
    public ushort Flag { get; set; } // hex format
    public ushort RESERVE_1A { get; set; } // reserved

    public ItemKeyItem()
    {
    }

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();
        MenuSorting = reader.ReadUInt32();
        Flag = reader.ReadUInt16();
        RESERVE_1A = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(MenuSorting);
        writer.Write(Flag);
        writer.Write(RESERVE_1A);
    }
}

public class ItemConsumable : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public uint MenuSorting { get; set; }
    public ushort Flag { get; set; }  // hex format, kept as ushort

    public Usability Availability { get; set; } // assumed ushort enum
    public BattleSkill Skill { get; set; }      // assumed ushort enum

    public ushort RESERVE_0E { get; set; }

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public Month MonthAvailable { get; set; }   // assumed ushort enum
    public byte DayAvailable { get; set; }
    public ushort RESERVE_1A { get; set; }

    public uint[] Material { get; set; } = new uint[5];

    public ItemConsumable() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();
        MenuSorting = reader.ReadUInt32();
        Flag = reader.ReadUInt16();

        Availability = (Usability)reader.ReadUInt16();
        Skill = (BattleSkill)reader.ReadUInt16();

        RESERVE_0E = reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();

        MonthAvailable = (Month)reader.ReadByte();
        DayAvailable = reader.ReadByte();
        RESERVE_1A = reader.ReadUInt16();

        for (int i = 0; i < 5; i++)
            Material[i] = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(MenuSorting);
        writer.Write(Flag);
        writer.Write((ushort)Availability);
        writer.Write((ushort)Skill);
        writer.Write(RESERVE_0E);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
        writer.Write((byte)MonthAvailable);
        writer.Write(DayAvailable);
        writer.Write(RESERVE_1A);
        for (int i = 0; i < 5; i++)
            writer.Write(Material[i]);
    }
}

public class ItemArmor : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public ushort WeaponFieldMenuSorting { get; set; }  // Higher value = top
    public ushort WeaponShopMenuSorting { get; set; }   // Lower value = top

    public EquippableUsers Users { get; set; }
    public ArmorWeaponShop IsItAvailable3 { get; set; } // Assume ushort enum

    public ushort ArmorDefense { get; set; }
    public ushort ArmorEvasion { get; set; }

    public Stats Stats { get; set; }  // Assume constructor exists
    public byte RESERVE_STAT { get; set; }

    public GearEffect[] Effect { get; set; } = new GearEffect[3]; // Assume GearEffect constructor exists

    public ushort Level { get; set; }
    public ushort Value { get; set; }
    public ushort RESERVE_1E { get; set; }

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public Month MonthAvailable { get; set; } // Assume ushort enum
    public byte DayAvailable { get; set; }
    public ushort Unknown { get; set; }

    public ItemArmor() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();

        WeaponFieldMenuSorting = reader.ReadUInt16();
        WeaponShopMenuSorting = reader.ReadUInt16();

        Users = (EquippableUsers)reader.ReadUInt32();
        IsItAvailable3 = (ArmorWeaponShop)reader.ReadUInt16();

        ArmorDefense = reader.ReadUInt16();
        ArmorEvasion = reader.ReadUInt16();

        Stats = new(reader); // Assume constructor exists
        RESERVE_STAT = reader.ReadByte();

        for (int i = 0; i < 3; i++)
            Effect[i] = (GearEffect)reader.ReadUInt16(); // Assume constructor exists

        Level = reader.ReadUInt16();
        Value = reader.ReadUInt16();
        RESERVE_1E = reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();

        MonthAvailable = (Month)reader.ReadByte();
        DayAvailable = reader.ReadByte();
        Unknown = reader.ReadUInt16();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(WeaponFieldMenuSorting);
        writer.Write(WeaponShopMenuSorting);
        writer.Write((uint)Users);
        writer.Write((ushort)IsItAvailable3);
        writer.Write(ArmorDefense);
        writer.Write(ArmorEvasion);
        Stats.Write(writer);
        writer.Write(RESERVE_STAT);
        for (int i = 0; i < 3; i++)
            writer.Write((ushort)Effect[i]);
        writer.Write(Level);
        writer.Write(Value);
        writer.Write(RESERVE_1E);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
        writer.Write((byte)MonthAvailable);
        writer.Write(DayAvailable);
        writer.Write(Unknown);
    }
}

public class ItemAccessory : IReadWrite
{
    public ItemType ItemKind { get; set; }

    public ushort WeaponFieldMenuSorting { get; set; } // Higher Value = top
    public ushort WeaponShopMenuSorting { get; set; }  // Lower Value = top

    public EquippableUsers Users { get; set; }
    public AccessoriesShop IsItAvailable4 { get; set; }

    public Stats Stats { get; set; }
    public byte RESERVE_STAT { get; set; }

    public GearEffect[] Effect { get; set; } = new GearEffect[3];

    public ushort Level { get; set; }
    public ushort Value { get; set; }
    public ushort RESERVE_1E { get; set; }

    public uint PurchasePrice { get; set; }
    public uint SellPrice { get; set; }

    public Month MonthAvailable { get; set; }
    public byte DayAvailable { get; set; }
    public ushort RESERVE_2A { get; set; }

    public uint[] Material { get; set; } = new uint[5];

    public ItemAccessory() {}

    public void Read(BinaryReader reader)
    {
        ItemKind = (ItemType)reader.ReadUInt32();

        WeaponFieldMenuSorting = reader.ReadUInt16();
        WeaponShopMenuSorting = reader.ReadUInt16();

        // Assume these are byte/enum types elsewhere
        Users = (EquippableUsers)reader.ReadUInt32();
        IsItAvailable4 = (AccessoriesShop)reader.ReadUInt16();

        Stats = new(reader); // Assuming constructor exists
        RESERVE_STAT = reader.ReadByte();

        for (int i = 0; i < 3; i++)
            Effect[i] = (GearEffect)reader.ReadUInt16(); // Assuming constructor exists

        Level = reader.ReadUInt16();
        Value = reader.ReadUInt16();
        RESERVE_1E = reader.ReadUInt16();

        PurchasePrice = reader.ReadUInt32();
        SellPrice = reader.ReadUInt32();

        MonthAvailable = (Month)reader.ReadByte();
        DayAvailable = reader.ReadByte();
        RESERVE_2A = reader.ReadUInt16();

        for (int i = 0; i < 5; i++)
            Material[i] = reader.ReadUInt32();
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write((uint)ItemKind);
        writer.Write(WeaponFieldMenuSorting);
        writer.Write(WeaponShopMenuSorting);
        writer.Write((uint)Users);
        writer.Write((ushort)IsItAvailable4);
        Stats.Write(writer);
        writer.Write(RESERVE_STAT);
        for (int i = 0; i < 3; i++)
            writer.Write((ushort)Effect[i]);
        writer.Write(Level);
        writer.Write(Value);
        writer.Write(RESERVE_1E);
        writer.Write(PurchasePrice);
        writer.Write(SellPrice);
        writer.Write((byte)MonthAvailable);
        writer.Write(DayAvailable);
        writer.Write(RESERVE_2A);
        for (int i = 0; i < 5; i++)
            writer.Write(Material[i]);
    }
}


[Flags]
public enum EquippableUsers
{
    Joker   = 1 << 0,  // originally low bit
    Ryuji   = 1 << 1,
    Morgana = 1 << 2,
    Ann     = 1 << 3,
    Yusuke  = 1 << 4,
    Makoto  = 1 << 5,
    Haru    = 1 << 6,
    Futaba  = 1 << 7,
    Goro    = 1 << 8,
    Kasumi  = 1 << 9   // now the high bit
}

[Flags]
public enum ItemType : uint
{
    Dagger = 1U << 0,
    Crowbar = 1U << 1,
    Whip = 1U << 2,
    BanditSword = 1U << 3,
    Katana = 1U << 4,
    FistWeapons = 1U << 5,
    Axe = 1U << 6,
    BeamSword = 1U << 7,
    Rapier = 1U << 8,
    Unknown4 = 1U << 9,
    Handgun = 1U << 10,
    Shotgun = 1U << 11,
    SMG = 1U << 12,
    Slingshot = 1U << 13,
    AR = 1U << 14,
    Revolver = 1U << 15,
    GL = 1U << 16,
    ToyGun = 1U << 17,
    LAR = 1U << 18,
    Unknown3 = 1U << 19,
    Armor = 1U << 20,
    Accessory = 1U << 21,
    Consumable = 1U << 22,
    KeyItem = 1U << 23,
    Treasure = 1U << 24,
    SkillCard = 1U << 25,
    Outfit = 1U << 26,
    DungeonItem = 1U << 27,
    CraftingMaterial = 1U << 28,
    Gift = 1U << 29,
    Unknown2 = 1U << 30,
    Unknown1 = 1U << 31
}
