using UnityEngine;
using System;
using System.Diagnostics;
using System.Web;
using Random = System.Random;

public class SampleUserPolling_ReadWrite : MonoBehaviour
{
    public SerialController serialControllerL;
    public SerialController serialControllerR;
    static Stopwatch stopWatch = new Stopwatch();
    private TimeSpan ts;
    private long i;

    public static string left = "F0P20\nF1P20\nF2P20\nF3P20";
    public static string right = "F0P20\nF1P20\nF2P20\nF3P20";
    private string lastL = "";
    private string lastR = "";   
    
    void Start()
    {
        serialControllerL = GameObject.Find("SerialControllerL").GetComponent<SerialController>();
        serialControllerR = GameObject.Find("SerialControllerR").GetComponent<SerialController>();
        stopWatch.Start();
    }

    void Update()
    {
        {
            if (left != lastL)
            {
                serialControllerL.SendSerialMessage(left);
            }
            if (right != lastR)
            {
                serialControllerR.SendSerialMessage(right);
            }
            lastL = left;
            lastR = right;
        }

        if (i % 50 == 0)
        {
            ts = stopWatch.Elapsed;
           // PrintElapsedTime(ts);
        }
        i++;
    }

    public static string PrintElapsedTime(TimeSpan ts)
    {
        /*====================================================================================\ 
        | * Preconditions - Needs a non-null TimeSpan value                                   |
        | * Postconditions - prints a formatted string to stdout, and returns the same string.|
        \====================================================================================*/
        
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        UnityEngine.Debug.Log("RunTime: " + elapsedTime);
        return elapsedTime;
    }// PrintTime
    
    public static void SetLeft(string cmd)
    {
        left = cmd;
    }
    
    public static void SetRight(string cmd)
    {
        right = cmd;
    }
}