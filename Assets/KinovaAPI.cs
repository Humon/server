using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using System;
using System.Linq;
using System.Text;
using System.Threading;

public class KinovaAPI : MonoBehaviour
{

  // TODO: Give external functions prefix to easily identify them as such (e.g., extern_InitRobot)
  //https://stackoverflow.com/questions/7276389/confused-over-dll-entry-points-entry-point-not-found-exception
  [DllImport ("ARM_base_32", EntryPoint = "InitRobot")]
  private static extern int _InitRobot ();

  [DllImport ("ARM_base_32", EntryPoint = "MoveArmHome")]
  private static extern int _MoveArmHome (bool rightArm);

  [DllImport ("ARM_base_32", EntryPoint = "MoveHand")]
  private static extern int _MoveHand (bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ);

  [DllImport ("ARM_base_32", EntryPoint = "MoveHandNoThetaY")]
  private static extern int _MoveHandNoThetaY (bool rightArm, float x, float y, float z, float thetaX, float thetaZ);

  [DllImport ("ARM_base_32", EntryPoint = "MoveFingers")]
  private static extern int _MoveFingers (bool rightArm, bool pinky, bool ring, bool middle, bool index, bool thumb);

  [DllImport ("ARM_base_32", EntryPoint = "CloseDevice")]
  private static extern int _CloseDevice (bool rightArm);

  [DllImport ("ARM_base_32", EntryPoint = "StopArm")]
  private static extern int _StopArm (bool rightArm);
	//new

  [DllImport ("ARM_base_32", EntryPoint = "MoveArmAngularVelocity")]
  private static extern int _MoveArmAngularVelocity (bool rightArm, float av1=0, float av2=0, float av3=0, float av4=0, float av5=0, float av6=0, float av7=0);
	
  [DllImport ("ARM_base_32", EntryPoint = "MoveArmAngularVelocityLooped")]
  private static extern int _MoveArmAngularVelocityLooped (bool rightArm, int iterations, float av1=0, float av2=0, float av3=0, float av4=0, float av5=0, float av6=0, float av7=0);

  [DllImport ("ARM_base_32", EntryPoint = "MoveArmAngularPosition")]
  private static extern int _MoveArmAngularPosition (bool rightArm, int ap1=0, int ap2=0, int ap3=0, int ap4=0, int ap5=0, int ap6=0, int ap7=0);

 [DllImport("ARM_base_32", EntryPoint = "CartesianPosition_MoveRelative")]
  private static extern int _CartesianPosition_MoveRelative(bool rightArm, float X, float Y, float Z, float ThetaX, float ThetaY, float ThetaZ);

 [DllImport("ARM_base_32", EntryPoint = "MoveArmCartesianPositionWithFingers")]
 private static extern int _MoveArmCartesianPositionWithFingers(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ, float fp1, float fp2, float fp3);
    //end new
    private static bool initSuccessful = false;

    // [DllImport("ARM_base_32", EntryPoint = "SetArmPosition")]

     [DllImport("ARM_base_32", EntryPoint = "SetArmPosition")]
    private static extern int _SetArmPosition(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ);

    [DllImport("ARM_base_32", EntryPoint = "SetFingerPosition")]
    private static extern int _SetFingerPosition(bool rightArm, float fp1, float fp2, float fp3);


    [DllImport("ARM_base_32", EntryPoint = "MoveArmUpdate")]
    private static extern int _MoveArmUpdate();


    [DllImport("ARM_base_32", EntryPoint = "FreezeArmPosition")]
    private static extern int _FreezeArmPosition();

    [DllImport("ARM_base_32", EntryPoint = "GetCartesianPositions")]
    public static extern IntPtr _GetCartesianPositions();

    [DllImport("ARM_base_32", EntryPoint = "GetCartesianCommands")]
    public static extern IntPtr _GetCartesianCommands();

    [DllImport("ARM_base_32", EntryPoint = "GetCachedCarteisanCommands")]
    public static extern IntPtr _GetCachedCarteisanCommands();


    [DllImport("ARM_base_32", EntryPoint = "ReleaseMemory")]
    public static extern int ReleaseMemory(IntPtr ptr);


    //[DllImport("C:\\Devs\\C++\\Projects\\Interop\\InteropTestApp\\Debug\\InteropTestApp.dll")]
    //public static extern IntPtr test();

    //[DllImport("C:\\Devs\\C++\\Projects\\Interop\\InteropTestApp\\Debug\\InteropTestApp.dll", CallingConvention = CallingConvention.Cdecl)]
    //public static extern int ReleaseMemory(IntPtr ptr);

