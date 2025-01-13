using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossArea : MonoBehaviour
{
    [SerializeField] GameObject boss;


    // Start is called before the first frame update
    void Start()
    {
        AreaDetector.onEnterArea += ActivateBoss;
    }

    void ActivateBoss(GameObject area)
    {
        if(area == gameObject)
        {
            boss.SetActive(true);
        } 
    }


}
