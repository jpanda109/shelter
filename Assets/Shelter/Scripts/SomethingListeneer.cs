using System;
using UnityEngine;
using System.Collections;
using Leap.Unity;
using Leap;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

public struct Coord {
    public double x;
    public double y;
    public double z;
}

public class MyMessageType {
    public static short Shape = MsgType.Highest + 1;
}

public class ShapeMessage : MessageBase {
    public string shapeType;
    public Vector3 position;
    public Vector3 dimensions;
    public Quaternion rotation;
}

public class SomethingListeneer : MonoBehaviour {
    LeapProvider provider;
    public Transform block;
    public Transform cylinder;
    public Transform sphere;
    int lastState = 0;
    int currentState = 0;
    Transform curBlock;
    Vector originalHandPos;

    Text modeText;
    Text selectedText;
    Text positionText;
    Text dimensionsText;
    Text colorText;
    int rotationIndex = 0;

	// Use this for initialization
	void Start () {
        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
        modeText = GameObject.FindWithTag("ModeText").GetComponent<Text>();
        selectedText = GameObject.FindWithTag("SelectedText").GetComponent<Text>();
        positionText = GameObject.FindWithTag("PositionText").GetComponent<Text>();
        dimensionsText = GameObject.FindWithTag("DimensionsText").GetComponent<Text>();
        colorText = GameObject.FindWithTag("ColorText").GetComponent<Text>();

        NetworkServer.Listen(4444);
	}

