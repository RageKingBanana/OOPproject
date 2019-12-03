using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LEVELSELECT : MonoBehaviour
{

//    [SerializeField] GameEvents events = null;
    public void selectScene()
    {
        switch (this.gameObject.name)
        {
            case "Level1":
                PlayerPrefs.SetInt("levelClicked",1);
                SceneManager.LoadScene(6);
                break; 
            case "Level2":
                PlayerPrefs.SetInt("levelClicked", 2);
                SceneManager.LoadScene(6);
                break;
            case "Level3":
                PlayerPrefs.SetInt("levelClicked", 3);
                SceneManager.LoadScene(6);
                break;
            case "Level4":
                PlayerPrefs.SetInt("levelClicked", 4);
                SceneManager.LoadScene(6);
                break;
            case "Level5":
                PlayerPrefs.SetInt("levelClicked", 5);
                SceneManager.LoadScene(6);
                break;
        }
        

    }


}