using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
public class CountdownManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI CountdownText;
    public float TimeToStart = 1.0f ;
    void Start()
    {
        CountdownText = GameManager.instance.CountDownText;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (TimeToStart > 0)
            {
                TimeToStart -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, TimeToStart);
            }
            else if (TimeToStart < 0)
            {
                photonView.RPC("StartBattle", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float Time) 
    {
        if (Time > 0)
        {
            CountdownText.text = Time.ToString("F1");
        }
        else 
        {
            CountdownText.text = "";
        }
    }

    [PunRPC]
    public void StartBattle() 
    {
        GetComponent<PlayerMovement>().IsControlEnabled = true;
        GetComponent<PlayerShooting>().IsControlEnabled = true;
        this.enabled = false;
    }
}
