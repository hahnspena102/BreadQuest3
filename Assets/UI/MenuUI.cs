using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class MenuUI : MonoBehaviour
{
    private Player player;
    [SerializeField] private CanvasGroup menuCanvasGroup;

     void Start() {
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        if (player.IsInMenu)
        {
            menuCanvasGroup.alpha = 1f;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            menuCanvasGroup.alpha = 0f;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }
    }
}