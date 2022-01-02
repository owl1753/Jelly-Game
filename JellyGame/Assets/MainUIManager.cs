using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    public GameObject fadeImage;
    public GameObject optionUIs;
    public GameObject miniGameUIs;
    public GameObject shopUIs;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TouchOptionButton()
    {
        fadeImage.SetActive(true);
        optionUIs.SetActive(true);
    }

    public void TouchMiniGameButton()
    {
        fadeImage.SetActive(true);
        miniGameUIs.SetActive(true);
    }

    public void TouchShopButton()
    {
        fadeImage.SetActive(true);
        shopUIs.SetActive(true);
    }
}
