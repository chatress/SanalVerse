using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using UnityEngine.UI;

public class VidPlayerLink3 : MonoBehaviourPunCallbacks
{
    [SerializeField] private List<string> videoUrls; // Video URL'leri
    [SerializeField] private VideoPlayer videoPlayer; // Video Player bile�eni
    [SerializeField] private Renderer videoCubeRenderer; // VideoCube Renderer
    [SerializeField] private GameObject panel; // URL giri�i i�in panel
    [SerializeField] private List<InputField> inputFields; // URL giri�leri i�in InputField listesi

    private int currentVideoIndex = 0; // �u anda oynat�lan video indeksi
    private bool isPlaying = false; // Videonun oynatma durumu

    void Awake()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer bile�eni atanmad�!");
        }
        else
        {
            videoPlayer.playOnAwake = false;
        }

        // VideoCube Renderer'a VideoPlayer'�n Texture'�n� atama
        if (videoCubeRenderer != null && videoPlayer != null)
        {
            videoCubeRenderer.material.mainTexture = videoPlayer.targetTexture;
        }

        // E�er oda kurucusu isek, video ba�lant�lar�n� odaya kaydet
        if (PhotonNetwork.IsMasterClient)
        {
            SyncLinksWithRoomProperties();
        }
    }

    public void PlayVideo()
    {
        if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
            isPlaying = true;

            // RPC ile di�er oyunculara oynatma durumunu bildir
            photonView.RPC("SyncVideoState", RpcTarget.All, currentVideoIndex, true);
        }
    }

    public void PauseVideo()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            isPlaying = false;

            // RPC ile di�er oyunculara duraklatma durumunu bildir
            photonView.RPC("SyncVideoState", RpcTarget.All, currentVideoIndex, false);
        }
    }

    // Video URL'lerini g�ncelle ve odaya kaydet
    public void ApplyVideoUrls()
    {
        for (int i = 0; i < inputFields.Count; i++)
        {
            if (i < videoUrls.Count)
            {
                videoUrls[i] = inputFields[i].text;
            }
            else
            {
                videoUrls.Add(inputFields[i].text);
            }
        }

        Debug.Log("Video URL'leri g�ncellendi!");
        SyncLinksWithRoomProperties(); // G�ncellenen linkleri odaya kaydet
    }

    // Paneli a��p kapatma
    public void TogglePanel()
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }

    // Videolar aras�nda ge�i� yapma
    public void ChangeVideo(int videoIndex)
    {
        if (videoIndex >= 0 && videoIndex < videoUrls.Count)
        {
            currentVideoIndex = videoIndex;
            videoPlayer.url = videoUrls[videoIndex];

            if (isPlaying)
            {
                videoPlayer.Play();
            }

            // RPC ile video de�i�ikli�ini bildir
            photonView.RPC("SyncVideoState", RpcTarget.All, currentVideoIndex, isPlaying);

            Debug.Log($"Video de�i�tirildi: {videoUrls[videoIndex]}");
        }
        else
        {
            Debug.LogError("Ge�ersiz video indeksi se�ildi.");
        }
    }

    // Video linklerini Photon Custom Properties ile odaya kaydet
    private void SyncLinksWithRoomProperties()
    {
        ExitGames.Client.Photon.Hashtable videoLinks = new ExitGames.Client.Photon.Hashtable();
        for (int i = 0; i < videoUrls.Count; i++)
        {
            videoLinks["Video_" + i] = videoUrls[i];
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(videoLinks);
    }

    // Odaya kat�lan oyuncular i�in oda �zellikleri g�ncellenir
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        foreach (var key in propertiesThatChanged.Keys)
        {
            if (key.ToString().StartsWith("Video_"))
            {
                int index = int.Parse(key.ToString().Replace("Video_", ""));
                string url = propertiesThatChanged[key].ToString();

                if (index < videoUrls.Count)
                {
                    videoUrls[index] = url;
                }
                else
                {
                    videoUrls.Add(url);
                }
            }
        }

        // E�er bir video oynat�l�yorsa, yeni kat�lan oyuncular da onu g�rmeli
        if (videoUrls.Count > currentVideoIndex)
        {
            videoPlayer.url = videoUrls[currentVideoIndex];
            if (isPlaying)
            {
                videoPlayer.Play();
            }
        }
    }

    [PunRPC]
    void SyncVideoState(int videoIndex, bool playState)
    {
        if (videoIndex < videoUrls.Count)
        {
            currentVideoIndex = videoIndex;
            videoPlayer.url = videoUrls[videoIndex];
            isPlaying = playState;

            if (isPlaying)
            {
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Pause();
            }
        }
    }
}
