﻿using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI.Extensions;
using UnityEngine.SceneManagement;

public class CanvasScriptForFree : MonoBehaviour
{

    private GameObject enter308;
    private Button enter308BTN;
    private Text[] enter308TXT;
    private float playTime = 0f;
    private float backTime;
    private string fileinfo;
    private bool inRoom308;
    private Dropdown dropDown;
    private DataBaseControl databasecontrol = new DataBaseControl();
    public UIWindowBase annotationWindow;
    private List<string> options;

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
        dropDown = GameObject.Find("VideoList").GetComponent<Dropdown>();
        options = databasecontrol.GetAllVideoList();
        options.Insert(0, "Please Select A Video.");
        StartCoroutine(LoadGoogleMap());
    }

    private void call()
    {
        if (!inRoom308)
        {
            backTime = GameObject.Find("sphere screen").GetComponent<FreeViewer>().playTime;
            fileinfo = "frames 007&385&0";
            Debug.LogWarning("Click Enter 308: " + fileinfo);
            GameObject.Find("sphere screen").GetComponent<FreeViewer>().ToLoadFrames(fileinfo);
            inRoom308 = true;
            //GameObject.Find("sphere screen").GetComponent<FreeViewer>().frameRate = 10;
        }
        else
        {
            fileinfo = "frames 002&749&" + backTime;
            Debug.LogWarning("Click Enter 308: " + fileinfo);
            GameObject.Find("sphere screen").GetComponent<FreeViewer>().ToLoadFrames(fileinfo);
            inRoom308 = false;
            // GameObject.Find("sphere screen").GetComponent<FreeViewer>().frameRate = 30;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playTime = GameObject.Find("sphere screen").GetComponent<FreeViewer>().playTime;
        Debug.Log("Play time In CanvasScript: " + playTime);
        TimeCheck(playTime);
    }

    void TimeCheck(float time)
    {
        if (GameObject.Find("sphere screen").GetComponent<FreeViewer>().reviewMode)
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

    // Show Annotation
    public void ShowAnnotation()
    {
        string option = dropDown.captionText.text;
        Debug.LogWarning(option);
        var optionContent = option.Split(',');
        string filePath = optionContent[0];
        string fileDate = optionContent[1];
        GameObject.Find("sphere screen").GetComponent<FreeViewer>().reviewMode = false;
        GameObject.Find("sphere screen").GetComponent<FreeViewer>().annotationMode = false;
        GameObject.Find("sphere screen").GetComponent<FreeViewer>().playNavigationMode = false;
        //loadTmpTexture(filePath, fileDate);
        var tmp = Resources.Load(filePath) as Texture2D;
        GameObject.Find("sphere screen").GetComponent<Renderer>().material.mainTexture = tmp;
        float camAngle = databasecontrol.GetAnnotaionAngle(fileDate);
        Debug.LogWarning(camAngle);
        Camera.main.transform.eulerAngles = new Vector3(0,camAngle,0);
        string comment = databasecontrol.GetAnnoatationContent(fileDate);
        Debug.LogWarning(comment);
        UIWindowBase myWindow = Instantiate(annotationWindow, new Vector3((float)0.5 * Screen.width, (float)0.5 * Screen.height, 0), new Quaternion()) as UIWindowBase;
        myWindow.GetComponentInChildren<InputField>().text = comment;
    }

    public void ShowVideo()
    {
        int value = dropDown.value;
        Debug.LogWarning(value);
        if (value == 0)
            return;
        string option = dropDown.captionText.text;
        Debug.LogWarning(option);
        string fileinfo = databasecontrol.GetVideoInfo(option);
        GameObject.Find("sphere screen").GetComponent<FreeViewer>().ToLoadFrames(fileinfo);
    }

    private IEnumerator loadTmpTexture(string filePath, string fileDate)
    {
        ResourceRequest request = Resources.LoadAsync(filePath);
        yield return request;
        var tmp = request.asset as Texture2D;
        GameObject.Find("sphere screen").GetComponent<Renderer>().material.mainTexture = tmp;
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public IEnumerator LoadGoogleMap()
    {
        dropDown.AddOptions(options);
        var reqaddress = "https://maps.googleapis.com/maps/api/staticmap?center=Brooklyn+Bridge,New+York,NY&zoom=13&size=600x300&maptype=roadmap";
        //var req = new WWW(url + "?" + qs);
        var req = new WWW(reqaddress);
        yield return req;
        gameObject.GetComponentInChildren<RawImage>().texture = req.texture;
    }
}
