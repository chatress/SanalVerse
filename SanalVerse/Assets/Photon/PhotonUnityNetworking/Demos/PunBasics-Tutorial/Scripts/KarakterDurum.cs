using UnityEngine;
using UnityEngine.UI;

public class KarakterDurum : MonoBehaviour
{
    public Toggle studentToggle;
    public Toggle teacherToggle;

    void Start()
    {
        // �nce kaydedilmi� bir se�im var m� kontrol edin
        if (PlayerPrefs.HasKey("IsStudent"))
        {
            // Kaydedilmi� de�eri y�kleyin
            bool isStudent = PlayerPrefs.GetInt("IsStudent") == 1;
            studentToggle.isOn = isStudent;
            teacherToggle.isOn = !isStudent;
        }
        else
        {
            // Varsay�lan olarak ��renci se�ili olsun
            studentToggle.isOn = true;
            teacherToggle.isOn = false;
            PlayerPrefs.SetInt("IsStudent", 1);
        }
    }

    public void OnStudentToggle(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("IsStudent", 1); // ��renci olarak ayarlay�n
            teacherToggle.isOn = false;
        }
    }

    public void OnTeacherToggle(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("IsStudent", 0); // ��retmen olarak ayarlay�n
            studentToggle.isOn = false;
        }
    }
}
