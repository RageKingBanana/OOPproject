using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{

    private int levelReached;
    public Button[] levelButtons;
    public int prevLevelPlayed;
    public int Iskor;
    public double compare;
    public string levelU;
    public char[] levelUc = new char[5];
    void Start()
    {
        NiceFunc();


    }
    private void Update()
    {

    }
    public void NiceFunc()
    {
        levelReached = PlayerPrefs.GetInt("levelReached", 1);
        Iskor = PlayerPrefs.GetInt("iskor");
        levelU = PlayerPrefs.GetString("levelU");

        prevLevelPlayed = PlayerPrefs.GetInt("levelClicked");

        Debug.Log(Iskor);
        Debug.Log(levelU);

        if (levelU == "")
        {
            PlayerPrefs.SetString("levelU", "10000");
            levelU = PlayerPrefs.GetString("levelU");
        }

        levelUc = levelU.ToCharArray(0, levelUc.Length);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                levelButtons[i].interactable = false;
            }
        }
        //compare = PlayerPrefs.GetInt("iskor3");
        //compare = (Iskor + compare) * 0.50;
        for (int i = 0; i < levelUc.Length; i++)

            if (Iskor > 9 && prevLevelPlayed == i)
                levelUc[prevLevelPlayed] = '1';

        string str = new string(levelUc);
        PlayerPrefs.SetString("levelU", str);
        //PlayerPrefs.SetInt("iskor3", 0);
        PlayerPrefs.SetInt("iskor", 0);
        for (int x = 0; x < levelButtons.Length; x++)
        {
            if (levelUc[x] == '1' && x < 5)
                levelButtons[x].interactable = true;
        }

    }
}
   



/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelSelection : MonoBehaviour
{
    public Button[] lvlButtons;

    // Start is called before the first frame update
    void Start()
    {
        int levelAt = PlayerPrefs.GetInt("levelAt", 1);  < Change this int value to whatever your
                                                             level selection build index is on your
                                                             build settings 

        for (int i = 0; i < lvlButtons.Length; i++)
        {
            if (i + 1 > levelAt)
                lvlButtons[i].interactable = false;
        }
    }

}*/
