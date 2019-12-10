using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class progres : MonoBehaviour
{
    // Start is called before the first frame update
    public int Iskore;
    void Start()
    {
        Iskore = PlayerPrefs.GetInt("iskor");
        if(Iskore > 40)
        {
            SceneManager.LoadScene(7);
        }
    }

    // Update is called once per frame
 
}
