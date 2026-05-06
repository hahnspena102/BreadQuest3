using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Awake() {
        int countLoaded = SceneManager.sceneCount;
        Scene[] loadedScenes = new Scene[countLoaded];

        for (int i = 0; i < countLoaded; i++)
        {
            loadedScenes[i] = SceneManager.GetSceneAt(i);
            if (loadedScenes[i].name == "UIScene")
            {
                return;
            }
        }

        
        SceneManager.LoadSceneAsync("UIScene", LoadSceneMode.Additive);




    }

    void Start() {
        //RandomizeToppings();
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.U)) {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void GameOver() {
        SceneManager.LoadScene("GameOver");
    }

    public static int FloorToTier(int floor) {
        if (floor <= 3) return 1;
        if (floor <= 6) return 2;
        if (floor <= 9) return 3;
        if (floor <= 12) return 4;
        if (floor <= 15) return 5;
        return 6;
    }

    public static float CalculateValueByFloor(float baseValue, float scale, int floor) {
        // Example scaling: 10% increase per floor
        int level = floor / 5; // Every 5 floors increases the level
        level -= 1;


        return baseValue * (1 + scale * (floor - 1));
    }

    [SerializeField]private ToppingData[] allToppings;

    public void RandomizeToppings() {
        int numToppings = 3;
        ToppingData[] selectedToppings = new ToppingData[numToppings];
        for (int i = 0; i < numToppings; i++) {
            int randomIndex = Random.Range(0, allToppings.Length);
            ToppingData randomTopping = allToppings[randomIndex];
            selectedToppings[i] = randomTopping;
            Debug.Log("Selected Topping: " + randomTopping.name);
        }

        ToppingsPanel toppingsPanel = FindFirstObjectByType<ToppingsPanel>();
        if (toppingsPanel != null) {
            toppingsPanel.DisplayToppings(selectedToppings);
        }
    }

    public void OnBossDefeated() {
        RandomizeToppings();
        BossGroup bossGroup = FindFirstObjectByType<BossGroup>();
        if (bossGroup != null) {
            bossGroup.CanvasGroup.alpha = 0f;
            bossGroup.CanvasGroup.interactable = false;
        }
    }

}

