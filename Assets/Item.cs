using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Inventory Item")]
public class Item : ScriptableObject
{
    public new string name = "New Item";
    public string description = "Item description";
    public int sellWorth = 0;
    public Sprite icon;
    public bool isPlayerEquipable = true;

    // Add any other properties or methods specific to the item here
}