using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    public Camera tpsCamera;
    public TextMeshProUGUI PlayerName;
    public GameObject PlayerUiPrefab;

    void Start()
    {
        
        GetComponent<PlayerMovement>().enabled = photonView.IsMine;
        GetComponent<PlayerShooting>().enabled = photonView.IsMine;
        tpsCamera.enabled = photonView.IsMine;
        PlayerName.text = photonView.Owner.NickName;

        if (photonView.IsMine) 
        {
            Instantiate(PlayerUiPrefab);
        }
    }


    void Update()
    {
        
    }
}
