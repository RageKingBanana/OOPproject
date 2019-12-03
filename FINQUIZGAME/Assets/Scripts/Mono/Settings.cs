using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer audiomixer;
    public Slider SliderInstance;
    private float v;
    public float s;
    private void Start ()
    {
        s = PlayerPrefs.GetFloat("volume");
        SliderInstance.value = s;
    }
    public void ChangeVolume(float volume)
    {
        v = volume;
        audiomixer.SetFloat("volume", volume);
    }
    private void Update()
    {
        PlayerPrefs.SetFloat("volume", v);
    }
}