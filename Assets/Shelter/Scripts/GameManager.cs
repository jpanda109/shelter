using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // Use this for initialization

    NetworkClient myClient;
    InputField infield;
    Text testText;
    public Text clock;

    public Transform block;
    public Transform cylinder;
    public Transform sphere;

    double timer;
    string drawing;

    void Start()
    {
        timer = 0.0;
        infield = GameObject.FindGameObjectWithTag("Input").GetComponent<InputField>();
        infield.onEndEdit.AddListener(submitName);
        myClient = new NetworkClient();
        myClient.RegisterHandler(MyMessageType.Shape, OnShapeMessage);
        myClient.Connect("10.66.175.175", 4444);
        testText = GameObject.FindGameObjectWithTag("TestText").GetComponent<Text>();
        NetworkServer.RegisterHandler(MyMessageType.Result, onResult);
    }

    void onResult (NetworkMessage result)
    {
        if (result.ReadMessage<StringMessage>().value == "success")
        {
            Application.LoadLevel("Victory Screen");
        } else
        {
            StartCoroutine(ShowMessage("Try again!", 2));
        }
    }

    IEnumerator ShowMessage (string message, float delay)
    {
        GetComponent<GUIText>().text = message;
        GetComponent<GUIText>().enabled = true;
        yield return new WaitForSeconds(delay);
        GetComponent<GUIText>().enabled = false;
    }

    private void submitName(string guess)
    {
        Debug.Log(guess);
        myClient.Send(MyMessageType.String, new StringMessage(guess));
    }
        
    void OnShapeMessage(NetworkMessage networkMessage) {
        ShapeMessage msg = networkMessage.ReadMessage<ShapeMessage>();
        Debug.Log("Got a message");
        testText.text = "Got a message";
        Transform curBlock;
        if (msg.shapeType == "block") {
            curBlock = (Transform)Instantiate(block, new Vector3(0, 0, 3), Quaternion.identity);
        } else if (msg.shapeType == "cylinder") {
            curBlock = (Transform)Instantiate(cylinder, new Vector3(0, 0, 3), Quaternion.identity);
        } else if (msg.shapeType == "sphere") {
            curBlock = (Transform)Instantiate(sphere, new Vector3(0, 0, 3), Quaternion.identity);
        } else {
            curBlock = (Transform)Instantiate(block, new Vector3(0, 0, 3), Quaternion.identity);
        }
        curBlock.transform.position = msg.position;
        curBlock.transform.localScale = msg.dimensions;
        curBlock.transform.rotation = msg.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (testText.text == "New Text" && myClient.isConnected) {
            testText.text = "connected!";
        }

        timer += Time.deltaTime;
        clock.text = "Time elapsed: " + timer.ToString("#.000");


    }
}
