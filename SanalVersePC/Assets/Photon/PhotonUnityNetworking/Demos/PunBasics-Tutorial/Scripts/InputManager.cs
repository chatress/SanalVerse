using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public List<InputField> inputFields; // InputField'lar�n�z� burada tutabilirsiniz.

    private InputField activeInputField; // Se�ili olan InputField

    void Update()
    {
        // Ctrl + V tu�lar�na bas�ld���nda �al��acak kod.
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            PasteToActiveInputField(); // Yap��t�rma i�lemi yaln�zca aktif input field'a yap�lacak
        }

        // E�er bir InputField t�klan�rsa, o input field'� aktif yap�yoruz
        foreach (InputField inputField in inputFields)
        {
            if (inputField.isFocused)
            {
                activeInputField = inputField;
                break;
            }
        }
    }

    void PasteToActiveInputField()
    {
        if (activeInputField != null)
        {
            string clipboardText = GUIUtility.systemCopyBuffer; // Panodaki metni al

            // E�er panoda bir �ey varsa, aktif input field'a yap��t�r
            if (!string.IsNullOrEmpty(clipboardText))
            {
                activeInputField.text = clipboardText; // Panodaki veriyi aktif input field'a yap��t�r
            }
        }
    }
}
