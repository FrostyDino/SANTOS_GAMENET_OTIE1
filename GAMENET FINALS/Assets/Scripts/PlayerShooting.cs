using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerShooting : MonoBehaviourPunCallbacks
{
    public Transform barrel;
    public GameObject HitEffect;
    public int Life = 3;

    [Header("HP Related Items")]
    public float StartHealth = 100;
    public float CurrentHealth;
    public Image HealthBar;

    public bool IsControlEnabled;
    void Start()
    {
        IsControlEnabled = false;
        CurrentHealth = StartHealth;
        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
        Life = 3;
    }

    private void LateUpdate()
    {
        if (IsControlEnabled) 
        {
            Shoot();
        }
    }

    void Shoot() 
    {
        if ( Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            RaycastHit hit;
            Ray ray = new Ray(barrel.position, barrel.forward);

            if (Physics.Raycast(ray, out hit, 200))
            {
                Debug.Log(hit.collider.gameObject.name);
                photonView.RPC("CreateHitEffect", RpcTarget.AllBuffered, hit.point);

                if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine) 
                {
                    hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
                }
                
            }
        }
    }

    void OnDeath() 
    {
        if (photonView.IsMine)
        {
            StartCoroutine(RespawnCountdown());
            Life--;
            UpdateLifeCounter();
            if (Life == 0) 
            {
                GetComponent<EndingController>().OnPlayerDeath();
            }
        }
    }

    public void UpdateLifeCounter() 
    {
        TextMeshProUGUI LifeCounterText = GameObject.Find("LifeCountdown").GetComponent<TextMeshProUGUI>();
        LifeCounterText.text = "Lives Remaing: " + Life;
    }

    IEnumerator RespawnCountdown() 
    {
        TextMeshProUGUI respawnText = GameObject.Find("RespawnCountdown").GetComponent<TextMeshProUGUI>();
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovement>().enabled = false;
            respawnText.text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }

        
        respawnText.GetComponent<TextMeshProUGUI>().text = "";

        int randomSpawnPoint = Random.Range(0, GameManager.instance.SpawnPositions.Length);
        this.transform.position = new Vector3(GameManager.instance.SpawnPositions[randomSpawnPoint].position.x, 
                                                GameManager.instance.SpawnPositions[randomSpawnPoint].position.y, 
                                                GameManager.instance.SpawnPositions[randomSpawnPoint].position.z);
     
        transform.GetComponent<PlayerMovement>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    

    [PunRPC]
    void CreateHitEffect(Vector3 position) 
    {
        GameObject HitEffectGameObject = Instantiate(HitEffect, position, Quaternion.identity);
        Destroy(HitEffectGameObject, 1.0f);
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info) 
    {
        this.CurrentHealth -= damage;
        this.HealthBar.fillAmount = CurrentHealth / StartHealth;

        if (CurrentHealth == 0) 
        {
            StartCoroutine(GameManager.instance.UpdateKillfeed(info.Sender, info.photonView.Owner));
            OnDeath();
        }
    }

    [PunRPC]
    public void RegainHealth()
    {
        CurrentHealth = 100;
        HealthBar.fillAmount = CurrentHealth / StartHealth;
    }
}
