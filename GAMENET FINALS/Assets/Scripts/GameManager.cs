using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public Transform[] SpawnPositions;
    public GameObject[] PlayerPrefabs;
    public static GameManager instance;

    public TextMeshProUGUI CountDownText;
    public TextMeshProUGUI KillfeedText;

    public int RemaningPlayers = 0;
    public GameObject[] FinishOrderText;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null) 
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            object playerSelectionNumber;

            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber))
            {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

                Vector3 instantiatePosition = SpawnPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(PlayerPrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }
        }

        foreach (GameObject go in FinishOrderText)
        {
            go.SetActive(false);
        }

        RemaningPlayers = PhotonNetwork.CurrentRoom.PlayerCount;

    }

    void Update()
    {
        
    }

    public IEnumerator UpdateKillfeed(Player Killer, Player Victim)
    {
        KillfeedText.text = Killer.NickName + " Killed " + Victim.NickName;
        yield return new WaitForSeconds(1.0f);
        KillfeedText.text = "";
    }
}
