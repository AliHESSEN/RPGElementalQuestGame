using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOverMenu : MonoBehaviour
{
  
    public void PlayGameAgain()
    {
        SceneManager.LoadScene(1); //loads the game


    }

    public void QuitGame()
    {
        Application.Quit(); // quiting the game
    }



}
