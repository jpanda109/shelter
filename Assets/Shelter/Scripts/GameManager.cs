using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    // Use this for initialization
    private string[] dictionary = new string[7] { "Pizza", "Squirrel", "Acorn", "Hack", "House", "Dog", "Tree" };
    NetworkClient myClient;
    public InputField infield;

    void Start()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MyMessageType.Shape, OnServerReadyToBeginMessage);
        myClient.RegisterHandler(MsgType.Error, OnError);
        myClient.Connect("10.66.175.175", 4444);


    }

    void OnServerReadyToBeginMessage(NetworkMessage networkMessage) {
        ShapeMessage msg = networkMessage.ReadMessage<ShapeMessage>();
        Debug.Log("Got a message");
    }

    void OnError(NetworkMessage netMsg) {
        Debug.Log("Error");
    }

    public void OnConnected(NetworkMessage netMsg) {
        Debug.Log("Connected to server");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
