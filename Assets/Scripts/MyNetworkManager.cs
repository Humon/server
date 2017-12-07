using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;


public class MyMsgTypes
{
	public static short MSG_STOP_ARM = 1003;
    public static short MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS = 1009;
    public static short MSG_FREEZE_ARM_POSITION = 1013;
    public static short MSG_CHAT = 1019;
    public static short MSG_REQUEST_CARTESIAN_POSITION = 1018;
}


public class GetCartesianCommandsMessage : MessageBase
{

}

public class MoveArmPositionWithFingersMessage : MessageBase
{
    public bool rightArm;
    public float x;
    public float y;
    public float z;
    public float thetaX;
    public float thetaY;
    public float thetaZ;
    public float fp1;
    public float fp2;
    public float fp3;
}

public class RequestCartesianPositionMessage : MessageBase {
    public float x;
    public float y;
    public float z;
    public float thetaX;
    public float thetaY;
    public float thetaZ;
    public float fp1;
    public float fp2;
    public float fp3;
}

public class ChatMessage : MessageBase {
    public string text;
}


public class FreezeArmPositionMessage : MessageBase
{
    public float x;
    public float y;
    public float z;
    public float thetaX;
    public float thetaY;
    public float thetaZ;
    public float fp1;
    public float fp2;
    public float fp3;

}

public class StopArmMessage : MessageBase
{
	public bool rightArm;
	public bool suppressLog;
}


public class MoveArmWithFingersMessage : MessageBase
{
    public bool rightArm;
    public float x;
    public float y;
    public float z;
    public float thetaX;
    public float thetaY;
    public float thetaZ;
    public float fp1;
    public float fp2;
    public float fp3;

}

public class MyNetworkManager : MonoBehaviour
{

    private bool handHasData = false;
    private bool handsInitialized = false;

    String commandLeft = "";
    String commandRight = "";


    public string address = "127.0.0.1";
    public int port = 11111;


    private bool isAtStartup = true;


    NetworkView clientView; // for sending to client(s) connected

    NetworkClient myClient;

    //public SampleUserPolling_ReadWrite handController;
    HUD hud;

    private void Start()
    {
        hud = FindObjectOfType<HUD>();
    }
    void Update()
    {

        if (isAtStartup) {
            if (Input.GetKeyDown(KeyCode.S)) {
                SetupServer();

            }

        }
    }

    

    // Create a server and listen on a port
    public void SetupServer()
    {
        KinovaAPI.InitRobot();
        NetworkServer.Listen(port);

        NetworkServer.RegisterHandler(MyMsgTypes.MSG_STOP_ARM, ReceiveStopArm);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_FREEZE_ARM_POSITION, ReceiveFreezeArmPosition);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS, ReceiveMoveArmWithFingers);
        NetworkServer.RegisterHandler((short)MyMsgTypes.MSG_CHAT, OnServerChatMessage);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_REQUEST_CARTESIAN_POSITION, ReceiveCartesianPositionRequest);

        isAtStartup = false;
        Debug.Log("Server running listening on port " + port);
    }


    private void ReceiveFreezeArmPosition(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<FreezeArmPositionMessage>(); // irrelevent, client doesn't need to send us anything except the request itself
        FreezeArmPositionMessage m = new FreezeArmPositionMessage();

        float[] positions = KinovaAPI.GetCartesianPositions();
        m.x = positions[0];
        m.y = positions[1];
        m.z = positions[2];
        m.thetaX = positions[3];
        m.thetaY = positions[4];
        m.thetaZ = positions[5];
        m.fp1 = positions[6];
        m.fp2 = positions[7];
        m.fp3 = 0;
        NetworkServer.SendToAll(MyMsgTypes.MSG_FREEZE_ARM_POSITION, m);
    }

    private void ReceiveCartesianPositionRequest(NetworkMessage netMsg) {
        var msg = netMsg.ReadMessage<RequestCartesianPositionMessage>(); // irrelevent, client doesn't need to send us anything except the request itself
        RequestCartesianPositionMessage m = new RequestCartesianPositionMessage();

        float[] positions = KinovaAPI.GetCartesianPositions();
        m.x = positions[0];
        m.y = positions[1];
        m.z = positions[2];
        m.thetaX = positions[3];
        m.thetaY = positions[4];
        m.thetaZ = positions[5];
        m.fp1 = positions[6];
        m.fp2 = positions[7];
        m.fp3 = 0;
        NetworkServer.SendToAll(MyMsgTypes.MSG_REQUEST_CARTESIAN_POSITION, m);
    }

    // Example pulled from internet to get basic server <--> client comm.
    // Can be removed.
    private void OnServerChatMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ChatMessage>();
        Debug.Log("New chat message on server: " + msg.text);
        ChatMessage m = new ChatMessage();
        m.text = "Server chat;" + msg.text;
        
        NetworkServer.SendToAll((short)MyMsgTypes.MSG_CHAT,m);
        
    }
    

    public void ReceiveMoveArmWithFingers(NetworkMessage message)
    {
        MoveArmPositionWithFingersMessage m = message.ReadMessage<MoveArmPositionWithFingersMessage>();
        KinovaAPI.MoveArmCartesianPositionWithFingers(m.rightArm, m.x, m.y, m.z, m.thetaX, m.thetaY, m.thetaZ, m.fp1, m.fp2, m.fp3);
        hud.armPosition.text = m.x.ToString("0.00")+","+m.y.ToString("0.00") + ","+","+m.z.ToString("0.00") + ": rot:"+m.thetaX.ToString("0.00") + "," + m.thetaY.ToString("0.00") + "," + m.thetaZ.ToString("0.00");
    }

    


    
    // Not currently used. Can be a more direct FREEZE than getting Positions (so robot doesn't "jog" during freeze, but freezes in place.)
    public void ReceiveGetCarteisanCommands(NetworkMessage message)
    {
        float[] positions = KinovaAPI.GetCartesianCommands();
        Debug.Log("<color=blue>Received:</color> got cart commands:"
            + positions[0] + ", "
            + positions[1] + ", "
            + positions[2] + ", "
            + positions[3] + ", "
            + positions[4] + ", "
            + positions[5] + ", "
            + positions[6] + ", "
            + positions[7]
            );
    }


  private void ReceiveStopArm (NetworkMessage message)
  {
	    StopArmMessage m = message.ReadMessage<StopArmMessage> ();
        
        // Debug.Log("<color=blue>Received:</color><color=red> stop arm at </color>" + Time.time);
        KinovaAPI.StopArm(m.rightArm);
        hud.stopReceived.text = Time.time.ToString();


    }
    


    private string ArmSide (bool rightArm)
  {
	return rightArm ? "right" : "left";
  }

    
}
		