    // Update is called once per frame
	void Update () {
        Frame frame = provider.CurrentFrame;
        List<Vector> indexFingerCoords = new List<Vector>();
        int numRightExtended = 0;

        foreach (Hand hand in frame.Hands) {
            if (hand.IsLeft) {
                currentState = 0;
                foreach (Finger finger in hand.Fingers) {
                    if (finger.IsExtended) {
                        currentState += 1;
                    }
                }
            } else if (hand.IsRight) {
                foreach (Finger finger in hand.Fingers) {
                    if (finger.IsExtended) {
                        numRightExtended += 1;
                    }
                }
            }
        }
        if (currentState == 0 && lastState == 1 && curBlock == null) {
            Debug.Log("Create Block");
            switch (numRightExtended) {
                case 1:
                    curBlock = (Transform)Instantiate(block, new Vector3(0, 0, 3), Quaternion.identity);
                    break;
                case 2:
                    curBlock = (Transform)Instantiate(cylinder, new Vector3(0, 0, 3), Quaternion.identity);
                    break;
                case 3:
                    curBlock = (Transform)Instantiate(sphere, new Vector3(0, 0, 3), Quaternion.identity);
                    break;
                default:
                    curBlock = (Transform)Instantiate(block, new Vector3(0, 0, 3), Quaternion.identity);
                    break;
            }
            selectedText.text = "Object Selected: True";
            Vector3 scale = curBlock.transform.localScale;
            Vector3 position = curBlock.transform.position;
            dimensionsText.text = String.Format("Dimensions: ({0,5:00.00}, {1,5:00.00}, {2,5:00.00})", scale.x, scale.y, scale.z);
            positionText.text = String.Format("Position: ({0,5:00.00}, {1,5:00.00}, {2,5:00.00})", position.x, position.y, position.z);
        }
        foreach (Hand hand in frame.Hands) {
            if (hand.IsRight) {
                float hCorr = .08f;
                switch (currentState) {
                    case 1:
                        modeText.text = "Mode: Shape Creation";
                        indexFingerCoords.Add(hand.Fingers[1].TipPosition);
                        break;
                    case 2:
                        modeText.text = "Mode: X Scale";
                        if (curBlock != null) {
                            float height = hand.PalmPosition.y;
                            Vector3 scale = curBlock.transform.localScale;
                            scale.y += (height + hCorr) / 10;
                            curBlock.transform.localScale = scale;
                            dimensionsText.text = String.Format("Dimensions: ({0,5:00.00}, {1,5:00.00}, {2,5:00.00})", scale.x, scale.y, scale.z);
                        }
                        break;
                    case 3:
                        modeText.text = "Mode: Y Scale";
                        if (curBlock != null ) {
                            float width = hand.PalmPosition.y;
                            Vector3 scale = curBlock.transform.localScale;
                            scale.x += (width + hCorr) / 10;
                            curBlock.transform.localScale = scale;
                            dimensionsText.text = String.Format("Dimensions: ({0,5:00.00}, {1,5:00.00}, {2,5:00.00})", scale.x, scale.y, scale.z);
                        }
                        break;
                    case 4:
                        modeText.text = "Mode: Z Scale";
                        if (curBlock != null) {
                            float length = hand.PalmPosition.y;
                            Vector3 scale = curBlock.transform.localScale;
                            scale.z += (length + hCorr) / 10;
                            curBlock.transform.localScale = scale;
                            dimensionsText.text = String.Format("Dimensions: ({0,5:00.00}, {1,5:00.00}, {2,5:00.00})", scale.x, scale.y, scale.z);
                        }
                        break;
                    case 5:
                        modeText.text = "Mode: Rotation";
                        if (curBlock != null) {
                            Quaternion rotation = curBlock.transform.rotation;
                            float rCorr = .2f;
                            // curBlock.transform.Rotate(-Vector3.right * hand.PalmPosition.y * rCorr);
                            // curBlock.transform.Rotate(-Vector3.up * hand.PalmPosition.x * rCorr);
                            // curBlock.transform.Rotate(-Vector3.forward * hand.PalmPosition.z * rCorr);
                            if (rotationIndex == 0) {
                                curBlock.transform.Rotate(-Vector3.right * hand.Direction.Pitch * rCorr);
                            } else if (rotationIndex == 1) {
                                curBlock.transform.Rotate(-Vector3.up * hand.Direction.Yaw * rCorr);
                            } else if (rotationIndex == 2) {
                                curBlock.transform.Rotate(-Vector3.forward * (hand.Direction.Roll+3.1415f/2) * rCorr);
                            }
                            rotationIndex = (rotationIndex + 1) % 3;
                        }
                        break;
                    default:
                        if (curBlock != null) {
                            int numExtended = 0;
                            foreach (Finger finger in hand.Fingers) {
                                if (finger.IsExtended) {
                                    numExtended += 1;
                                }
                            }
                            if (numExtended == 0) {
                                modeText.text = "Mode: Movement";
                                float xCorr = -.04f;
                                float yCorr = .08f;
                                float zCorr = -.2f;
                                Vector3 position = curBlock.transform.position;
                                position.x += (hand.PalmPosition.x + xCorr) / 10;
                                position.y += (hand.PalmPosition.y + yCorr) / 10;
                                position.z += (hand.PalmPosition.z + zCorr) / 10;
                                curBlock.transform.position = position;
                                positionText.text = String.Format("Position: ({0,5:00.00}, {1,5:00.00}, {2,5:00.00})", position.x, position.y, position.z);
                            } else {
                                if (hand.PalmVelocity.x > 1) {
                                    selectedText.text = "Object Selected: False";
                                    positionText.text = "Position: N/A";
                                    dimensionsText.text = "Dimensions: N/A";
                                    ShapeMessage msg = new ShapeMessage();
                                    msg.shapeType = "block";
                                    msg.position = curBlock.transform.position;
                                    msg.rotation = curBlock.transform.rotation;
                                    msg.dimensions = curBlock.transform.localScale;
                                    NetworkServer.SendToAll(MyMessageType.Shape, msg);
                                    curBlock = null;
                                } else if (hand.PalmVelocity.x < -1) {
                                    Destroy(curBlock.gameObject);
                                    curBlock = null;
                                    selectedText.text = "Object Selected: False";
                                    positionText.text = "Position: N/A";
                                    dimensionsText.text = "Dimensions: N/A";
                                } else {
                                    modeText.text = "Mode: Neutral";
                                }
                            }
                        }

                        break;
                }
            }
        }
        lastState = currentState;
	}

    public void OnConnected(NetworkMessage netMsg) {
        Debug.Log("Connected to server");
    }
}
