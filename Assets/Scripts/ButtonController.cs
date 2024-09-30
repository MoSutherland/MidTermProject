using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{


    public void RestartLevel()
    {
        GameManager.Instance.RestartLevel();
    }

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

}
