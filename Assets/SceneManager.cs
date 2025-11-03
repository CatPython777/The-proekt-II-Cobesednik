using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneLoader : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("игра");
    }
    
    public void LoadLevel1()
    {
        SceneManager.LoadScene("proekt");
    }
}