using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct Msg : NetworkMessage
{
    bool x;
}

public class XnetworkManager : NetworkManager
{

    public GameObject sika;
    public GameObject bino;

    public Transform sikaTransform;
    public Transform binoTransform;

    // XGameManager gm;


    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<Msg>(OnCreateCharacter);
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Msg x = new Msg();
        conn.Send(x);
    }


    void OnCreateCharacter(NetworkConnection conn, Msg x)
    {
        if(numPlayers==0){
            Debug.Log("sika");
            GameObject player0 = (GameObject) Instantiate(sika, sikaTransform.position, sikaTransform.rotation);
            NetworkServer.AddPlayerForConnection(conn, player0);
        }
        else{
            Debug.Log("bino");
            GameObject player1 = (GameObject) Instantiate(bino, binoTransform.position, binoTransform.rotation);
            NetworkServer.AddPlayerForConnection(conn, player1);
        }
    }


}
