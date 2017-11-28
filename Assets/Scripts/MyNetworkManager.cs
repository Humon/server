using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;


public class MyMsgTypes
{
	public static short MSG_MOVE_ARM = 1000;
	public static short MSG_MOVE_ARM_NO_THETAY = 1001;
	public static short MSG_MOVE_ARM_HOME = 1002;
	public static short MSG_STOP_ARM = 1003;
	public static short MSG_MOVE_FINGERS = 1004;
    public static short MSG_MOVE_HAND = 1005;
    public static short MSG_MOVE_ARM_ANGULAR_VELOCITY = 1006;
    public static short MSG_MOVE_ARM_ANGULAR_POSITION = 1007; // 3 new msgs
	public static short MSG_MOVE_ARM_ANGULAR_VELOCITY_LOOPED = 1008;
    public static short MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS = 1009;
    public static short MSG_SET_ARM_POSITION = 1010;
    public static short MSG_SET_FINGER_POSITION = 1011;
    public static short MSG_MOVE_ARM_UPDATE = 1012; // MoveArm in ARM_base32.dll
    public static short MSG_FREEZE_ARM_POSITION = 1013;
    public static short MSG_GET_CARTESIAN_POSITIONS = 1014;
    public static short MSG_GET_CARTESIAN_COMMANDS = 1015;
    public static short MSG_GET_CACHED_CARTESIAN_COMMANDS = 1016;

}
public class GetCachedCartesianCommandsMessage : MessageBase
{

}

public class GetCartesianPositionsMessage : MessageBase
{

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

// Charlie's 3 separate functions for setting/uypdating arm/finger positions
public class SetArmPositionMessage : MessageBase
{
    public bool rightArm;
    public float x;
    public float y;
    public float z;
    public float thetaX;
    public float thetaY;
    public float thetaZ;
}

public class SetFingerPositionMessage : MessageBase
{
    public bool rightArm;
    public float fp1;
    public float fp2;
    public float fp3;
}

public class MoveArmUpdateMessage : MessageBase
{

}

public class FreezeArmPositionMessage : MessageBase
{

}

// end Charlie

//public class MoveArmCartesianPosition_MoveRelativeMessage : MessageBase //new
//{
//    public bool rightArm;
//    public float X;
//    public float Y;
//    public float Z;
//    public float ThetaX;
//    public float ThetaY;
//    public float ThetaZ;
//}

//public class MoveArmAngularVelocityMessage : MessageBase //new
//{
//    public bool rightArm;
//    public float av1;
//    public float av2;
//    public float av3;
//    public float av4;
//    public float av5;
//    public float av6;
//    public float av7;
//}

//public class MoveArmAngularVelocityLoopedMessage : MessageBase //new
//{
//	public bool rightArm;
//	public int iterations;
//	public float av1;
//	public float av2;
//	public float av3;
//	public float av4;
//	public float av5;
//	public float av6;
//	public float av7;
//}

//public class MoveArmAngularPositionMessage : MessageBase //new
//{
//    public bool rightArm;
//    public int ap1;
//    public int ap2;
//    public int ap3;
//    public int ap4;
//    public int ap5;
//    public int ap6;
//    public int ap7;
//}

//public class MoveArmMessage : MessageBase
//{
//	public bool rightArm;
//	public float x;
//	public float y;
//	public float z;
//	public float thetaX;
//	public float thetaY;
//	public float thetaZ;
//}

//public class MoveArmNoThetaYMessage : MessageBase
//{
//	public bool rightArm;
//	public float x;
//	public float y;
//	public float z;
//	public float thetaX;
//	public float thetaZ;
//}

//public class MoveArmHomeMessage : MessageBase
//{
//	public bool rightArm;
//}

public class StopArmMessage : MessageBase
{
	public bool rightArm;
	public bool suppressLog;
}

//public class MoveFingersMessage : MessageBase
//{
//	public float []gloveData = new float[10];
//}

//public class ReceiveMoveToHandsMessage : MessageBase
//{
//	public int ring;
//}

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
  
  SerialPort handPortR;
  SerialPort handPortL;

  private float HandMoveDelay = 0.0f;

  public string address = "127.0.0.1";
  public int port = 11111;  
  public GameObject cameraRig;
  public VideoChatExample videoChat;

