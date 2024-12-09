using UnityEngine;
using UnityEngine.SceneManagement; // Sahne y�netimi i�in gerekli

public class MenuManager : MonoBehaviour
{
    // Yonetim sahnesine gitmek i�in
    public void GoToYonetimScene()
    {
        SceneManager.LoadScene("Yonetim"); // Yonetim adl� sahneye ge�i� yapar
    }

    // Menu sahnesine geri d�nmek i�in
    public void GoToMenuScene()
    {
        SceneManager.LoadScene("Menu"); // Menu adl� sahneye ge�i� yapar
    }
}
