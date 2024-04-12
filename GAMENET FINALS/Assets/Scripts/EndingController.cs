using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
public class EndingController : MonoBehaviourPunCallbacks
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
        FinishOrder = PhotonNetwork.CurrentRoom.PlayerCount + 1;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoDiedCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nicknameOfFinishedPlayer = (string)data[0];
            FinishOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nicknameOfFinishedPlayer + " " + FinishOrder);

            GameObject orderTMPro = GameManager.instance.FinishOrderText[FinishOrder];
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
        GetComponent<PlayerSetUp>().tpsCamera.transform.parent = null;
        GetComponent<PlayerMovement>().IsControlEnabled = false;
        GetComponent<PlayerShooting>().IsControlEnabled = false;
        this.gameObject.SetActive(false);

        FinishOrder--;
        GameManager.instance.RemaningPlayers--;

        string nickname = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        object[] data = new object[] { nickname, FinishOrder, viewId };

        RaiseEventOptions raisedEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoDiedCode, data, raisedEventOptions, sendOption);
    }

    private void CheckRemainingPlayers()
    {
        if (GameManager.instance.RemaningPlayers == 1 && GetComponent<PlayerMovement>().IsControlEnabled)
        {
            OnPlayerDeath();
        }
    }
}
