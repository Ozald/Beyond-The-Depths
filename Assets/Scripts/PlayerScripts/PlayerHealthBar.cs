using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public StatsManager statsManager;
    public Vector3 offset;

    private Slider _hpSlider;
    private CanvasGroup canvasGroup;

    void Start()
    {
        _hpSlider = GetComponent<Slider>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        _hpSlider.value = (float)statsManager.currentHP / statsManager.maxHP;
        transform.rotation = Camera.main.transform.rotation;

        /*
        if (enemy.currentHP <= 0 || enemy.currentHP >= enemy.enemyData.HP)
            canvasGroup.alpha = 0;
        else
            canvasGroup.alpha = 1;
        */
    }
}
