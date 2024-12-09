using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.Networking;
using SFB;

[RequireComponent(typeof(Button))]
public class CanvasSampleOpenFileImage : MonoBehaviour, IPointerDownHandler, IOnEventCallback
{
    public RawImage output; // Resim g�sterimi i�in RawImage
    private string playerName; // Oyuncu Ad�
    private const byte ShareUploadedFilesEventCode = 1; // Photon RPC i�in Event Code

    void Start()
    {
        playerName = PhotonNetwork.LocalPlayer.NickName; // Oyuncu ad�n� al
        Debug.Log($"Oyuncu Ad�: {playerName}");

        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        PhotonNetwork.AddCallbackTarget(this); // Photon callback i�in ekle
    }

    private void OnClick()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // WebGL i�in dosya se�ici
            OpenFilePanelForWebGL();
        }
        else
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Dosya Se�", "",
                new[] { new ExtensionFilter("G�r�nt� Dosyalar�", "png", "jpg"),
                        new ExtensionFilter("T�m Dosyalar", "*") }, true);

            if (paths.Length > 0)
            {
                output.texture = null;
                foreach (string selectedFilePath in paths)
                {
                    StartCoroutine(UploadFileToServer(selectedFilePath));
                }
            }
        }
    }

    // WebGL i�in FilePicker i�levi
    private void OpenFilePanelForWebGL()
    {
        string filePickerScript = @"
            var input = document.createElement('input');
            input.type = 'file';
            input.accept = 'image/*';
            input.onchange = function(e) {
                var file = e.target.files[0];
                if(file) {
                    var reader = new FileReader();
                    reader.onload = function(event) {
                        var base64Data = event.target.result.split(',')[1];
                        SendFileToUnity(base64Data);
                    };
                    reader.readAsDataURL(file);
                }
            };
            input.click();
        ";

        Application.ExternalEval(filePickerScript);
    }

    // JavaScript'ten gelen base64 verisini Unity'ye g�nderin
    private void SendFileToUnity(string base64Data)
    {
        StartCoroutine(UploadFileToServerFromBase64(base64Data));
    }

    // Standalone (PC, Mac, Linux) i�in dosya y�kleme
    private IEnumerator UploadFileToServer(string filePath)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(filePath);

        // Dosyay� server'a y�klemek i�in WWWForm olu�tur
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, System.IO.Path.GetFileName(filePath), "image/png");

        UnityWebRequest www = UnityWebRequest.Post("http://sanalverse.wuaze.com/wp-json/custom/v1/upload", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string fileUrl = www.downloadHandler.text;
            Debug.Log($"Dosya ba�ar�yla y�klendi: {fileUrl}");

            // Photon ile dosya URL'sini payla�
            ShareFileUrl(fileUrl);
        }
        else
        {
            Debug.LogError($"Dosya y�kleme ba�ar�s�z: {www.error}");
        }
    }

    // Base64 verisi ile sunucuya dosya y�kleme (WebGL)
    private IEnumerator UploadFileToServerFromBase64(string base64Data)
    {
        byte[] fileData = System.Convert.FromBase64String(base64Data);

        // Dosyay� server'a y�klemek i�in WWWForm olu�tur
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", fileData, "image.png", "image/png");

        UnityWebRequest www = UnityWebRequest.Post("http://sanalverse.wuaze.com/wp-json/custom/v1/upload", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string fileUrl = www.downloadHandler.text;
            Debug.Log($"Dosya ba�ar�yla y�klendi: {fileUrl}");

            // Photon ile dosya URL'sini payla�
            ShareFileUrl(fileUrl);
        }
        else
        {
            Debug.LogError($"Dosya y�kleme ba�ar�s�z: {www.error}");
        }
    }

    // Photon ile dosya URL'sini payla�ma
    private void ShareFileUrl(string fileUrl)
    {
        object[] content = new object[] { fileUrl };
        PhotonNetwork.RaiseEvent(ShareUploadedFilesEventCode, content, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
    }

    // URL'deki resmi y�kle ve g�ster
    private IEnumerator LoadTextureFromUrl(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            output.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }
        else
        {
            Debug.LogError($"Resim y�klenemedi: {www.error}");
        }
    }

    // Photon event callback'ini dinleyin
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == ShareUploadedFilesEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string fileUrl = (string)data[0];
            Debug.Log($"Di�er oyuncudan resim URL'si al�nd�: {fileUrl}");

            // URL'deki resmi y�kle ve g�ster
            StartCoroutine(LoadTextureFromUrl(fileUrl));
        }
    }

    // OnPointerDown metodunu ekleyin
    public void OnPointerDown(PointerEventData eventData)
    {
        // Burada butona t�klanma olay�n� i�leyebilirsiniz.
        Debug.Log("Pointer Down detected!");
    }

    void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this); // Callback'� kald�r
    }
}
