using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   
    public void PlayGame()
    {

        SceneManager.LoadScene(1); //loads the game

    }

    public void MainMenuQuitGame()
    {
        Application.Quit(); // quting the game from main menu
    }

}
