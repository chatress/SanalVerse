using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OturmaKodu2 : MonoBehaviourPunCallbacks
{
    private bool isOtur = false;
    private GameObject chair;

    private void Update()
            {
                if (photonView.IsMine && Input.GetMouseButtonDown(0))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (!isOtur && hit.collider.CompareTag("Chair"))
                        {
                            // Oturmayan bir karakteri t�klayarak oturttuk
                            photonView.RPC("SitDown", RpcTarget.AllBuffered, hit.collider.gameObject.GetPhotonView().ViewID);
                        }
                        else if (isOtur)
                        {
                            // Oturan bir karakteri t�klayarak kald�rd�k
                            photonView.RPC("StandUp", RpcTarget.AllBuffered, hit.collider.gameObject.GetPhotonView().ViewID);
                        }
                    }
                }
            }

            [PunRPC]
            private void SitDown(int chairViewID)
            {
        // Oturma i�lemi ger�ekle�tir
        isOtur = true;
                chair = PhotonView.Find(chairViewID).gameObject;
                transform.position = chair.transform.position;
                transform.rotation = chair.transform.rotation;
            }

            [PunRPC]
            private void StandUp()
            {
        // Kalkma i�lemi ger�ekle�tir
        isOtur = false;
                transform.Translate(Vector3.forward * 2f); // Varsay�lan olarak sandalyeden kalkt���nda karakteri bir ad�m ileri ta��d�k
            }
        }
