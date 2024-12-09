using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class VidPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] string videoFileName;

    private VideoPlayer videoPlayer;
    private bool isPlaying = false; // Ekledi�imiz yeni de�i�ken

    // Start is called before the first frame update
    void Start()
    {
        // PauseVideo();
    }

    public void PlayVideo()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log(videoPath);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
            isPlaying = true; // Videonun oynay�p oynamad���n� saklamak i�in de�i�keni g�ncelledik
        }
        else
        {
            Debug.LogError("VideoPlayer component not found!");
        }
        photonView.RPC("SyncVideoState", RpcTarget.Others, isPlaying); // RPC i�levini �a��rd�k
    }

    public void PauseVideo()
    {
        if (videoPlayer)
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
                isPlaying = false; // Videonun oynay�p oynamad���n� saklamak i�in de�i�keni g�ncelledik
            }
            else
            {
                videoPlayer.Play();
                isPlaying = true; // Videonun oynay�p oynamad���n� saklamak i�in de�i�keni g�ncelledik
            }
        }
    }

    // RPC i�levimiz
    [PunRPC]
    void SyncVideoState(bool state)
    {
        if (state == true)
        {
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Pause();
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
