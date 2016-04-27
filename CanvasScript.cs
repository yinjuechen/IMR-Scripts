﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using System;

public class CanvasScript : MonoBehaviour
{

    private GameObject enter308;
    private Button enter308BTN;
    private Text[] enter308TXT;
    private float playTime;
    private float backTime;
    private string fileinfo;
    private bool inRoom308;




    // Use this for initialization
    void Start()
    {
        enter308 = GameObject.Find("Enter308");
        enter308BTN = enter308.GetComponent<Button>();
        enter308BTN.interactable = false;
        enter308TXT = enter308BTN.GetComponentsInChildren<Text>();
        enter308TXT[0].text = "";
        enter308BTN.onClick.AddListener(call);
        fileinfo = "frames 007&385&0";
        inRoom308 = false;
    }

    private void call()
    {
        if (!inRoom308)
        {
            backTime = GameObject.Find("sphere screen").GetComponent<FrameViewer>().playTime;
            fileinfo = "frames 007&385&0";
            Debug.LogWarning("Click Enter 308: " + fileinfo);
            GameObject.Find("sphere screen").GetComponent<FrameViewer>().ToLoadFrames(fileinfo);
            inRoom308 = true;
            //GameObject.Find("sphere screen").GetComponent<FrameViewer>().frameRate = 10;
        }
        else
        {
            fileinfo = "frames 002&749&" + backTime;
            Debug.LogWarning("Click Enter 308: " + fileinfo);
            GameObject.Find("sphere screen").GetComponent<FrameViewer>().ToLoadFrames(fileinfo);
            inRoom308 = false;
            // GameObject.Find("sphere screen").GetComponent<FrameViewer>().frameRate = 30;
        }
    }

    // Update is called once per frame
    void Update()
    {

        playTime = GameObject.Find("sphere screen").GetComponent<FrameViewer>().playTime;
        Debug.Log("Play time In CanvasScript: " + playTime);
        TimeCheck(playTime);
    }



    void TimeCheck(float time)
    {
        if (GameObject.Find("sphere screen").GetComponent<FrameViewer>().reviewMode)
        {
            if (!inRoom308)
            {
                if (20.6 < time && time < 21.8)
                {
                    enter308BTN.interactable = true;
                    enter308TXT[0].text = "Enter Room 308";
                    Debug.Log("Enter room 308");
                }
                else
                {
                    enter308BTN.interactable = false;
                    enter308TXT[0].text = "";
                }
            }
        }
        else if (inRoom308)
        {
            if (0 <= time && time < 1.6)
            {
                enter308BTN.interactable = true;
                enter308TXT[0].text = "Back to Aisle";
                Debug.Log("Back to Aisle");
            }
            else
            {
                enter308BTN.interactable = false;
                enter308TXT[0].text = "";
            }
        }
    }


}