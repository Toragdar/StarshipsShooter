using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static int playerLives = 3;
    public static int currentScene = 0;
    public static int gameLevelScene = 3;

    bool died = false;
    public bool Died
    {
        get { return died; }
        set { died = value; }
    }
    static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        CheckGameManagerIsInScene();
        currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        LightAndCameraSetup(currentScene);
    }
    public void LifeLost()
    {
        if (playerLives > 1)
        {
            playerLives--;
            Debug.Log("Lives left: " + playerLives);
            GetComponent<ScenesManager>().ResetScene();
        }
        else
        {
            playerLives = 3;
            GetComponent<ScenesManager>().GameOver();
        }
    }

    void CheckGameManagerIsInScene()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
    }
    void LightAndCameraSetup(int sceneNum)
    {
        switch (sceneNum)
        {
            //testLevel, level1, level2, level3
            case 3: case 4: case 5: case 6:
                {
                    LightSetup();
                    CameraSetup();
                    break;
                }
        }
    }

    void CameraSetup()
    {
        GameObject gameCamera = GameObject.FindGameObjectWithTag("MainCamera");

        //Camera Transform
        gameCamera.transform.position = new Vector3(0, 0, -300);
        gameCamera.transform.eulerAngles = new Vector3(0, 0, 0);

        //Camera Properties
        gameCamera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
        gameCamera.GetComponent<Camera>().backgroundColor = new Color32(0, 0, 0, 255);
    }
    void LightSetup()
    {
        GameObject dirLight = GameObject.Find("Directional Light");

        dirLight.transform.eulerAngles = new Vector3(50, -30, 0);
        dirLight.GetComponent<Light>().color = new Color32(152, 204, 255, 255);
    }
}
