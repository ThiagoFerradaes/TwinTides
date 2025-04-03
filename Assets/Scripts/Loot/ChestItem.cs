using UnityEngine;

public class ChestItem: ScriptableObject
{
    [SerializeField] string itemName;
    [SerializeField] Sprite sprite;

    public string ReturnName() {
        return itemName;
    }

    public Sprite ReturnSprite() {
        return sprite;
    }
}
