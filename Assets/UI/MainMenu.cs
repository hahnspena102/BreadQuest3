using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Inventory inventory;
    [SerializeField] private PlayerData starterPlayerData;
    [SerializeField] private Inventory starterInventory;
    public void NewGame() {

        playerData.ResetToStarter(starterPlayerData);
        inventory.ResetToStarter(starterInventory);

        SceneManager.LoadScene("SampleScene");
    }

    

}
