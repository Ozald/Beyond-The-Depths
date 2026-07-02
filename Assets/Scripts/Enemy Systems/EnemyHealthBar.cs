using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHP enemy;
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
        _hpSlider.value = enemy.currentHP / enemy.enemyData.HP;
        transform.rotation = Camera.main.transform.rotation;
        transform.position = enemy.transform.position + offset;

        if (enemy.currentHP <= 0 || enemy.currentHP >= enemy.enemyData.HP)
            canvasGroup.alpha = 0;
        else
            canvasGroup.alpha = 1;
    }
}
