using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Chairsit : MonoBehaviourPunCallbacks
{
    private string characterTag; // Karakterin etiketini saklayacak de�i�ken
    public GameObject intText, standText; // Oturan nesnesi prefab'�n� atanacak de�i�ken
    public GameObject sittingPrefab1, sittingPrefab2, sittingPrefab3, sittingPrefab4;
    public bool interactable, sitting, standing;

    public GameObject Camera; // �kinci kamera
    public GameObject MainCamera; // �lk kamera

    private GameObject playerObject; // Player objesinin referans�
    private GameObject sittingObject; // Player objesinin referans�
    private Renderer characterRenderer;

    public GameObject avatarPrefab1;
    public GameObject avatarPrefab2;
    public GameObject avatarPrefab3;
    public GameObject avatarPrefab4;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(true);
            interactable = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            intText.SetActive(false);
            interactable = false;
        }
    }

    void Update()
    {
        if (interactable == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                intText.SetActive(false);
                sitting = true;
                interactable = false;
                standText.SetActive(true);
                MainCamera.SetActive(false);
                Camera.SetActive(true);

                playerObject = GameObject.FindGameObjectWithTag("ErkekOgretmen"); // Player etiketini uygun �ekilde de�i�tirebilirsiniz

                if (playerObject != null)
                {
                    PhotonNetwork.Destroy(playerObject); // Ayakta nesnesini yok et
                    sittingPrefab1.SetActive(true);
                }

                playerObject = GameObject.FindGameObjectWithTag("KadinOgretmen"); // Player etiketini uygun �ekilde de�i�tirebilirsiniz

                if (playerObject != null)
                {
                    PhotonNetwork.Destroy(playerObject); // Ayakta nesnesini yok et
                    sittingPrefab2.SetActive(true);
                }

                playerObject = GameObject.FindGameObjectWithTag("Erkek");

                if (playerObject != null)
                {
                    PhotonNetwork.Destroy(playerObject); // Ayakta nesnesini yok et
                    sittingPrefab3.SetActive(true);
                }

                playerObject = GameObject.FindGameObjectWithTag("Kiz");

                if (playerObject != null)
                {
                    PhotonNetwork.Destroy(playerObject); // Ayakta nesnesini yok et
                    sittingPrefab4.SetActive(true);
                }

                if (sitting == true)
                {
                    if (Input.GetKeyDown(KeyCode.Q))
                    {
                        intText.SetActive(true);
                        sitting = false;
                        interactable = true;
                        standText.SetActive(false);
                        MainCamera.SetActive(true);
                        Camera.SetActive(false);

                        playerObject = GameObject.FindGameObjectWithTag("ErkekOgretmen");
                        if (playerObject != null)
                        {
                            sittingPrefab1.SetActive(false);
                            PhotonNetwork.Instantiate(avatarPrefab1.name, Vector3.zero, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}