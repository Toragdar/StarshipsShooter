using UnityEngine.SceneManagement;
using UnityEngine;

public class ScenesManager : MonoBehaviour
{
    Scenes scenes;
    public enum Scenes
    {
        bootUp,
        title,
        shop,
        level1,
        level2,
        level3,
        gameOver
    }
    public void BeginGame()
    {
        SceneManager.LoadScene("testLevel");
    }
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GameOver()
    {
        Debug.Log("ENDSCORE : " + GameManager.Instance.GetComponent<ScoreManager>().PlayerScore);
        SceneManager.LoadScene("gameOver");
    }
}
