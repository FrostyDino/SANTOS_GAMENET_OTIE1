using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;

public class ShootingScript : MonoBehaviourPunCallbacks
{

    public Camera cameral;
    public GameObject hitEffectPrefab;

    [Header("HP related stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;
    private int KillCount;
    private int RequiredKillToWin = 3;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();
        RequiredKillToWin -= 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fire()
    {
        Debug.Log("shooting");
        RaycastHit hit;
        Ray ray = cameral.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);
            }
        }

    }

    [PunRPC]
    public void TakeDamage(int dmg, PhotonMessageInfo Info)
    {
        this.health -= dmg;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();
            Info.Sender.AddScore(1);
            Debug.Log(Info.Sender.NickName + " killed " + Info.photonView.Owner.NickName);
            GameManager.instance.KillfeedUpdate(Info.Sender, Info.photonView.Owner);

            if (Info.Sender.GetScore() == RequiredKillToWin) 
            {
                photonView.RPC("GameOver", RpcTarget.AllBuffered);
            }

            
            
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.5f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("IsDead", true);
            StartCoroutine(RespawnCountdown());
        }

    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<TextMeshProUGUI>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("IsDead", false);
        respawnText.GetComponent<TextMeshProUGUI>().text = "";

        int randomPointX = Random.Range(-20, 20);

        int randomPointZ = Random.Range(-20, 20);

        this.transform.position = new Vector3(randomPointX, 0, randomPointZ);
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }

    [PunRPC]
    public void GameOver(PhotonMessageInfo info) 
    {
        GameObject GameOver = GameObject.Find("RespawnText");
        Time.timeScale = 0;
        transform.GetComponent<PlayerMovementController>().enabled = false;

        if (info.photonView.Owner.GetScore() >= RequiredKillToWin)
        {
            GameOver.GetComponent<TextMeshProUGUI>().text = info.Sender.NickName + " Wins";
        }
    }
}
