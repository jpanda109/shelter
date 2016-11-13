using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // Use this for initialization
    private string[] dictionary = new string[7] { "Pizza", "Squirrel", "Acorn", "Hack", "House", "Dog", "Tree" };
    NetworkClient myClient;
    GameObject infield = GameObject.FindWithTag("Input");
    Text testText;
    void Start()
    {
        Debug.Log("hello");
        Debug.Log(infield.name);
        myClient = new NetworkClient();
        myClient.RegisterHandler(MyMessageType.Shape, OnShapeMessage);
        myClient.Connect("10.66.175.175", 4444);
        testText = GameObject.FindGameObjectWithTag("TestText").GetComponent<Text>();
    }

    void OnShapeMessage(NetworkMessage networkMessage) {
        ShapeMessage msg = networkMessage.ReadMessage<ShapeMessage>();
        Debug.Log("Got a message");
        testText.text = "Got a message";
    }

    // Update is called once per frame
    void Update()
    {
        if (testText.text == "New Text" && myClient.isConnected) {
            testText.text = "connected!";
        }
    }
}
