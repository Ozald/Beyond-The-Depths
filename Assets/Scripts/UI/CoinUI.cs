using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{

    public TextMeshProUGUI currentCoins;

    public StatsManager statsManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentCoins.text = statsManager.doubloons.ToString();
    }
}
