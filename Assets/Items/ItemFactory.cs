public static class ItemFactory
{
    public static Item CreateFromData(ItemData data)
    {
        if (data == null)
        {
            return null;
        }

        Item item;
        if (data is MeleeData)
        {
            item = new Melee();
        }
        else if (data is MagicData)
        {
            item = new Magic();
        }
        else if (data is PotionData)
        {
            item = new Potion();
        }
        else if (data is WeaponData)
        {
            item = new Weapon();
        }
        else if (data is ActiveData)
        {
            item = new Active();
        }
        else
        {
            item = new Item();
        }

        item.ItemData = data;
        return item;
    }

    public static Item Clone(Item source)
    {
        if (source == null)
        {
            return null;
        }

        Item clone = CreateFromData(source.ItemData);
        if (clone == null)
        {
            clone = new Item();
        }

        clone.Count = source.Count;

        if (source is Weapon sourceWeapon && clone is Weapon cloneWeapon)
        {
            cloneWeapon.Level = sourceWeapon.Level;
        }

        return clone;
    }
}