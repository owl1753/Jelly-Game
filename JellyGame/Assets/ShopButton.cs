using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    GameObject Head;
    GameObject Body;
    ShopUIManager shopUIManager;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        shopUIManager = FindObjectOfType<ShopUIManager>();
        Head = shopUIManager.transform.GetChild(3).GetChild(0).gameObject;
        Body = shopUIManager.transform.GetChild(3).GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TouchShopButton()
    {
        if (shopUIManager.part.Equals("Head"))
        {
            if (Head.transform.GetChild(index).gameObject.activeSelf)
            {
                Head.transform.GetChild(index).gameObject.SetActive(false);
                GameObject myEventSystem = GameObject.Find("EventSystem");
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
                shopUIManager.headIndex = -1;
            }
            else
            {
                shopUIManager.headIndex = index;
                for (int i = 0; i < Head.transform.childCount; i++)
                {
                    Head.transform.GetChild(i).gameObject.SetActive(false);
                }
                Head.transform.GetChild(index).gameObject.SetActive(true);
            }
        }
        else if (shopUIManager.part.Equals("Body"))
        {
            if (Body.transform.GetChild(index).gameObject.activeSelf)
            {
                shopUIManager.bodyIndex = -1;
                Body.transform.GetChild(index).gameObject.SetActive(false);
                GameObject myEventSystem = GameObject.Find("EventSystem");
                myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
            }
            else
            {
                shopUIManager.bodyIndex = index;
                for (int i = 0; i < Body.transform.childCount; i++)
                {
                    Body.transform.GetChild(i).gameObject.SetActive(false);
                }
                Body.transform.GetChild(index).gameObject.SetActive(true);
            }
        }
    }
}
