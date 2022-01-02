using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUIManager : MonoBehaviour
{
    public GameObject fadeImage;
    public Sprite[] headSprites;
    public Sprite[] bodySprites;
    public GameObject buttonPrefab;
    public Vector2 startPoint;
    public float dx;
    public float dy;
    public string part;
    public int headIndex = -1;
    public int bodyIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = 0;
    }

    public void TouchBackButton()
    {
        SoftBody player = FindObjectOfType<SoftBody>();
        for (int i = 0; i < player.transform.GetChild(0).childCount; i++)
        {
            player.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < player.transform.GetChild(1).childCount; i++)
        {
            player.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
        }
        if (headIndex != -1)
        {
            player.transform.GetChild(0).GetChild(headIndex).gameObject.SetActive(true);
        }
        if (bodyIndex != -1)
        {
            player.transform.GetChild(1).GetChild(bodyIndex).gameObject.SetActive(true);
        }
        Time.timeScale = 1;
        fadeImage.SetActive(false);
        gameObject.SetActive(false);
    }

    public void SelectHead()
    {
        part = "Head";

        ShopButton[] preButtons = FindObjectsOfType<ShopButton>();
        for (int i = 0; i < preButtons.Length; i++)
        {
            Destroy(preButtons[i].gameObject);
        }

        for (int i = 0; i < headSprites.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab) as GameObject;
            button.GetComponent<ShopButton>().index = i;
            button.transform.SetParent(transform, false);
            button.transform.localPosition = new Vector2(startPoint.x + (i % 4) * dx, startPoint.y + (i / 4) * dy);
            button.transform.GetChild(0).GetComponent<Image>().sprite = headSprites[i];
        }
    }

    public void SelectBody()
    {
        part = "Body";

        ShopButton[] preButtons = FindObjectsOfType<ShopButton>();
        for (int i = 0; i < preButtons.Length; i++)
        {
            Destroy(preButtons[i].gameObject);
        }

        for (int i = 0; i < bodySprites.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab) as GameObject;
            button.GetComponent<ShopButton>().index = i;
            button.transform.SetParent(transform, false);
            button.transform.localPosition = new Vector2(startPoint.x + (i % 4) * dx, startPoint.y + (i / 4) * dy);
            button.transform.GetChild(0).GetComponent<Image>().sprite = bodySprites[i];
        }
    }
}
