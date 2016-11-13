﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // Use this for initialization

    NetworkClient myClient;
    InputField infield;
    Text testText;
    public Text clock;

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
    }

    private void submitName(string guess)
    {
        Debug.Log(guess);
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

        timer += Time.deltaTime;
        clock.text = "Time elapsed: " + timer.ToString("#.000");


    }
}
