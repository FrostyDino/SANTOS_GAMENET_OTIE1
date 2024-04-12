using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using Photon.Pun.Demo.Cockpit;

public class BattleRoyaleController : MonoBehaviourPunCallbacks
{
    public enum RaiseEventsCode
    {
        WhoDiedCode
    }
    private int FinishOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    void Start()
    {
        if(PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            FinishOrder = PhotonNetwork.CurrentRoom.PlayerCount + 1;
        }
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoDiedCode)
        {
            object[] data = (object[]) photonEvent.CustomData;

            string nicknameOfFinishedPlayer = (string)data[0];
            FinishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nicknameOfFinishedPlayer + " " + FinishOrder);

            GameObject orderTMPro = DeathRaceManager.instance.finisherTextTMPro[FinishOrder];
            orderTMPro.SetActive(true);

            if (viewId == photonView.ViewID)
            {
                orderTMPro.GetComponent<TextMeshProUGUI>().text = FinishOrder + " " + nicknameOfFinishedPlayer;
                orderTMPro.GetComponent<TextMeshProUGUI>().color = Color.white;

            }
            else
            {
                orderTMPro.GetComponent<TextMeshProUGUI>().text = FinishOrder + " " + nicknameOfFinishedPlayer + "(YOU)";
                orderTMPro.GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            CheckRemainingPlayers();
        }
    }

    public void OnPlayerDeath()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().isControlEnabled = false;
        GetComponent<PlayerShooting>().isControlEnabled = false;

        FinishOrder--;
        DeathRaceManager.instance.remainingPlayers--;

        string nickname = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        object[] data = new object[] {nickname, FinishOrder, viewId};

        RaiseEventOptions raisedEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false 
        };

        PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.WhoDiedCode, data, raisedEventOptions, sendOption);
    }

    private void CheckRemainingPlayers()
    {
        if (DeathRaceManager.instance.remainingPlayers == 1 && GetComponent<VehicleMovement>().isControlEnabled)
        {
            OnPlayerDeath();
        }
    }
}
