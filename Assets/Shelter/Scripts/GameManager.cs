using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{

    // Use this for initialization
    private string[] dictionary = new string[7] { "Pizza", "Squirrel", "Acorn", "Hack", "House", "Dog", "Tree" };
    NetworkClient myClient;
    void Start()
    {
        myClient = new NetworkClient();
        myClient.RegisterHandler(MyMessageType.Shape, OnServerReadyToBeginMessage);
        myClient.Connect("192.168.56.1", 4444);
    }

    void OnServerReadyToBeginMessage(NetworkMessage networkMessage) {
        ShapeMessage msg = networkMessage.ReadMessage<ShapeMessage>();
        Debug.Log("Got a message");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
