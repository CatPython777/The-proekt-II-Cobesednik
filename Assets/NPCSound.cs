using UnityEngine;

public class NPCSound : MonoBehaviour
{
    public AudioClip sound;
    
    void Start()
    {
        // Просто инициализация
    }
    
    void OnMouseDown()
    {
        if (sound != null)
        {
            AudioSource.PlayClipAtPoint(sound, transform.position);
            Debug.Log("NPC звук работает!");
        }
    }
}