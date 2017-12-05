using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ClientBroadcaster : MonoBehaviour
{

    
    // Stream or broadcast from one person to many others?
    public bool oneToManyBroadcast;
    public int numberReceivers = 2;

    private HostData[] hostData;
    private NetworkView clientView;
    
    // Use this for initialization
    void Start()
    {

    
        MasterServer.ClearHostList();
        MasterServer.RequestHostList("ClientBroadcaster");
        hostData = MasterServer.PollHostList();
        

        
		clientView = gameObject.AddComponent< NetworkView >();
		clientView.stateSynchronization = NetworkStateSynchronization.Off;
		clientView.group = 6;
    }

    IEnumerator DelayedConnection()
    {
        yield return new WaitForSeconds(2.0f);

        string connectionResult = "";

        if (hostData.Length > 0)
        {
            connectionResult = "" + Network.Connect(hostData[0]);
            Debug.Log("connected:" + connectionResult);
        }
        
    }

    public void JoinServer()
    {
        StartCoroutine(JoinServerE());
    }
    IEnumerator JoinServerE() {
        yield return new WaitForSeconds(2.0f);
        // called by MyNetworkManager.Server which needs to broadcast info to the MyNetworkManager.Client
        hostData = MasterServer.PollHostList();
        string connectionResult = "" + Network.Connect(hostData[0] );
        Debug.Log("client broadcaster connected:" + connectionResult);
        
    }
    public void StartServer()
    {
        //MyNetworkManager.client will call this function to receive info from mynetworkmanager.server
           Network.InitializeServer(1, 2301, true);

            MasterServer.RegisterHost("ClientBroadcaster", "Test");
    }

    public void SendPosToClient(string p) {
        Debug.Log("send to client.");
        clientView.RPC("GetCurrentArmCartesianPosition", RPCMode.All, p);
    }

    [RPC]
    public void GetCurrentArmCartesianPosition(string p) { }


}
