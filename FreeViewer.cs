using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using NGenerics.Algorithms;
using NGenerics.DataStructures.General;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class FreeViewer : MonoBehaviour
{

    public string filePathAndName = "";
    public int numberOfFrames = 0;
    public float frameRate = 30;
    public Dictionary<string, List<Texture2D>> videoDic;
    public string currentFile;

    public AudioClip room308;
    public AudioClip menroom;
    public AudioClip room306;
    public AudioClip elevator;

    private List<Texture2D> currentFrameList = new List<Texture2D>();
    public float playTime = 0f;
    private AudioSource sound;
    private int currentFrame = 0;
    private int currentMaxFrameNumber;

    public Button annotaionBTN;
    public bool reviewMode;
    public bool annotationMode;
    public UIWindowBase annotationWindow;
    bool tmpreviewMode = false;
    bool tmpplayNavigationMode = false;

    public Button Room308BTN;
    public Button AisleBTN;
    public InputField FromVertexInput;
    public InputField ToVertexInput;

    public Graph<string> videoGraph = new Graph<string>(true);
    private Queue<string> pathQueue = new Queue<string>();
    private DataBaseControl databaseControl;
    public bool playNavigationMode;
    private Queue<string> videoQueue = new Queue<string>();
    private int TotalNavFrames;
    private AssetBundle myBundle;
    private List<AssetBundle> bundleList = new List<AssetBundle>();
    private Texture2D tmpTexture;
    private int tmpCurrentFrame = 0;
    private float playSpeed = 0.6f;
    private Dictionary<Texture2D, string> rotate360Dic = new Dictionary<Texture2D, string>();
    private List<Texture2D> rotate360List = new List<Texture2D>();
    private bool beginSpin = false;
    private float angle = 0;
    private float spinSpeed = 0.02f;
    private string voiceFile = "";
    private UIWindowBase myWindow;

    // Use this for initialization
    void Start()
    {
        sound = GetComponent<AudioSource>();
        videoDic = new Dictionary<string, List<Texture2D>>();
        reviewMode = true;
        playNavigationMode = false;
        annotationMode = false;
        databaseControl = new DataBaseControl();
        StartCoroutine(LoadGraph());
        myBundle = new AssetBundle();
        //StartCoroutine(SetLoadFramesForReview(filePathAndName));
        annotaionBTN.onClick.AddListener(onAnnotationClicked);
    }

    // Update is called once per frame+
    void Update()
    {
        if (reviewMode)
            PlayReview();

        if (annotationMode)
            PlayAnnotation();

        if (playNavigationMode)
            PlayNavigation();
    }

    //Jump to SetLoadFrames
    public void ToLoadFrames(string fileInfo)
    {
        Debug.LogWarning("ToLoadFrames: " + fileInfo);
        StopAllCoroutines();
        StartCoroutine(SetLoadFramesForReview(fileInfo));
    }

    //Set Load Frames For ReviewMode
    public IEnumerator SetLoadFramesForReview(string fileInfo)
    {
        float startTime = Time.realtimeSinceStartup;
        reviewMode = true;
        playNavigationMode = false;
        annotationMode = false;
        string[] fileInfos = fileInfo.Split('&');
        filePathAndName = fileInfos[0];
        numberOfFrames = int.Parse(fileInfos[1]);
        currentFrame = 0;
        playTime = 0;
        currentMaxFrameNumber = numberOfFrames;
        Debug.Log(filePathAndName);
        Debug.Log(numberOfFrames);
        playTime = float.Parse(fileInfos[2]) / frameRate;
        List<Texture2D> frames = new List<Texture2D>();
        Texture2D tmp = null;
        currentFile = filePathAndName;
        if (!videoDic.ContainsKey(filePathAndName))
        {
            videoDic.Add(filePathAndName, frames);
            for (int i = 0; i < numberOfFrames; ++i)
            {
                ResourceRequest request = Resources.LoadAsync(filePathAndName + "/Stitched " + string.Format("{0:d3}", i + 1));
                yield return request;
                tmp = request.asset as Texture2D;
                ((videoDic[filePathAndName])).Add(tmp);
                currentFrameList = (videoDic[filePathAndName]);
                //if (i == 1)
                //    rotate360List.Add(tmp);
                Resources.UnloadAsset(tmp);

            }
        }
        else if (((videoDic[filePathAndName])).Count < numberOfFrames)
        {
            Debug.LogWarning("Not Finish load frames: ");
            int size = ((videoDic[filePathAndName])).Count;
            for (int i = size; i < numberOfFrames; ++i)
            {
                ResourceRequest request = Resources.LoadAsync(filePathAndName + "/Stitched " + string.Format("{0:d3}", i + 1));
                yield return request;
                tmp = request.asset as Texture2D;
                if (!((videoDic[filePathAndName])).Contains(tmp))
                {
                    ((videoDic[filePathAndName])).Add(tmp);
                    currentFrameList = videoDic[filePathAndName];
                }
                Resources.UnloadAsset(tmp);
            }
        }
        else
        {
            currentFrameList = videoDic[filePathAndName];
        }
        float costTime = Time.realtimeSinceStartup - startTime;
    }

    //Set loading frames for NavMode
    public IEnumerator SetLoadFramesForNavigation(Queue<string> videoQueue)
    {
        reviewMode = false;
        playNavigationMode = true;
        annotationMode = false;
        List<Texture2D> frames = new List<Texture2D>();
        TotalNavFrames = 99999999;
        int framesCount = 0;
        int tmptotal = 0;
        currentFrame = 0;
        playTime = 0;
        currentFrameList.Clear();
        frames.Clear();
        Texture2D tmp = null;
        while (videoQueue.Count > 0)
        {
            Debug.LogWarning("Video Queue Size: " + videoQueue.Count);
            string fileInfo = videoQueue.Dequeue();
            Debug.Log("file info: " + fileInfo);
            string[] fileInfos = fileInfo.Split('&');
            filePathAndName = fileInfos[0];
            var firstFrameNum = int.Parse(fileInfos[1]);
            var lastFrameNum = int.Parse(fileInfos[2]);
            //currentMaxFrameNumber = lastFrameNum;
            Debug.Log(filePathAndName);
            Debug.Log(lastFrameNum);
            framesCount = lastFrameNum - firstFrameNum;
            Debug.LogWarning("framesCount: " + framesCount);
            tmptotal += Math.Abs(framesCount);
            yield return Resources.UnloadUnusedAssets();
            if (framesCount >= 0)
            {
                currentFrameList = frames;
                for (int i = firstFrameNum; i < lastFrameNum; i++)
                {
                    var request = Resources.LoadAsync(filePathAndName + "/Stitched " + string.Format("{0:d3}", i + 1));
                    yield return request;
                    tmp = request.asset as Texture2D;
                    frames.Add(tmp);
                    if (i == firstFrameNum)
                    {
                        if (!rotate360Dic.ContainsKey(tmp))
                            rotate360Dic.Add(tmp, filePathAndName);
                        rotate360List.Add(tmp);
                    }
                    if (i == lastFrameNum - 1)
                    {
                        if (!rotate360Dic.ContainsKey(tmp))
                            rotate360Dic.Add(tmp, filePathAndName);
                    }
                    Resources.UnloadAsset(tmp);
                }
            }
            else
            {
                for (int i = firstFrameNum; i > lastFrameNum; i--)
                {
                    var request = Resources.LoadAsync(filePathAndName + "/Stitched " + string.Format("{0:d3}", i + 1));
                    yield return request;
                    tmp = request.asset as Texture2D;
                    frames.Add(tmp);
                    if (i == firstFrameNum)
                    {
                        if (!rotate360Dic.ContainsKey(tmp))
                            rotate360Dic.Add(tmp, filePathAndName);
                        rotate360List.Add(tmp);
                    }
                    if (i == lastFrameNum - 1)
                    {
                        if (!rotate360Dic.ContainsKey(tmp))
                            rotate360Dic.Add(tmp, filePathAndName);
                    }
                    Resources.UnloadAsset(tmp);
                }
            }

        }
        TotalNavFrames = tmptotal;
        frames = null;
    }

    //Play audio prompt
    private void PlayAudioReminder(float time)
    {
        if (!sound.isPlaying)
        {
            if (7.6 < time && time < 8.9)
            {
                sound.clip = menroom;
                sound.Play();
                Debug.Log("Play men room");
            }
            else if (10.6 < time && time < 12.5)
            {
                sound.clip = elevator;
                sound.Play();
                Debug.Log("Play elevator");
            }
            else if (19.68 < time && time < 21.6)
            {
                sound.clip = room306;
                sound.Play();
                Debug.Log("Play room 306");
            }
            else if (25 < time && time < 26.5)
            {
                sound.clip = room308;
                sound.Play();
                Debug.Log("Play room 308");
            }
            else
            {
                sound.Stop();
                Debug.Log("Stop play audio");
            }
        }
    }

    //Click annotation BUtton
    void onAnnotationClicked()
    {
        if (!annotationMode)
        {
            annotationMode = true;
            tmpreviewMode = reviewMode;
            tmpplayNavigationMode = playNavigationMode;
            reviewMode = false;
            playNavigationMode = false;
            annotaionBTN.GetComponentsInChildren<Text>()[0].text = "Save";
            myWindow = Instantiate(annotationWindow, new Vector3((float)0.5 * Screen.width, (float)0.5 * Screen.height, 0), new Quaternion()) as UIWindowBase;
            Button tts = myWindow.GetComponentInChildren<Button>();
            tts.onClick.AddListener(TextToSpeech);
            voiceFile = "";
        }
        else
        {
            string annotation = myWindow.GetComponentInChildren<InputField>().text;
            string date = DateTime.Now.ToString();
            float camAngle = Camera.main.transform.eulerAngles.y;
            string fileName = currentFrameList[currentFrame].name;
            if (voiceFile != "")
            {
                databaseControl.SaveAnnotation(currentFile, fileName, date, camAngle, annotation, voiceFile);
            }
            else
            {
                databaseControl.SaveAnnotation(currentFile, fileName, date, camAngle, annotation, null);
            }
            Destroy(myWindow.gameObject, 0f);
            annotationMode = false;
            playNavigationMode = tmpplayNavigationMode;
            reviewMode = tmpreviewMode;
            annotaionBTN.GetComponentsInChildren<Text>()[0].text = "Annotation";

        }
    }

    //Click BeginNavigaiton Button
    void onBeginNavClicked()
    {
        string FromVertexInfo = FromVertexInput.text;
        string ToVertexInfo = ToVertexInput.text;
        var FromVertex = videoGraph.GetVertex(FromVertexInfo);
        var ToVertex = videoGraph.GetVertex(ToVertexInfo);
        FindPath(FromVertex, ToVertex);

    }

    //Load vertexes and build graph
    public IEnumerator LoadGraph()
    {
        videoGraph = databaseControl.LoadGraph();
        yield return videoGraph;
    }

    // Find a path between two vertexes
    public void FindPath(Vertex<string> FromVertex, Vertex<string> ToVertext)
    {
        Debug.LogWarning("FromVertex Data: " + FromVertex.Data);
        Debug.LogWarning("ToVertex Data: " + ToVertext.Data);
        videoQueue.Clear();
        Graph<string> pathGraph = GraphAlgorithms.DijkstrasAlgorithm<string>(videoGraph, FromVertex);
        var endVertex = pathGraph.GetVertex(ToVertext.Data);
        pathQueue.Clear();
        pathQueue.Enqueue(ToVertext.Data);
        string fromVertexID = FromVertex.Data;
        string nextVertexID = null;
        var nextVertex = endVertex;
        while (nextVertexID != fromVertexID)
        {
            nextVertexID = nextVertex.IncidentEdges[0].FromVertex.Data;
            if (nextVertexID == nextVertex.Data)
                nextVertexID = nextVertex.IncidentEdges[1].FromVertex.Data;
            pathQueue.Enqueue(nextVertexID);
            Debug.LogWarning("next vertex: " + nextVertexID);
            nextVertex = pathGraph.GetVertex(nextVertexID);
        }

        var pathQueueSize = pathQueue.Count;
        List<string>[] tmpList = new List<string>[pathQueueSize];
        var pathlist = pathQueue.ToList();
        for (int i = pathlist.Count - 1; i > 0; i--)
        {
            Debug.LogWarning("path list i: " + pathlist[i]);
            var list0 = databaseControl.GetVideoList(pathlist[i]);
            Debug.LogWarning("path list i-1: " + pathlist[i - 1]);
            var list1 = databaseControl.GetVideoList(pathlist[i - 1]);
            var commons = list0.Intersect(list1);
            Debug.LogWarning(commons.FirstOrDefault());
            Debug.LogWarning("videoQueue size: " + videoQueue.Count);
            string playInfo = databaseControl.GetPlayInfo(pathlist[i], pathlist[i - 1], commons.FirstOrDefault());
            videoQueue.Enqueue(playInfo);
        }


        reviewMode = false;
        StopAllCoroutines();
        currentFrameList.Clear();
        StartCoroutine(SetLoadFramesForNavigation(videoQueue));
        playNavigationMode = true;


    }

    public void PlayNavigation()
    {
        if (currentFrameList.Count > 0)
        {
            Debug.LogWarning("CurrentFrame: " + currentFrame + "," + currentFrameList.Count);
            playTime = playTime + (float)0.6 * Time.fixedDeltaTime; // normal forward
            currentFrame = (int)(playTime * frameRate);
            if (currentFrame > currentFrameList.Count - 1)
            {
                currentFrame = currentFrameList.Count - 1;
                playNavigationMode = false;
                reviewMode = true;
                playTime = 0;
                Debug.LogWarning("Play Done.");
            }
            else if (currentFrame < 0)
            {
                playTime = 0;
                currentFrame = 0;
            }
            if (tmpTexture != currentFrameList[currentFrame])
            {
                tmpCurrentFrame = currentFrame;
                Resources.UnloadAsset(tmpTexture);
                tmpTexture = currentFrameList[currentFrame];
            }
            gameObject.GetComponent<Renderer>().material.mainTexture = currentFrameList[currentFrame];
            if (rotate360List.Contains(currentFrameList[currentFrame]))
            {
                rotate360List.Remove(currentFrameList[currentFrame]);
                StartCoroutine(CamSpin());
                currentFile = rotate360Dic[currentFrameList[currentFrame]];
                Debug.LogWarning("Frame file path: " + currentFile);
            }
        }
    }

    public void PlayReview()
    {
        if (currentFrameList.Count > 0)
        {
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
            {
                playTime += Time.deltaTime; // normal forward
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
            {
                playTime += Time.deltaTime * 2; // fast forward
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space))
            {
                playTime += Time.deltaTime / 2; // slow forward
            }
            else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
            {
                playTime -= Time.deltaTime; // normal backward
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift))
            {
                playTime -= Time.deltaTime * 2; // fast backward
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.Space))
            {
                playTime -= Time.deltaTime / 2; // slow backward
            }
            currentFrame = (int)(playTime * frameRate);
            Debug.Log("CurrentFrame: " + currentFrame + "," + currentFrameList.Count);
            if (currentFrame >= currentFrameList.Count)
            {
                Debug.LogWarning("Play done");
                playTime = currentFrameList.Count / frameRate;
                currentFrame = currentFrameList.Count - 1;
            }
            else if (currentFrame < 0)
            {
                playTime = 0;
                currentFrame = 0;
            }
            gameObject.GetComponent<Renderer>().material.mainTexture = currentFrameList[currentFrame];
            PlayAudioReminder(playTime);
            Debug.Log("Play time is " + playTime);
            if (rotate360Dic.ContainsKey(currentFrameList[currentFrame]))
                currentFile = rotate360Dic[currentFrameList[currentFrame]];
            if (tmpTexture != currentFrameList[currentFrame])
            {
                Resources.UnloadAsset(tmpTexture);
                tmpTexture = currentFrameList[currentFrame];
            }
            PlayAnnotation();
        }
    }

    public void PlayAnnotation()
    {
        string frame = "Stitched " + string.Format("{0:d3}", currentFrame + 1);
        string voicefile = databaseControl.AnnotationContains(currentFile, frame);
        if (voicefile == null)
             return;
        Debug.LogWarning(voicefile);
        if (!sound.isPlaying)
        {
            sound.clip = Resources.Load("Sounds/" + voicefile) as AudioClip;
            sound.Play();
            Debug.LogWarning("Play Voice File");
        }
    }


    // Make the camera spin 360
    private IEnumerator CamSpin()
    {
        playNavigationMode = false;
        Camera.main.transform.eulerAngles = new Vector3(0, 0, 0);
        while (0 <= Camera.main.transform.rotation.eulerAngles.y && Camera.main.transform.rotation.eulerAngles.y < 357)
        {
            Camera.main.transform.Rotate(Vector3.up, spinSpeed * 40, Space.World);
            yield return Camera.main.transform.rotation.eulerAngles.y;
        }
        playNavigationMode = true;
        Camera.main.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public IEnumerator GetAnnotaion(string filePath, string fileDate)
    {
        Debug.LogWarning("In getannotation");
        reviewMode = false;
        playNavigationMode = false;
        annotationMode = false;
        Debug.LogWarning(fileDate);
        Debug.LogWarning(filePath);
        ResourceRequest request = Resources.LoadAsync(filePath);
        yield return request;
        var tmp = request.asset as Texture2D;
        gameObject.GetComponent<Renderer>().material.mainTexture = tmp;

    }

    void TextToSpeech()
    {
        string text = myWindow.GetComponentInChildren<InputField>().text;
        Voice myvoice = new Voice();
        voiceFile = currentFile + " " + currentFrameList[currentFrame].name + " " + DateTime.Now.Year + " " + DateTime.Now.Month + " " + DateTime.Now.Day + " " + DateTime.Now.Hour + " " + DateTime.Now.Minute;
        myvoice.CreatePromt(text, voiceFile);
    }
}