  private bool isAtStartup = true;
  private bool connectedToServer = false;
  private bool localRun = false;

  public static bool isServer = false;
    
  NetworkClient myClient;

    //public SampleUserPolling_ReadWrite handController;
    HUD hud;

    private void Start()
    {
        hud = FindObjectOfType<HUD>();
    }
    void Update ()
  {
	HandMoveDelay += Time.deltaTime;
	if (isAtStartup) {
	  if (Input.GetKeyDown (KeyCode.S)) {
		SetupServer ();
		  isServer = true;
	  }
            
	  
	}
  }

  void OnGUI ()
  {
	if (isAtStartup) {
	  GUI.Label (new Rect (2, 10, 200, 100), "Press S for server (robot)");     
	  //GUI.Label (new Rect (2, 30, 200, 100), "Press C for client (controller)");
	  //GUI.Label (new Rect (2, 50, 200, 100), "Press B for both");   
	 }
  }

  // Create a server and listen on a port
  public void SetupServer ()
  {
	KinovaAPI.InitRobot ();
	NetworkServer.Listen (port);
	//NetworkServer.RegisterHandler (MyMsgTypes.MSG_MOVE_ARM, ReceiveMoveArm);
	//NetworkServer.RegisterHandler (MyMsgTypes.MSG_MOVE_ARM_NO_THETAY, ReceiveMoveArmNoThetaY);
	//NetworkServer.RegisterHandler (MyMsgTypes.MSG_MOVE_ARM_HOME, ReceiveMoveArmHome);
	NetworkServer.RegisterHandler (MyMsgTypes.MSG_STOP_ARM, ReceiveStopArm);
 //   //Shawn testing 10.1.17
 //   NetworkServer.RegisterHandler(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_VELOCITY, RecieveMoveArmAngularVelocity);
	//NetworkServer.RegisterHandler(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_VELOCITY_LOOPED, ReceieveMoveArmAngularVelocityLooped);
 //   NetworkServer.RegisterHandler(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_POSITION, RecieveMoveArmAngularPosition);
 //   NetworkServer.RegisterHandler(MyMsgTypes.MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS, RecieveMoveArms_WithFingers);

        // Charlie added the 3 separated functions
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_SET_ARM_POSITION, ReceiveSetArmPosition);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_SET_FINGER_POSITION, ReceiveSetFingerPosition);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_MOVE_ARM_UPDATE, ReceiveMoveArmUpdate);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_FREEZE_ARM_POSITION, ReceiveFreezeArmPosition);

        NetworkServer.RegisterHandler(MyMsgTypes.MSG_GET_CARTESIAN_POSITIONS, ReceiveGetCarteisanPositions);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_GET_CARTESIAN_COMMANDS, ReceiveGetCarteisanCommands);
        NetworkServer.RegisterHandler(MyMsgTypes.MSG_GET_CACHED_CARTESIAN_COMMANDS, ReceiveGetCachedCarteisanCommands);

        NetworkServer.RegisterHandler(MyMsgTypes.MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS, ReceiveMoveArmWithFingers);
        


    //Testing
    //  NetworkServer.RegisterHandler (MyMsgTypes.MSG_MOVE_FINGERS, ReceiveMoveFingers);


    //if (!localRun) {
    //  videoChat.gameObject.SetActive (true); 
    //  videoChat.StartVideoChat ();
    //}
    isAtStartup = false;
	Debug.Log ("Server running listening on port " + port);
  }
    
  // Create a client and connect to the server port
 // public void SetupClient ()
 // {
	//myClient = new NetworkClient ();
	//InitClient ();    
	//myClient.Connect (address, port);
	//Debug.Log ("Started client");
 // }
    
 // // Create a local client and connect to the local server
 // public void SetupLocalClient ()
 // {
	//myClient = ClientScene.ConnectLocalServer ();
	//InitClient ();
	//videoChat.remoteView.GetComponent<CameraController>().StartLocalStream();
	//Debug.Log ("Started local client");
 // }

 // private void InitClient ()
 // {
	//myClient.RegisterHandler (MsgType.Connect, OnConnected);
	//cameraRig.SetActive (true); // transitively enables VIVE controllers
	//if (!localRun) {
	//  videoChat.gameObject.SetActive (true);
	//}
	//isAtStartup = false;
 // }

 // // Client function
 // public void OnConnected (NetworkMessage netMsg)
 // {
	//Debug.Log ("Connected to server on " + address + ":" + port);
	//if (!localRun) {
	//  Invoke ("JoinVideoChat", 3.0f);
	//}
	//connectedToServer = true;
 // }

  private void JoinVideoChat ()
  {
	videoChat.JoinVideoChat ();
  }

    //shawn test
    //    public void SendMoveArmCartesianPosition_MoveRelativeMessage(bool rightArm, float X, float Y, float Z, float ThetaX, float ThetaY, float ThetaZ)
    //{
    //    if (!connectedToServer)
    //    {
    //        Debug.LogWarning("Not connected to server!");
    //        return;
    //    }

    //    Debug.Log("Sending move " + ArmSide(rightArm) + "with MoveArmCartesianPositionMoveRelative sent!");

    //    MoveArmCartesianPosition_MoveRelativeMessage m = new MoveArmCartesianPosition_MoveRelativeMessage();
    //    m.rightArm = rightArm;
    //    m.X = X;
    //    m.Y = Y;
    //    m.Z = Z;
    //    m.ThetaX = ThetaX;
    //    m.ThetaY = ThetaY;
    //    m.ThetaZ = ThetaZ;

    //    myClient.Send(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_POSITION, m);

    //}

    public void ReceiveMoveArmWithFingers(NetworkMessage message)
    {
        MoveArmPositionWithFingersMessage m = message.ReadMessage<MoveArmPositionWithFingersMessage>();
        KinovaAPI.MoveArmCartesianPositionWithFingers(m.rightArm, m.x, m.y, m.z, m.thetaX, m.thetaY, m.thetaZ, m.fp1, m.fp2, m.fp3);
    }


    public void ReceiveSetArmPosition(NetworkMessage message) {
        SetArmPositionMessage m = message.ReadMessage<SetArmPositionMessage>();
        KinovaAPI.SetArmPosition(m.rightArm, m.x, m.y, m.z, m.thetaX, m.thetaY, m.thetaZ);
        hud.armPosition.text = m.x + ", " + m.y + ", " + m.z + ", " + m.thetaX + ", " + m.thetaY + ", " + m.thetaZ;
       // Debug.Log("<color=blue>Received:</color> set arm pos mess:" + m.x + ","+m.y+","+m.z);

    }

    public void ReceiveSetFingerPosition(NetworkMessage message)
    {
        SetFingerPositionMessage m = message.ReadMessage<SetFingerPositionMessage>();
        //KinovaAPI.SetFingerPosition(m.rightArm,m.fp1,m.fp2,m.fp3);
        KinovaAPI.SetFingerPosition(m.rightArm, m.fp1, m.fp2, m.fp3);
        hud.fingerPosition.text = m.fp1 + "," + m.fp2 + "," + m.fp3;
        //Debug.Log("<color=blue>Received:</color> set arm finger mess:" + m.fp1+","+m.fp2+","+m.fp3);
    }

    public void ReceiveMoveArmUpdate(NetworkMessage message)
    {
        MoveArmUpdateMessage m = message.ReadMessage<MoveArmUpdateMessage>();
        KinovaAPI.MoveArmUpdate();
        hud.updateReceived.text = Time.time.ToString();
       //Debug.Log("<color=blue>Received:</color> Update arm pos");
    }


    public void ReceiveFreezeArmPosition(NetworkMessage message)
    {
        FreezeArmPositionMessage m = message.ReadMessage<FreezeArmPositionMessage>();
        KinovaAPI.FreezeArmPosition();
        hud.freezeReceived.text = Time.time.ToString();

       // Debug.Log("<color=blue>Received:</color> update arm positions");
    }

    public void ReceiveGetCarteisanPositions(NetworkMessage message)
    {
        float[] positions = KinovaAPI.GetCartesianPositions();
        Debug.Log("<color=blue>Received:</color> got cart positions:"
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

    public void ReceiveGetCachedCarteisanCommands(NetworkMessage message)
    {
        float[] positions = KinovaAPI.GetCachedCartesianCommands();
        Debug.Log("<color=blue>Received:</color> got cached cart commands:"
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


    //public void RecieveMoveArms_WithFingers(NetworkMessage message)
    //{
    //    MoveArmWithFingersMessage m = message.ReadMessage<MoveArmWithFingersMessage>();
    //    // Debug.LogError("Move " + ArmSide(m.rightArm) + " with MoveArmCartesianPositionWithFingers received!");
    //    KinovaAPI.MoveArmCartesianPositionWithFingers(m.rightArm, m.x, m.y, m.z, m.thetaX, m.thetaY, m.thetaZ, m.fp1, m.fp2, m.fp3);
    //    Debug.Log("receiving:" + m.x + "," + m.y + "," + m.z + "," + m.thetaX + "," + m.thetaY + "," + m.thetaZ + ", fingers:  " + m.fp1 + "," + m.fp2 + "," + m.fp3);
    //}
    //public void RecieveMoveArmCartesianPosition_MoveRelative(NetworkMessage message)
    //{
    //    //MoveArmAngularPositionMessage m = message.ReadMessage<MoveArmAngularPositionMessage>();
    //    MoveArmCartesianPosition_MoveRelativeMessage m = message.ReadMessage<MoveArmCartesianPosition_MoveRelativeMessage>();

    //    Debug.LogError("Move " + ArmSide(m.rightArm) + " with MoveArmCartesianPosition_MoveRelativeMessage received!");
    //    KinovaAPI.MoveArmCartesianPositionRelative(m.rightArm, m.X, m.Y, m.Z, m.ThetaX, m.ThetaY, m.ThetaZ);
    //}

    //public void SendMoveArmAngularVelocity(bool rightArm, float av1, float av2, float av3, float av4, float av5, float av6, float av7)
    //{
    //    if (!connectedToServer)
    //    {
    //        //Debug.LogWarning ("Not connected to server!");
    //        return;
    //    }
    //    Debug.Log("Sending move " + ArmSide(rightArm) + " arm...");
    //    MoveArmAngularVelocityMessage m = new MoveArmAngularVelocityMessage();
    //    m.rightArm = rightArm;
    //    m.av1 = av1;
    //    m.av2 = av2;
    //    m.av3 = av3;
    //    m.av4 = av4;
    //    m.av5 = av5;
    //    m.av6 = av6;
    //    m.av7 = av7;

    //    myClient.Send(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_VELOCITY, m);
    //}

    //public void RecieveMoveArmAngularVelocity(NetworkMessage message)
    //{
    //    MoveArmAngularVelocityMessage m = message.ReadMessage<MoveArmAngularVelocityMessage>();
    //    Debug.LogError("Move " + ArmSide(m.rightArm) + " with MoveArmAngularVelocity received!");
    //    KinovaAPI.MoveArmAngularVelocity(m.rightArm, m.av1, m.av2, m.av3, m.av4, m.av5, m.av6, m.av7);
		
    //}
	
	//public void ReceieveMoveArmAngularVelocityLooped(NetworkMessage message)
	//{
	//	MoveArmAngularVelocityLoopedMessage m = message.ReadMessage<MoveArmAngularVelocityLoopedMessage>();
	//	Debug.LogError("Move " + ArmSide(m.rightArm) + " with MoveArmAngularVelocityLooped received!");
	//	KinovaAPI.MoveArmAngularVelocityLooped(m.rightArm, m.iterations,  m.av1, m.av2, m.av3, m.av4, m.av5, m.av6, m.av7);
		
	//}
	

    //public void SendMoveArmAngularPosition(bool rightArm, int ap1,int ap2, int ap3, int ap4, int ap5, int ap6, int ap7)
    //{
    //    if (!connectedToServer)
    //    {
    //        //Debug.LogWarning ("Not connected to server!");
    //        return;
    //    }
    //    Debug.Log("Sending move " + ArmSide(rightArm) + "with MoveArmAngularPosition recieved!");
    //    MoveArmAngularPositionMessage m = new MoveArmAngularPositionMessage();
    //    m.rightArm = rightArm;
    //    m.ap1 = ap1;
    //    m.ap2 = ap2;
    //    m.ap3 = ap3;
    //    m.ap4 = ap4;
    //    m.ap5 = ap5;
    //    m.ap6 = ap6;
    //    m.ap7 = ap7;

    //    myClient.Send(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_POSITION, m);

    //}
 //   public void RecieveMoveArmAngularPosition(NetworkMessage message)
 //   {
 //       MoveArmAngularPositionMessage m = message.ReadMessage<MoveArmAngularPositionMessage>();
 //       Debug.LogError("Move " + ArmSide(m.rightArm) + " with MoveArmAngularPostion received!");
 //       KinovaAPI.MoveArmAngularPosition(m.rightArm, m.ap1, m.ap2, m.ap3, m.ap4, m.ap5, m.ap6, m.ap7);
 //   }
 //   // end shawn test 10.1.17
 //   public void SendMoveArm (bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ)
 // {
	//if (!connectedToServer) {
	//  //Debug.LogWarning ("Not connected to server!");
	//  return;
	//}

	//Debug.Log ("Sending move " + ArmSide(rightArm) + " arm...");
 //   MoveArmMessage m = new MoveArmMessage();
 //   m.rightArm = rightArm;
 //   m.x = x;
 //   m.y = y;
 //   m.z = z;
 //   m.thetaX = thetaX;
 //   m.thetaY = thetaY;
 //   m.thetaZ = thetaZ;

 //   myClient.Send (MyMsgTypes.MSG_MOVE_ARM, m);
 // }

 // private void ReceiveMoveArm (NetworkMessage message)
 // {
	//MoveArmMessage m = message.ReadMessage<MoveArmMessage>();
	//Debug.LogError ("Move " + ArmSide(m.rightArm) + " arm received!");
 //   KinovaAPI.MoveHand(m.rightArm, m.x, m.y, m.z, m.thetaX, m.thetaY, m.thetaZ);
 //   Debug.LogError(m.rightArm + " " + m.x + " " + m.y + " " + m.z + " "  + m.thetaX + " " + m.thetaY + " " + m.thetaZ);
 // }

 // public void SendMoveArmNoThetaY (bool rightArm, float x, float y, float z, float thetaX, float thetaZ)
 // {
	//if (!connectedToServer) {
	//  //Debug.LogWarning ("Not connected to server!");
	//  return;
	//}

	//Debug.Log ("Sending move " + ArmSide(rightArm) + " arm no theta y...");
	//MoveArmNoThetaYMessage m = new MoveArmNoThetaYMessage();
 //   m.rightArm = rightArm;
 //   m.x = x;
 //   m.y = y;
 //   m.z = z;
 //   m.thetaX = thetaX;
 //   m.thetaZ = thetaZ;

 //   myClient.Send (MyMsgTypes.MSG_MOVE_ARM_NO_THETAY, m);
 // }

 // private void ReceiveMoveArmNoThetaY (NetworkMessage message)
 // {
	//MoveArmNoThetaYMessage m = message.ReadMessage<MoveArmNoThetaYMessage>();
	//Debug.Log ("Move " + ArmSide(m.rightArm) + " arm received!");
 //   KinovaAPI.MoveHandNoThetaY(m.rightArm, m.x, m.y, m.z, m.thetaX, m.thetaZ);
 // }

 // public void SendMoveArmHome (bool rightArm)
 // {
	//if (!connectedToServer) {
	// // Debug.LogWarning ("Not connected to server!");
	//  return;
	//}

	//Debug.Log ("Sending move " + ArmSide (rightArm) + " arm home...");
	//MoveArmHomeMessage m = new MoveArmHomeMessage();
 //   m.rightArm = rightArm;

 //   myClient.Send (MyMsgTypes.MSG_MOVE_ARM_HOME, m);
 // }

 // private void ReceiveMoveArmHome (NetworkMessage message)
 // {
	//MoveArmHomeMessage m = message.ReadMessage<MoveArmHomeMessage> ();
	//Debug.Log ("Stop " + ArmSide (m.rightArm) + " arm received!");
 //   KinovaAPI.MoveArmHome(m.rightArm);
 // }

  //public void SendStopArm (bool rightArm, bool suppressLog)
  //{

  //      if (!connectedToServer) {
  //       // Debug.LogWarning ("Not connected to server!");
  //        return;
  //      }

  //  if (!suppressLog) {
  //      Debug.Log ("Sending stop " + ArmSide (rightArm) + " arm...");
  //  }
  //  StopArmMessage m = new StopArmMessage();
  //  m.rightArm = rightArm;
  //  m.suppressLog = suppressLog;

  //  myClient.Send (MyMsgTypes.MSG_STOP_ARM, m);
  //}

  private void ReceiveStopArm (NetworkMessage message)
  {
	    StopArmMessage m = message.ReadMessage<StopArmMessage> ();
        
        // Debug.Log("<color=blue>Received:</color><color=red> stop arm at </color>" + Time.time);
        KinovaAPI.StopArm(m.rightArm);
        hud.stopReceived.text = Time.time.ToString();


    }

    //public void SendMoveFingers (float []FingerData)
    // {
    //if (!connectedToServer) {
    // // Debug.LogWarning ("Not connected to server!");
    //  return;
    //}

    //   Debug.Log ("Sending move to hands");
    //   MoveFingersMessage m = new MoveFingersMessage();
    //m.gloveData = FingerData;

    //   myClient.Send (MyMsgTypes.MSG_MOVE_FINGERS, m);
    // }

    //private void ReceiveMoveFingers (NetworkMessage message)
    //{
    // Debug.Log("RecieveMoveFingers() Happening");
    // MoveFingersMessage m = message.ReadMessage<MoveFingersMessage>();

    // for(int i = 0; i < 10; i++){
    //  try
    //  {
    //	  if (m.gloveData[i].Equals(null))
    //	  {
    //		  Debug.Log("null value from hands");

    //	  }
    //	  else
    //	  {
    //		  try
    //		  {
    //			  if (m.gloveData[i] < 0.0f)
    //			  {
    //				  m.gloveData[i] = m.gloveData[i] * -1.0f;
    //			  }


    //			  if (m.gloveData[i] * 2 > 90)
    //			  {
    //				  m.gloveData[i] = 95;
    //			  }
    //			  else if (m.gloveData[i] * 2 < 20)
    //			  {
    //				  m.gloveData[i] = 5;
    //			  }

    //		  }
    //		  catch (Exception b)
    //		  {

    //			  Debug.Log("exception " + b);
    //		  }

    //	  }
    //  }
    //  catch(Exception e)
    //  {
    //	  Debug.Log("exception " + e);

    //  }

    // }
    // string temp = "F0P" + ((int) m.gloveData[4]).ToString() + "\n" + "F1P" + ((int) m.gloveData[3]).ToString() + "\n" +
    //                                                   "F2P" + ((int) m.gloveData[2]).ToString() + "\n" + "F3P" + ((int) m.gloveData[1]).ToString();

    // SampleUserPolling_ReadWrite.SetLeft(temp);
    // Debug.Log("=============String recieved and sent to hands is" + temp);

    // temp = "F0P" + ((int) m.gloveData[5]).ToString() + "\n" + "F1P" + ((int) m.gloveData[6]).ToString() + "\n" +
    //        "F2P" + ((int) m.gloveData[7]).ToString() + "\n" + "F3P" + ((int) m.gloveData[8]).ToString();

    // SampleUserPolling_ReadWrite.SetRight(temp);
    // Debug.Log("=============String recieved and sent to hands is" + temp);



    //}



    private string ArmSide (bool rightArm)
  {
	return rightArm ? "right" : "left";
  }


 // public void SendMoveToHands (int ringFinger)
 // {
	//if (!connectedToServer) {
	//	//Debug.LogWarning ("Not connected to server!");
	//	return;
	//}

	//Debug.Log ("ring finger data sent ");
	//ReceiveMoveToHandsMessage m = new ReceiveMoveToHandsMessage();
	//m.ring = ringFinger;

	//myClient.Send (MyMsgTypes.MSG_MOVE_FINGERS, m);
 // }

 // private void RecieveMoveToHands (NetworkMessage message)
 // {
	//ReceiveMoveToHandsMessage m = message.ReadMessage<ReceiveMoveToHandsMessage> ();
	//Debug.Log ("ring finger data recieved");
 // } 

}
		