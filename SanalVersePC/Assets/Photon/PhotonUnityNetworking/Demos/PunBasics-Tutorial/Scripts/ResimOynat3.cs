using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;
using UnityEngine.Networking;

public class ResimOynat3 : MonoBehaviourPunCallbacks
{
    public RawImage imageDisplay; // Resmi g�stermek i�in bir RawImage
    public Button nextButton;
    public Button prevButton;
    public GameObject loadingSpinner; // Y�kleme animasyonu

    private List<Texture2D> loadedTextures = new List<Texture2D>(); // Y�klenen resimlerin listesi
    private int index = 0; // Hangi resmin g�sterildi�ini takip eder

    void Start()
    {
        // Uygulaman�n k�k dizinindeki "Sunum" klas�r�nde, oyuncu ad�yla e�le�en klas�r� bul
        StartCoroutine(LoadImagesFromPlayerFolder());

        nextButton.onClick.AddListener(() => OnNextButtonPressed());
        prevButton.onClick.AddListener(() => OnPrevButtonPressed());
    }

    private IEnumerator LoadImagesFromPlayerFolder()
    {
        // Oyuncunun ad�n� al
        string playerName = PhotonNetwork.NickName;
        string folderPath = Path.Combine(Application.dataPath, "Sunum", playerName); // Oyuncu ad�na g�re klas�r� bul

        if (!Directory.Exists(folderPath))
        {
            Debug.LogError(playerName + " adl� oyuncuya ait klas�r bulunamad�: " + folderPath);
            yield break;
        }

        // Y�kleme i�lemine ba�lamadan �nce loading spinner'� g�ster
        loadingSpinner.SetActive(true);

        // Sadece .png ve .jpg dosyalar�n� al
        string[] files = Directory.GetFiles(folderPath, "*.*");
        foreach (var filePath in files)
        {
            if (filePath.EndsWith(".png") || filePath.EndsWith(".jpg"))
            {
                string url = "file:///" + filePath; // Yerel dosyaya eri�mek i�in "file:///" protokol� kullan�l�r
                using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
                {
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = DownloadHandlerTexture.GetContent(www); // Resmi Texture2D'ye y�kle
                        loadedTextures.Add(texture);
                    }
                    else
                    {
                        Debug.LogError("Resim y�klenemedi: " + www.error);
                    }
                }
            }
        }

        // Y�kleme i�lemi tamamland�
        loadingSpinner.SetActive(false); // Y�kleme spinner'�n� gizle

        // E�er hi� resim y�klenmezse uyar� ver
        if (loadedTextures.Count == 0)
        {
            Debug.LogError(playerName + " adl� oyuncunun klas�r�nde y�klenebilir resim bulunamad�!");
        }
        else
        {
            // �lk resmi ekrana g�ster
            index = 0;
            imageDisplay.texture = loadedTextures[index];
        }
    }

    public void ToggleLeft()
    {
        // Liste bo�sa hi�bir �ey yapma
        if (loadedTextures.Count == 0)
            return;

        // Bir �nceki resme ge�
        index--;
        if (index < 0)
            index = loadedTextures.Count - 1;

        // Resmi de�i�tir
        imageDisplay.texture = loadedTextures[index];
    }

    public void ToggleRight()
    {
        // Liste bo�sa hi�bir �ey yapma
        if (loadedTextures.Count == 0)
            return;

        // Bir sonraki resme ge�
        index++;
        if (index >= loadedTextures.Count)
            index = 0;

        // Resmi de�i�tir
        imageDisplay.texture = loadedTextures[index];
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

    public void OnNextButtonPressed()
    {
        photonView.RPC("NextImageRPC", RpcTarget.AllBuffered);
    }

    public void OnPrevButtonPressed()
    {
        photonView.RPC("PrevImageRPC", RpcTarget.AllBuffered);
    }
}
