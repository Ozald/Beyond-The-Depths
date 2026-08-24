using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    public StatsManager playerStats;
    public PlayerInventory playerInventory;
    public bool loadData = false;

    public GameObject swordPrefab;
    public GameObject tridentPrefab;
    public GameObject bookPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        if (loadData)
        {
            // HP Loading
            StatsManager.Instance.maxHealth.SetBaseValue(PlayerPrefs.GetInt("maxHealth"));
            StatsManager.Instance.currentHP = PlayerPrefs.GetInt("currentHP");

            // Player Stats Loading
            StatsManager.Instance.speed.SetBaseValue(PlayerPrefs.GetFloat("speed"));
            StatsManager.Instance.damage.SetBaseValue(PlayerPrefs.GetInt("damage"));
            StatsManager.Instance.knockback.SetBaseValue(PlayerPrefs.GetFloat("knockback"));
            StatsManager.Instance.attackRange.SetBaseValue(PlayerPrefs.GetFloat("attackRange"));
            StatsManager.Instance.critChance.SetBaseValue(PlayerPrefs.GetFloat("critChance"));
            StatsManager.Instance.attackSpeed.SetBaseValue(PlayerPrefs.GetFloat("attackSpeed"));
            StatsManager.Instance.luck.SetBaseValue(PlayerPrefs.GetInt("luck"));
            StatsManager.Instance.playerSize.SetBaseValue(PlayerPrefs.GetFloat("playerSize"));

            // Money Loading
            StatsManager.Instance.doubloons = PlayerPrefs.GetInt("doubloons");

            // Inventory Loading
            int weaponID1 = PlayerPrefs.GetInt("weaponID1");
            int weaponID2 = PlayerPrefs.GetInt("weaponID2");

            // Weapon 1
            Weapon weapon1 = null;
            switch (weaponID1)
            {
                case 0:
                    weapon1 = Instantiate(swordPrefab).GetComponent<Weapon>();
                    break;
                case 1:
                    weapon1 = Instantiate(tridentPrefab).GetComponent<Weapon>();
                    break;
                case 2:
                    weapon1 = Instantiate(bookPrefab).GetComponent<Weapon>();
                    break;
            }

            weapon1.CanInteract = false;
            PlayerInventory.instance.playerInv.Add(weapon1);

            // Weapon 2
            Weapon weapon2 = null;
            switch (weaponID2)
            {
                case 0:
                    weapon2 = Instantiate(swordPrefab).GetComponent<Weapon>();
                    break;
                case 1:
                    weapon2 = Instantiate(tridentPrefab).GetComponent<Weapon>();
                    break;
                case 2:
                    weapon2 = Instantiate(bookPrefab).GetComponent<Weapon>();
                    break;
            }

            weapon2.CanInteract = false;
            PlayerInventory.instance.playerInv.Add(weapon2);
        }
        else
        {
            // If loadData is false, initialize default values
            StatsManager.Instance.maxHealth.SetBaseValue(10);
            StatsManager.Instance.speed.SetBaseValue(5);
            StatsManager.Instance.attackSpeed.SetBaseValue(0);
            StatsManager.Instance.damage.SetBaseValue(0);
            StatsManager.Instance.defense.SetBaseValue(0);
            StatsManager.Instance.critChance.SetBaseValue(0.1f);
            StatsManager.Instance.attackRange.SetBaseValue(1);
            StatsManager.Instance.luck.SetBaseValue(0);
            StatsManager.Instance.knockback.SetBaseValue(1);

            StatsManager.Instance.currentHP = StatsManager.Instance.maxHealth.value;
            StatsManager.Instance.doubloons = 0;

            // Initialize default weapons
            Weapon defaultWeapon1 = Instantiate(swordPrefab).GetComponent<Weapon>();
            defaultWeapon1.CanInteract = false;
            PlayerInventory.instance.playerInv.Add(defaultWeapon1);
        }
    }

    public static void SaveData()
    {
        // HP Saving
        PlayerPrefs.SetInt("maxHealth", StatsManager.Instance.maxHealth.value);
        PlayerPrefs.SetInt("currentHP", StatsManager.Instance.currentHP);

        // Player Stats Saving
        PlayerPrefs.SetFloat("speed", StatsManager.Instance.speed.value);
        PlayerPrefs.SetInt("damage", StatsManager.Instance.damage.value);
        PlayerPrefs.SetFloat("knockback", StatsManager.Instance.knockback.value);
        PlayerPrefs.SetFloat("attackRange", StatsManager.Instance.attackRange.value);
        PlayerPrefs.SetFloat("critChance", StatsManager.Instance.critChance.value);
        PlayerPrefs.SetFloat("attackSpeed", StatsManager.Instance.attackSpeed.value);
        PlayerPrefs.SetInt("luck", StatsManager.Instance.luck.value);
        PlayerPrefs.SetFloat("playerSize", StatsManager.Instance.playerSize.value);

        // Money Saving
        PlayerPrefs.SetInt("doubloons", StatsManager.Instance.doubloons);

        // Inventory Saving
        PlayerPrefs.SetInt("weaponID1", PlayerInventory.instance.playerInv[0].weaponData.weaponDataID);
        PlayerPrefs.SetInt("weaponID2", PlayerInventory.instance.playerInv[1].weaponData.weaponDataID);

        PlayerPrefs.Save();
    }
}