    public static float[] GetCartesianPositions()
    {
        IntPtr ptr = _GetCartesianPositions();
        float[] result = new float[8];
        Marshal.Copy(ptr, result, 0, 8);

        ReleaseMemory(ptr);
        return result;
        //Debug.Log("ptr:" + ptr[0] + "," + ptr[1]);
    }

    public static float[] GetCartesianCommands()
    {
        IntPtr ptr = _GetCartesianCommands();
        float[] result = new float[8];
        Marshal.Copy(ptr, result, 0, 8);

        ReleaseMemory(ptr);
        return result;
        //Debug.Log("ptr:" + ptr[0] + "," + ptr[1]);
    }

    public static float[] GetCachedCartesianCommands()
    {
        IntPtr ptr = _GetCachedCarteisanCommands();
        float[] result = new float[8];
        Marshal.Copy(ptr, result, 0, 8);

        ReleaseMemory(ptr);
        return result;
        //Debug.Log("ptr:" + ptr[0] + "," + ptr[1]);
    }


    public class Position
  {
	public float X { get; }

	public float Y { get; }

	public float Z { get; }

	public float ThetaX { get; }

	public float ThetaY { get; }

	public float ThetaZ { get; }

	public Position (float x, float y, float z, float thetaX, float thetaY, float thetaZ)
	{
	  // meters
	  X = x;
	  Y = y;
	  Z = z;

	  // radians
	  ThetaX = thetaX;
	  ThetaY = thetaY;
	  ThetaZ = thetaZ; // wrist rotation
	}
  }

  // Only path that is non-blocking at the moment:
  // RaiseTheRoof <--> Home Position <--> Scooping
  // Note that all these positions are for the left arm; for right
  // arm positions, negate x and compliment thetaX.

  // HOME (Cartesian Position for Joystick Home)
  // note: since Joystick home positions the arm by actuator, this
  // home position will not exactly match Joystick home

  public static Position HomePosition =
	new Position (0.29f, -0.26f, 0.29f, 1.5924f, -1.1792f, 0f);

  // Arm raised up
  public static Position RaiseTheRoof =
	new Position (0.0f, -0.60f, 0.33f, 1.5665f, -0.4711f, 0f);

  // Arm ready to scoop ice cream
  public static Position Scooping =
	new Position (-0.15f, 0.41f, 0.57f, -1.6554f, -0.6633f, 0f);

  // Arm stretched out from the shoulder
  public static Position StretchOut =
	new Position (-0.11f, -0.25f, 0.75f, 1.5956f, 0.0318f, 0f);

  // Arm hanging to the side
  public static Position RestingPosition =
	new Position (0.04f, 0.67f, 0.29f, -1.57f, -0.32f, 0f);

  // Arm flexing biceps
  public static Position FlexBiceps =
	new Position (-0.08f, -0.46f, 0.22f, 1.37f, -0.26f, 0f);

  public static void InitRobot ()
  {
    Debug.Log ("trying to init robot...");
	if (initSuccessful) {
	  Debug.Log ("Already initialized");
	  return;
	}
	int errorCode = _InitRobot ();
	switch (errorCode) {
	case 0:
	  Debug.Log ("Kinova robotic arm loaded and device found");
	  initSuccessful = true;
	  break;
	case -1:
	  Debug.LogError ("Robot APIs troubles");
	  break;
	case -2:
	  Debug.LogError ("Robot - no device found");
	  break;
	case -3:
	  Debug.LogError ("Robot - more devices found - not sure which to use");
	  break;
	case -10:
	  Debug.LogError ("Robot APIs troubles: InitAPI");
	  break;
	case -11:
	  Debug.LogError ("Robot APIs troubles: CloseAPI");
	  break;
	case -12:
	  Debug.LogError ("Robot APIs troubles: SendBasicTrajectory");
	  break;
	case -13:
	  Debug.LogError ("Robot APIs troubles: GetDevices");
	  break;
	case -14:
	  Debug.LogError ("Robot APIs troubles: SetActiveDevice");
	  break;
	case -15:
	  Debug.LogError ("Robot APIs troubles: GetAngularCommand");
	  break;
	case -16:
	  Debug.LogError ("Robot APIs troubles: MoveHome");
	  break;
	case -17:
	  Debug.LogError ("Robot APIs troubles: InitFingers");
	  break;
	case -18:
	  Debug.LogError ("Robot APIs troubles: StartForceControl");
	  break;
    case -19:
	  Debug.LogError ("Robot APIs troubles: MoveArmAngularVelocity");
	  break;
	case -20:
	  Debug.LogError ("Robot APIs troubles: MoveArmAngularPosition");
	  break;
	case -123:
	  Debug.LogError ("Robot APIs troubles: Command Layer Handle");
	  break;
	default:
	  Debug.LogError ("Robot - unknown error from initialization");
	  break;
	}
  }
    //new

