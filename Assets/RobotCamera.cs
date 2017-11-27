using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotCamera : MonoBehaviour {



    WebCamDevice webcam;

    //Texture2D webcamFrame;
    //string serializedWebcamFrame;
    //void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    //{
    //    int health = 0;
    //    if (stream.isWriting)
    //    {
    //        health = currentHealth;
    //        stream.Serialize(ref health);
    //    }
    //    else
    //    {
    //        stream.Serialize(ref health);
    //        currentHealth = health;
    //    }
    //}

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{

        //    FindWebcam();
        //}

        //WebCamTexture.devices[0].

        // get pixel 32
        // serialize into string
        // send over network
        // deserialize and render

    }
    void FindWebcam() { 
        Debug.Log("webcam devices len:" + WebCamTexture.devices.Length);
        for (int i = 0; i < WebCamTexture.devices.Length; i++) {
            webcam = WebCamTexture.devices[i];
            Debug.Log("cam:" + WebCamTexture.devices[i].name);
            break;
        }
	}
	

}
