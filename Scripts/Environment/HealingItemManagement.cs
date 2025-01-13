using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingItemManagement : MonoBehaviour
{
    ObjectPooling healingPool;
    GameObject healingObject;
    [SerializeField] Transform[] healingObjectPosition;
    [SerializeField] float healingItemRecharge = 25;
    int healingObjectAmount;

    // Start is called before the first frame update
    void Start()
    {
        healingPool = GetComponent<ObjectPooling>();
        healingObjectAmount = healingObjectPosition.Length;
        SpawnInitialHealth();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnHealth()
    {
        healingObject = healingPool.GetPooledObject();
        if (healingObject != null)
        {
            healingObject.SetActive(true);
        }
    }

    void SpawnInitialHealth()
    {
        for (int i = 0; i < healingObjectAmount; i++)
        {
            SpawnHealth();
            healingObject.transform.position = healingObjectPosition[i].position;
        }
        InvokeRepeating("SpawnHealth", healingItemRecharge, healingItemRecharge);
    }
}
