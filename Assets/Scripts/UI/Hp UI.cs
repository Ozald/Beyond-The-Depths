using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HpUI : MonoBehaviour
{
    public TextMeshProUGUI currentHP;

    public TextMeshProUGUI maxHP;

    public StatsManager statsManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentHP.text = statsManager.currentHP.ToString();
        maxHP.text = statsManager.maxHP.ToString();
    }
}
