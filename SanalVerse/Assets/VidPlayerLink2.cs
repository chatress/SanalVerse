using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;

public class VidPlayerLink2 : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<string> videoUrls; // Video URL'lerini burada tan�mlayaca��z
    private VideoPlayer videoPlayer;
    private int currentVideoIndex = 0; // �u anda oynayan video indeksi
    private bool isPlaying = false;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer)
        {
            videoPlayer.playOnAwake = false;
            SetVideo(currentVideoIndex);
        }
    }

    // Videoyu ba�lat�r ve Photon RPC ile herkese g�nderir
    public void PlayVideo()
    {
        if (!videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;
            photonView.RPC("SyncVideoState", RpcTarget.All, currentVideoIndex, true);
        }
    }

    // Videoyu duraklat�r ve Photon RPC ile herkese g�nderir
    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            isPlaying = false;
            photonView.RPC("SyncVideoState", RpcTarget.All, currentVideoIndex, false);
        }
    }

    // Video URL'ini de�i�tirir ve herkese bildirir
    public void ChangeVideo(int videoIndex)
    {
        if (videoIndex < videoUrls.Count)
        {
            currentVideoIndex = videoIndex;
            SetVideo(currentVideoIndex);
            photonView.RPC("SyncVideoState", RpcTarget.All, currentVideoIndex, isPlaying);
        }
    }

    // Videonun URL'ini ayarlayan ve oynatan metod
    private void SetVideo(int videoIndex)
    {
        videoPlayer.url = videoUrls[videoIndex];
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        if (isPlaying)
        {
            videoPlayer.Play();
        }
    }

    // Photon �zerinden gelen videonun durumu ve indeksi senkronize edilir
    [PunRPC]
    void SyncVideoState(int videoIndex, bool playState)
    {
        if (videoIndex < videoUrls.Count)
        {
            currentVideoIndex = videoIndex;
            isPlaying = playState;
            SetVideo(videoIndex);
        }
    }
}