    public static int SetArmPosition(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ)
    {
        if (initSuccessful)
        {
            _SetArmPosition(rightArm, x, y, z, thetaX, thetaY, thetaZ);
        }

        return 0;
    }

    public static int FreezeArmPosition() {
        if (initSuccessful)
        {
            _FreezeArmPosition();
        }
        return 0;

    }

    public static int SetFingerPosition(bool rightArm, float fp1, float fp2, float fp3)
    {
        if (initSuccessful)
        {
            _SetFingerPosition(rightArm, fp1, fp2, fp3);
        }

        return 0;
    }

    public static int MoveArmUpdate() {
        if (initSuccessful)
        {
            _MoveArmUpdate();
        }

        return 0;
    }


    public static int MoveArmCartesianPositionWithFingers(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ, float fp1, float fp2, float fp3)
    {
        if (initSuccessful)
        {
            _MoveArmCartesianPositionWithFingers(rightArm, x, y, z, thetaX, thetaY, thetaZ, fp1, fp2, fp3);
        }
        
        return 0;
    }
    public static int MoveArmCartesianPositionRelative(bool rightArm, float X, float Y, float Z, float ThetaX, float ThetaY, float ThetaZ)
    {
        if (initSuccessful)
        {
            _CartesianPosition_MoveRelative(rightArm, X, Y, Z, ThetaX, ThetaY, ThetaZ);
        }
        return 0;
    }

    public static int MoveArmAngularVelocity (bool rightArm, float av1, float av2, float av3, float av4, float av5, float av6, float av7)
  {
        if(initSuccessful) 
        {
          var imdifficult = _MoveArmAngularVelocity (rightArm,av1,av2,av3,av4,av5,av6,av7);       
	      Debug.LogError("I'm pretty fucking difficult, no really: " + imdifficult);
        }
        return 0;
  }
	
	public static int MoveArmAngularVelocityLooped (bool rightArm, int iterations, float av1, float av2, float av3, float av4, float av5, float av6, float av7)
	{
		if(initSuccessful) 
		{
			var imdifficult = _MoveArmAngularVelocityLooped (rightArm, iterations, av1, av2, av3, av4, av5, av6, av7);       
			Debug.LogError("MovedArmAmgularVelocityLooped called with " + imdifficult);
		}
		return 0;
	}
    public static int MoveArmAngularPosition (bool rightArm, int ap1=0, int ap2=0, int ap3=0, int ap4=0, int ap5=0, int ap6=0, int ap7=0)
  {
        if(initSuccessful) 
        {
           _MoveArmAngularPosition (rightArm,ap1,ap2,ap3,ap4,ap5,ap6,ap7);                    
        }
        return 0;
  }
//end new
  public static void StopArm (bool rightArm)
  {
	if (initSuccessful) {
      _StopArm (rightArm);
	}
  }

  public static void MoveArmHome (bool rightArm)
  {
	if (initSuccessful) {
	  _MoveArmHome (rightArm);
	}
  }

  public static void MoveHand (bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ)
  {
	if (initSuccessful) {
	  _MoveHand (rightArm, x, y, z, thetaX, thetaY, thetaZ);
	}
  }

  public static void MoveHandNoThetaY (bool rightArm, float x, float y, float z, float thetaX, float thetaZ)
  {
	if (initSuccessful) {
	  _MoveHandNoThetaY (rightArm, x, y, z, thetaX, thetaZ);
	}
  }

  public static void MoveFingers (bool rightArm, bool pinky, bool ring, bool middle, bool index, bool thumb)
  {
	if (initSuccessful) {
	  _MoveFingers (rightArm, pinky, ring, middle, index, thumb);
	}
  }


  /**@brief OnApplicationQuit() is called when application closes.
   * 
   * section DESCRIPTION
   * 
   * OnApplicationQuit(): Is called on all game objects before the 
   * application is quit. In the editor it is called when the user 
   * stops playmode. This function is called on all game objects 
   * before the application is quit. In the editor it is called 
   * when the user stops playmode.
   */
  private void OnApplicationQuit ()
  {
	if (initSuccessful) {
	  Debug.Log("Closing Robot API...");
	  _CloseDevice (false);
	}
  }
}