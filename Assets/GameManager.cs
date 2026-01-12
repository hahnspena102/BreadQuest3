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
}
