using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class StartStop : MonoBehaviourPunCallbacks, IPunObservable
{
    private VideoPlayer player;
    public Button button;
    public Sprite startSprite;
    public Sprite stopSprite;

    private bool isPlaying = false; // Ekledi�imiz yeni de�i�ken

    void Start()
    {
        player = GetComponent<VideoPlayer>();
    }

    void Update()
    {

    }

    public void ChangeStartStop()
    {
        if (player.isPlaying == false)
        {
            player.Play();
            button.image.sprite = stopSprite;
            isPlaying = true; // Videonun oynay�p oynamad���n� saklamak i�in de�i�keni g�ncelledik
        }
        else
        {
            player.Pause();
            button.image.sprite = startSprite;
            isPlaying = false; // Videonun oynay�p oynamad���n� saklamak i�in de�i�keni g�ncelledik
        }

        photonView.RPC("SyncVideoState", RpcTarget.Others, isPlaying); // RPC i�levini �a��rd�k
    }

    // RPC i�levimiz
    [PunRPC]
    void SyncVideoState(bool state)
    {
        if (state == true)
        {
            player.Play();
            button.image.sprite = stopSprite;
        }
        else
        {
            player.Pause();
            button.image.sprite = startSprite;
        }
    }

    // PhotonView'�n durumunu senkronize etmek i�in gerekli olan OnPhotonSerializeView i�levi
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isPlaying);
        }
        else
        {
            isPlaying = (bool)stream.ReceiveNext();
        }
    }
}
