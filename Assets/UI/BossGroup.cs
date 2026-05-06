using UnityEngine;
using UnityEngine.UI;

public class BossGroup : MonoBehaviour
{
    private EnemyManager enemyManager;
    private CanvasGroup canvasGroup;
    private Slider bossHealthSlider;

    public CanvasGroup CanvasGroup { get => canvasGroup; set => canvasGroup = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        bossHealthSlider = GetComponentInChildren<Slider>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyManager.BossEnemy != null)
        {
            bossHealthSlider.value = enemyManager.BossEnemy.GetHealthPercentage();
            canvasGroup.alpha = 1f;
        }
    }

    
}
