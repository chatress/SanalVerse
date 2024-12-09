using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;

public class SunumYukle : MonoBehaviourPunCallbacks
{
    public RawImage imageDisplay; // Resmi g�sterecek RawImage bile�eni
    private List<Texture2D> loadedTextures = new List<Texture2D>(); // Y�klenen resimlerin listesi

    // OpenFilePicker fonksiyonunu WebGL'de �a��r�yoruz
    public void OpenFilePicker()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        // JavaScript'ten dosya se�imini tetikle
        Application.ExternalEval("UploadFile('OnFileSelected');");
#else
        Debug.LogWarning("JavaScript k�pr�s� sadece WebGL'de �al���r.");
#endif
    }

    // JavaScript'ten g�nderilen base64 verisini al�r ve resmi y�kler
    public void OnFileSelected(string base64Data)
    {
        // Base64 verisini i�leyip Texture2D'ye d�n��t�r
        byte[] imageBytes = System.Convert.FromBase64String(base64Data.Substring(base64Data.IndexOf(",") + 1));
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        // Y�klenen resmi listeye ekle
        loadedTextures.Add(texture);

        // �lk resmi g�ster (iste�e ba�l�)
        if (loadedTextures.Count == 1)
        {
            imageDisplay.texture = loadedTextures[0];
        }

        // Photon �zerinden di�er oyunculara resmi g�nder
        photonView.RPC("SyncImage", RpcTarget.AllBuffered, base64Data);
    }

    // Resmi senkronize etmek i�in RPC fonksiyonu
    [PunRPC]
    public void SyncImage(string base64Data)
    {
        byte[] imageBytes = System.Convert.FromBase64String(base64Data.Substring(base64Data.IndexOf(",") + 1));
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);

        // Resmi RawImage bile�enine ata
        imageDisplay.texture = texture;
    }

    // Resimler aras�nda ge�i� yapmaya yarayan metodlar
    public void ShowNextImage()
    {
        if (loadedTextures.Count == 0) return;

        // Bir sonraki resme ge�
        int nextIndex = (loadedTextures.IndexOf((Texture2D)imageDisplay.texture) + 1) % loadedTextures.Count;
        imageDisplay.texture = loadedTextures[nextIndex];

        // Photon �zerinden di�er oyunculara ge�i�i bildir
        photonView.RPC("SyncNextImage", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SyncNextImage()
    {
        if (loadedTextures.Count == 0) return;

        // Bir sonraki resme ge�
        int nextIndex = (loadedTextures.IndexOf((Texture2D)imageDisplay.texture) + 1) % loadedTextures.Count;
        imageDisplay.texture = loadedTextures[nextIndex];
    }

    public void ShowPreviousImage()
    {
        if (loadedTextures.Count == 0) return;

        // Bir �nceki resme ge�
        int prevIndex = (loadedTextures.IndexOf((Texture2D)imageDisplay.texture) - 1 + loadedTextures.Count) % loadedTextures.Count;
        imageDisplay.texture = loadedTextures[prevIndex];

        // Photon �zerinden di�er oyunculara ge�i�i bildir
        photonView.RPC("SyncPreviousImage", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void SyncPreviousImage()
    {
        if (loadedTextures.Count == 0) return;

        // Bir �nceki resme ge�
        int prevIndex = (loadedTextures.IndexOf((Texture2D)imageDisplay.texture) - 1 + loadedTextures.Count) % loadedTextures.Count;
        imageDisplay.texture = loadedTextures[prevIndex];
    }
}
