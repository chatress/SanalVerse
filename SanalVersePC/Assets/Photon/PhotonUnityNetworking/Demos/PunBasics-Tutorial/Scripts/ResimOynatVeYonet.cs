using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.Networking;
using Photon.Pun.Demo.PunBasics;
using System;

public class ResimOynatVeYonet : MonoBehaviourPunCallbacks
{
    public RawImage imageDisplay;
    public Button nextButton;
    public Button prevButton;

    private List<Texture2D> loadedTextures = new List<Texture2D>();
    private int index = 0;

    [Header("Sunum Y�netimi")]
    public List<string> imageUrls; // Resim ba�lant�lar�
    public List<InputField> linkInputs; // Paneldeki InputField'lar
    public GameObject linkEditorPanel; // Link d�zenleme paneli

    private PlayerAnimatorManager localPlayerMovementScript; // Yerel oyuncunun hareket scripti

    private void Start()
    {
        StartCoroutine(LoadImagesFromLinks());

        nextButton.onClick.AddListener(() => OnNextButtonPressed());
        prevButton.onClick.AddListener(() => OnPrevButtonPressed());

        // Yerel oyuncunun hareket scriptini bul
        foreach (var player in FindObjectsOfType<PlayerAnimatorManager>())
        {
            if (player.photonView.IsMine)
            {
                localPlayerMovementScript = player;
                break;
            }
        }
    }

    IEnumerator LoadImagesFromLinks()
    {
        loadedTextures.Clear();

        foreach (var url in imageUrls)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    loadedTextures.Add(texture);
                }
                else
                {
                    Debug.LogError("Resim y�klenemedi: " + request.error);
                }
            }
        }

        if (loadedTextures.Count > 0)
        {
            index = 0;
            imageDisplay.texture = loadedTextures[index];
        }
        else
        {
            Debug.LogWarning("Y�klenecek resim bulunamad�!");
        }
    }

    public void OpenLinkEditor()
    {
        linkEditorPanel.SetActive(true);

        // Hareket scriptini devre d��� b�rak
        if (localPlayerMovementScript != null)
        {
            localPlayerMovementScript.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        for (int i = 0; i < linkInputs.Count; i++)
        {
            if (i < imageUrls.Count)
            {
                linkInputs[i].text = imageUrls[i];
            }
        }
    }

    public void CloseLinkEditor()
    {
        linkEditorPanel.SetActive(false);

        // Hareket scriptini tekrar etkinle�tir
        if (localPlayerMovementScript != null)
        {
            localPlayerMovementScript.enabled = true;
        }
    }

    public void SaveLinks()
    {
        imageUrls.Clear(); // Mevcut listeyi temizle

        foreach (var input in linkInputs)
        {
            // E�er input bo� de�ilse ve ge�erli bir URL ise i�leme al
            if (!string.IsNullOrEmpty(input.text) && IsValidURL(input.text))
            {
                imageUrls.Add(input.text);
            }
            else if (string.IsNullOrEmpty(input.text))
            {
                Debug.LogWarning("Bo� bir InputField tespit edildi, atlan�yor.");
            }
            else
            {
                Debug.LogError("Ge�ersiz URL: " + input.text);
            }
        }

        // G�ncellenen URL'leri t�m oyunculara g�nder
        photonView.RPC("UpdateImageLinks", RpcTarget.AllBuffered, imageUrls.ToArray());
    }

    // URL'nin ge�erli olup olmad���n� kontrol eden yard�mc� fonksiyon
    private bool IsValidURL(string url)
    {
        return Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }


    [PunRPC]
    public void UpdateImageLinks(string[] newLinks)
    {
        imageUrls = new List<string>(newLinks);

        StartCoroutine(LoadImagesFromLinks());
    }

    public void OnNextButtonPressed()
    {
        photonView.RPC("NextImageRPC", RpcTarget.AllBuffered);
    }

    public void OnPrevButtonPressed()
    {
        photonView.RPC("PrevImageRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void NextImageRPC()
    {
        ToggleRight();
    }

    [PunRPC]
    public void PrevImageRPC()
    {
        ToggleLeft();
    }

    public void ToggleRight()
    {
        if (loadedTextures.Count == 0) return;

        index++;
        if (index >= loadedTextures.Count)
            index = 0;

        imageDisplay.texture = loadedTextures[index];
    }

    public void ToggleLeft()
    {
        if (loadedTextures.Count == 0) return;

        index--;
        if (index < 0)
            index = loadedTextures.Count - 1;

        imageDisplay.texture = loadedTextures[index];
    }
}
