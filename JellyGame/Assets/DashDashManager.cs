using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DashDashManager : MonoBehaviour
{
    public Transform obstacleHolder;
    public int hp;
    public int score;
    public GameObject[] obstaclePrefabs;
    public float scrollSpeed;
    public float spawnTimer;
    public float spawnInterval;
    public float scoreInterval;
    public float scoreTimer;

    public Image hpBar;
    public GameObject menu;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreText2;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(hp <= 0)
        {
            menu.SetActive(true);
            scoreText.text = "Score: " + score;
            Time.timeScale = 0;
            return;
        }
        Time.timeScale = 1;
        menu.SetActive(false);

        scoreText2.text = "Score: " + score;

        obstacleHolder.position += Vector3.left * scrollSpeed * Time.deltaTime;
        
        if (spawnTimer < 0)
            Spawn();
        else
            spawnTimer -= Time.deltaTime;

        if (scoreTimer < 0)
        {
            score++;
            scoreTimer = scoreInterval;
        }
        else
            scoreTimer -= Time.deltaTime;

        hpBar.fillAmount = hp / 5f;
    }

    public void Initialize()
    {
        hp = 5;
        score = 0;
        obstacleHolder.position = Vector3.zero;
        spawnTimer = 0;
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            Destroy(o);
        }
        
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }

    void Spawn()
    {
        int idx = Random.Range(0, obstaclePrefabs.Length);

        GameObject spawnedObject = Instantiate(obstaclePrefabs[idx], obstacleHolder);
        
        switch (idx)
        {
            case 0:
                Destroy(spawnedObject);
                break;
            case 1:
                spawnedObject.transform.position = new Vector2(11, -2.47f);
                break;
            case 2:
                spawnedObject.transform.position = new Vector2(11, -2.19f);
                break;
            case 3:
                spawnedObject.transform.position = new Vector2(13, -1.16f);
                break;
        }

        spawnTimer = spawnInterval + Random.Range(-1, 1);
    }
}
