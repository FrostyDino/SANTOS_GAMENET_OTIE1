using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public GameObject killListPrefab;
    public GameObject[] Spawnpoints;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            instance = this;
        }
    }
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady) 
        {
            int randomSpawn = Random.Range(0, Spawnpoints.Length);
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(Spawnpoints[randomSpawn].transform.position.x, 0, Spawnpoints[randomSpawn].transform.position.z), Quaternion.identity);
        }
    }

    public void KillfeedUpdate(Player killer, Player victim)
    {
        GameObject KillPanel = GameObject.Find("KillfeedPanel");
        GameObject listItem = Instantiate(killListPrefab);
        listItem.transform.SetParent(KillPanel.transform);
        listItem.transform.localScale = Vector3.one;

        KillPanel.transform.Find("KillListText").GetComponent<TextMeshProUGUI>().text = killer.NickName + " Killed " + victim.NickName;

        Destroy(listItem, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
