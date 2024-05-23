using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KarakterDurum : MonoBehaviour
{
    public Toggle studentToggle;
    public Toggle teacherToggle;

    private bool isStudent = true;

    void Start()
    {
        // �lk olarak ��renci se�ili olarak ba�lay�n
        studentToggle.isOn = true;
        teacherToggle.isOn = false;
    }

    public void OnStudentToggle(bool isOn)
    {
        if (isOn)
        {
            isStudent = true;
            teacherToggle.isOn = false;
        }
    }

    public void OnTeacherToggle(bool isOn)
    {
        if (isOn)
        {
            isStudent = false;
            studentToggle.isOn = false;
        }
    }

    // Se�ilen karakterin kimli�ini d�nd�rmek i�in bu i�levi kullanabilirsiniz
    public int GetSelectedCharacter()
    {
        return isStudent ? 0 : 1;
    }
}