using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI TextHP;
    
    void Start()
    {
        StatsManager.Instance.currentHP = StatsManager.Instance.maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        TextHP.text = "HP: " + StatsManager.Instance.currentHP + "/" + StatsManager.Instance.maxHP;
    }
}
