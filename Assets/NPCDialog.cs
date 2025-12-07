using UnityEngine;
using UnityEngine.UI;
using TMPro; // Добавьте эту строку!

public class NPCDialog : MonoBehaviour
{
    public AudioClip clickSound;
    public GameObject dialogPanel;
    public TMP_InputField inputField; // Измените тип!
    public Button sendButton;
    public Button closeButton;
    
    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
            
        if (sendButton != null)
            sendButton.onClick.AddListener(OnSendButtonClick);
            
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseButtonClick);
    }
    
    void OnMouseDown()
    {
        if (clickSound != null)
        {
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
        }
        
        if (dialogPanel != null)
        {
            dialogPanel.SetActive(true);
            
            if (inputField != null)
            {
                inputField.Select();
                inputField.ActivateInputField();
            }
        }
    }
    
    public void OnSendButtonClick()
    {
        if (inputField != null && !string.IsNullOrEmpty(inputField.text))
        {
            Debug.Log("Вы написали: " + inputField.text);
            inputField.text = "";
            dialogPanel.SetActive(false);
        }
    }
    
    public void OnCloseButtonClick()
    {
        if (dialogPanel != null)
        {
            if (inputField != null)
                inputField.text = "";
            dialogPanel.SetActive(false);
        }
    }
}
