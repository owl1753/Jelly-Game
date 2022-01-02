using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUIManager : MonoBehaviour
{
    public GameObject fadeImage;

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
        Time.timeScale = 1;
        fadeImage.SetActive(false);
        gameObject.SetActive(false);
    }

    public void TouchExitButton()
    {
        Application.Quit();
    }
}
