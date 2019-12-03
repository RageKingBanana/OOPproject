using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeChanger : MonoBehaviour
{
   /* private void Update()
    {

    }*/
    [Header("CurrentSettings")]
    [SerializeField] float volume = 1;
    public void ChangeSFX(float _vol)
    {
        volume = _vol;
        AudioListener.volume = _vol;



    }
}