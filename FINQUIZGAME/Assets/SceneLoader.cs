﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{

    private bool loadScene = false;

    [SerializeField]
    private int scene;
    [SerializeField]
    private Text loadingText;
    public string[] tips =new string[5];

    // Updates once per frame
    private void Start()
    {


    }
    void Update()
    {
        int n;
        n = Random.Range(0, 8);

        // If the player has pressed the space bar and a new scene is not loading yet...
        if (loadScene == false)
        {
            Debug.Log(false);
            // ...set the loadScene boolean to true to prevent loading a new scene more than once...
            loadScene = true;

            // ...change the instruction text to read "Loading..."
            loadingText.text = tips [n];

            // ...and start a coroutine that will load the desired scene.
            StartCoroutine(LoadNewScene());

        }

        // If the new scene has started loading...
        if (loadScene == true)
        {

            // ...then pulse the transparency of the loading text to let the player know that the computer is still working.
           // loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
            Debug.Log(true);
        }

    }


    // The coroutine runs on its own at the same time as Update() and takes an integer indicating which scene to load.
    IEnumerator LoadNewScene()
    {

        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(10);
        scene = PlayerPrefs.GetInt("levelClicked");

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync(scene);

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }

    }

}