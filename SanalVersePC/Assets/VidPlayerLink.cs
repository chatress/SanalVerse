using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class VidPlayerLink : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] string videoUrl = "https://drive.google.com/uc?id=1dt1IAojbZ_buggbycV9Ei1dSdTBF5Jvs";

    private VideoPlayer videoPlayer;
    private bool isPlaying = false; // Ekledi�imiz yeni de�i�ken

    // Start is called before the first frame update
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer)
        {
            videoPlayer.url = videoUrl;
            videoPlayer.playOnAwake = false;
            videoPlayer.Prepare();

            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
        else
        {
            Debug.LogError("VideoPlayer component not found!");
        }
        photonView.RPC("SyncVideoState", RpcTarget.Others, isPlaying); // RPC i�levini �a��rd�k
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        // Video is prepared, but not playing yet
    }

    public void PlayVideo()
    {
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            photonView.RPC("SyncVideoState", RpcTarget.All, true);
        }
    }

    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            photonView.RPC("SyncVideoState", RpcTarget.All, false);
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

    [PunRPC]
    void SyncVideoTime(float startTime)
    {
        videoPlayer.time = startTime;
        videoPlayer.Play();
    }

    // PhotonView'�n durumunu senkronize etmek i�in gerekli olan OnPhotonSerializeView i�levi
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(videoPlayer.time);
            stream.SendNext(videoPlayer.isPlaying);
        }
        else
        {
            float startTime = (float)stream.ReceiveNext();
            bool isPlaying = (bool)stream.ReceiveNext();
            photonView.RPC("SyncVideoTime", RpcTarget.All, startTime);
            if (isPlaying)
            {
                videoPlayer.Play();
            }
        }
    }
}
