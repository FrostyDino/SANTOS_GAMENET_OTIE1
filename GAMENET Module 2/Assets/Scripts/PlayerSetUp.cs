using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSetUp : MonoBehaviourPunCallbacks
{
    public GameObject fpsModel;
    public GameObject nonFPSModel;
    public GameObject playerUIPrefab;
    public PlayerMovementController playerMovementController;
    public Camera fpsCamera;
    public TextMeshProUGUI PlayerName;

    private Animator animator;
    public Avatar fpsAvatar;
    public Avatar nonFPSAvatar;

    private ShootingScript shooting;
    void Start()
    {
        playerMovementController = this.GetComponent<PlayerMovementController>();
        animator = this.GetComponent<Animator>();
        fpsModel.SetActive(photonView.IsMine);
        nonFPSModel.SetActive(!photonView.IsMine);
        animator.SetBool("isLocal", photonView.IsMine);

        animator.avatar = photonView.IsMine ? fpsAvatar : nonFPSAvatar;

        shooting = this.GetComponent<ShootingScript>();

        if (photonView.IsMine)
        {
            GameObject playerUI = Instantiate(playerUIPrefab);
            playerMovementController.fixedTouchField = playerUI.transform.Find("Rotation Touch Field").GetComponent<FixedTouchField>();
            playerMovementController.joystick = playerUI.transform.Find("Fixed Joystick").GetComponent<Joystick>();

            playerUI.transform.Find("FireButton").GetComponent<Button>().onClick.AddListener(() => shooting.Fire());


        }
        else
        {
            playerMovementController.enabled = false;
            GetComponent<MovementController>().enabled = false;
            fpsCamera.enabled = false;
        }

        this.photonView.RPC("SetPlayerName", RpcTarget.AllBuffered);
    }

    // Update is called once per frame
    void Update()
    {

    }

    [PunRPC]
    public void SetPlayerName(PhotonMessageInfo info) 
    {
        PlayerName.text = info.photonView.Owner.NickName;
    }
}
