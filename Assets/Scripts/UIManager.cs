using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject ply1WinScreen;
    public GameObject ply2WinScreen;

    public void ply1Win()
    {
        ply1WinScreen.SetActive(true);
    }

    public void ply2Win()
    {
        ply2WinScreen.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene("UndergroundColosseum");
    }
}
