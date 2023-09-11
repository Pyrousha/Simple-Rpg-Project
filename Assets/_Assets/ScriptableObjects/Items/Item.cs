using UnityEngine;

public class Item : ScriptableObject
{
    [field: SerializeField] public Sprite Icon { get; private set; }
}

[CreateAssetMenu(menuName = "Items/KeyItem")]
public class KeyItem : Item
{

}

[CreateAssetMenu(menuName = "Items/Consumable")]
public class Consumable : Item
{

}

[CreateAssetMenu(menuName = "Items/Equipment")]
public class Equipment : Item
{

}
