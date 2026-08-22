using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    public EnemyHP enemy;
    private Slider _hpSlider;

    void Start()
    {
        _hpSlider = GetComponent<Slider>();
    }

    void Update()
    {
        _hpSlider.value = enemy.currentHP / enemy.enemyData.HP;
    }
}
