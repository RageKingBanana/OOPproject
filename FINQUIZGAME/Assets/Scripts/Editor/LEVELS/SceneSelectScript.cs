using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelectScript : MonoBehaviour
{

    public void selectScene()
    {
        switch (this.gameObject.name)
        {
            case "Level1":
                SceneManager.LoadScene("Level1");
                break;
            case "Level2":
                SceneManager.LoadScene("Level2");
                break;
            case "Level3":
                SceneManager.LoadScene("Level3");
                break;
            case "Level4":
                SceneManager.LoadScene("Level1");
                break;
            case "Level5":
                SceneManager.LoadScene("Level2");
                break;
        }
    }

}