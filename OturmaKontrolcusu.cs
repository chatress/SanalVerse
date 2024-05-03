using UnityEngine;
using Photon.Pun;

public class OturmaKontrolcusu : MonoBehaviourPunCallbacks
{
    private Animator anim;
    private bool isSitting = false;
    private bool isWalking = false;
    private RaycastHit hitInfo; // T�klanan nesneyi saklamak i�in

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (photonView.IsMine) // Sadece yerel oyuncu bu kontrolleri i�lesin
        {
            if (isSitting)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // Oturma animasyonunu durdur
                    anim.SetBool("IsWalking", true);
                    isSitting = false;

                    // Karakterin y�n�n� sandalyeden uzakla�t�r
                    Vector3 newDirection = transform.forward;
                    newDirection.y = 0f;
                    transform.forward = newDirection;

                    // Oturma i�lemi di�er oyunculara iletilmeli
                    photonView.RPC("SetIsWalkingRPC", RpcTarget.All, true);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E)) // Sol t�kland���nda
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out hitInfo))
                    {
                        if (hitInfo.collider.CompareTag("Chair"))
                        {
                            // Sandalyeye t�klan�ld���nda oturma animasyonunu ba�lat
                            anim.SetBool("IsSitting", true);
                            isSitting = true;
                            transform.position = hitInfo.point; // Karakteri sandalyeye yerle�tir

                            // Karakterin y�n�n� sandalyeye do�ru �evir
                            Vector3 newDirection = hitInfo.transform.forward;
                            newDirection.y = 0f;
                            transform.forward = newDirection;

                            // Oturma i�lemi di�er oyunculara iletilmeli
                            photonView.RPC("SetIsSittingRPC", RpcTarget.All, true);
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    private void SetIsSittingRPC(bool value)
    {
        isSitting = value;
        anim.SetBool("IsSitting", value);
    }
    [PunRPC]
    private void SetIsWalkingRPC(bool value)
    {
        isWalking = value;
        anim.SetBool("IsWalking", value);
    }
}