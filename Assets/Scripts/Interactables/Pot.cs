using System.Collections;
using UnityEngine;

public class Pot : MonoBehaviour
{
    public WeightedItem[] items;
    private ItemSelector<GameObject> LootPool;
    public bool isOpen = false;
    public Animator animator;

    void Start()
    {
        LootPool = new ItemSelector<GameObject>();

        foreach (WeightedItem item in items)
            LootPool.AddItem(item.item, item.weight);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            AttackHitboxData data = other.gameObject.GetComponent<AttackHitboxData>();
            if (data != null && !isOpen)
                StartCoroutine(OpenPot());
        }
    }

    public IEnumerator OpenPot()
    {
        isOpen = true;
        yield return new WaitForSeconds(0.2f);

        GameObject item = LootPool.Roll();

        Instantiate(item, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}