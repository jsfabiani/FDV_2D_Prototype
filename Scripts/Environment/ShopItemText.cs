using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemText : MonoBehaviour
{
    [SerializeField] GameObject shopItemText;
    [SerializeField] private float textOffset = 70f;

    // Start is called before the first frame update
    void Start()
    {
        shopItemText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 shopTextPos = Camera.main.WorldToScreenPoint(this.transform.position);
        shopItemText.transform.position = shopTextPos + new Vector3(0.0f, textOffset, 0.0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            shopItemText.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            shopItemText.SetActive(false);
        }
    }

    void OnDisabled()
    {
        shopItemText.SetActive(false);
    }
}
