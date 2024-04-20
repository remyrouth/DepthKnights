using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour
{

    public GameObject BatEquipObject;

    public Item BatItem;
    public GameObject EdenEquipObject;
    public Item EdenItem;

    public List<GameObject> InventorySlots = new List<GameObject>(); // List for inventory slots
    //Item
    public List<Item> InventoryItem = new List<Item>(); // List for inventory slots
    public Image GatBatIcon;
    public Image EdenIcon;

    // PlayerGFX pgfx;
    public SpriteRenderer pgfx;
    public SpriteRenderer bgfx;


    void UpdateInventorySprites()
    {
        // Loop through each inventory slot
        for (int i = 0; i < InventorySlots.Count; i++)
        {

            // Check if there's a corresponding item for this slot
            if (i < InventoryItem.Count && InventoryItem[i] != null && InventoryItem[i].icon != null)
            {
                ItemAvailable(InventorySlots[i], InventoryItem[i]);
            } else {
                ResetItemFrame(InventorySlots[i], "Empty Item Slot");
            }
        }

        if (BatEquipObject != null && BatItem != null) {
            ItemAvailable(BatEquipObject, BatItem);
        } else if (BatEquipObject != null && BatItem == null) {
            ResetItemFrame(BatEquipObject, "Bat Equip slot");
        }

        if (EdenEquipObject != null && EdenItem != null) {
            ItemAvailable(EdenEquipObject, EdenItem);
        } else if (EdenEquipObject != null && EdenItem == null) {
            ResetItemFrame(EdenEquipObject, "Eden Equip slot");
        }
     }

    private void ItemAvailable(GameObject image, Item item) {
        Image imageComponent = image.GetComponent<Image>(); // Added semicolon at the end
        imageComponent.sprite = item.icon; // Corrected variable name from Icomponent to component
        imageComponent.color = new Color(1f, 1f, 1f, 1f);

        Text titleText = image.transform.Find("ItemTitle").GetComponent<Text>();
        if (titleText != null) {
            titleText.text = item.name;
        } else {
            Debug.Log("title text object not named or made child of parent correctly");
        }

        Text costText = image.transform.Find("ItemCost").GetComponent<Text>();
        if (costText != null) {
            costText.text = "Worth: " + item.sellWorth;
        } else {
            Debug.Log("cost text object not named or made child of parent correctly");
        }

        Text descriptionText = image.transform.Find("ItemDescription").GetComponent<Text>();
        if (descriptionText != null) {
            descriptionText.text = "" + item.description;
        } else {
            Debug.Log("description text object not named or made child of parent correctly");
        }
    }

    private void ResetItemFrame(GameObject image, string titleName) {
        Image imageComponent = image.GetComponent<Image>(); // Added semicolon at the end
        imageComponent.sprite = null;
        imageComponent.color = new Color(0f, 0f, 0f, 0f);

        Text titleText = image.transform.Find("ItemTitle").GetComponent<Text>();
        if (titleText != null) {
            titleText.text = titleName;
        } else {
            Debug.Log("title text object not named or made child of parent correctly");
        }

        Text costText = image.transform.Find("ItemCost").GetComponent<Text>();
        if (costText != null) {
            costText.text = "";
        } else {
            Debug.Log("cost text object not named or made child of parent correctly");
        }

        Text descriptionText = image.transform.Find("ItemDescription").GetComponent<Text>();
        if (descriptionText != null) {
            descriptionText.text = "";
        } else {
            Debug.Log("description text object not named or made child of parent correctly");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bgfx = FindObjectOfType<GatBatController>().transform.Find("BatHolder").Find("GatBat").gameObject.GetComponent<SpriteRenderer>();
        pgfx = FindObjectOfType<PlayerGFX>().gameObject.GetComponent<SpriteRenderer>();
        
        UpdateInventorySprites();
    }

    public void EquipIndex(int itemInventoryIndex) {
        // Item tempItem;
        if (itemInventoryIndex == 0) {

            TransferItem(0);
            UpdateInventorySprites();

        } else if (itemInventoryIndex == 1) {
            TransferItem(1);
            UpdateInventorySprites();

            // Item tempItem = InventorySlots[1];
        } else if (itemInventoryIndex == 2) {
            TransferItem(2);
            UpdateInventorySprites();

            // Item tempItem = InventorySlots[2];
        } else if (itemInventoryIndex == 3) {
            TransferItem(3);
            UpdateInventorySprites();

            // Item tempItem = InventorySlots[3];
        } 
    }

    public void TransferItem(int index) {
        // if (InventorySlots[0] == null ) {
        //     ResetItemFrame(InventoryItem[index], "Empty Item Slot");
        // } 
        if (InventoryItem[index] == null) {

        } else {
            if (InventoryItem[index].isPlayerEquipable) {
                // EdenEquipObject
                Item tempItem = EdenItem;
                EdenItem = InventoryItem[index];
                InventoryItem[index] = tempItem;

                if (InventorySlots[index] == null ) {
                    ResetItemFrame(InventorySlots[index], "Eden Equip slot");
                } 
            } else {
                Item tempItem = BatItem;
                BatItem = InventoryItem[index];
                InventoryItem[index] = tempItem;

                if (InventorySlots[index] == null ) {
                    ResetItemFrame(InventorySlots[index], "Bat Equip slot");
                } 
            }

            if (InventorySlots[index] == null ) {
                ResetItemFrame(InventorySlots[index], "Empty Item Slot");
            } 
        }


    }

    // Update is called once per frame
    void Update()
    {
        GatBatIcon.sprite = bgfx.sprite;
        EdenIcon.sprite = pgfx.sprite;
    }


}
