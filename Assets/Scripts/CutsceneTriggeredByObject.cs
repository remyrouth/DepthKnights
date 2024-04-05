using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggeredByObject : MonoBehaviour
{
    public List<GameObject> activationList = new List<GameObject>();

    public void StartScene() {
        ActivateItems();
    }

    // Method to activate all items in the activationList
    private void ActivateItems()
    {
        foreach(GameObject obj in activationList)
        {
            obj.SetActive(true);
        }
    }
}
