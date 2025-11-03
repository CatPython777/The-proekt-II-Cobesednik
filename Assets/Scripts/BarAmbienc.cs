using UnityEngine;
using System.Collections;

public class BarAmbience : MonoBehaviour
{
    [Header("=== ОСВЕЩЕНИЕ ===")]
    public Light[] neonLights;
    public Light mainBarLight;
    public GameObject[] neonSigns;
    
    [Header("=== АТМОСФЕРНЫЕ ЭФФЕКТЫ ===")]
    public ParticleSystem smokeEffect;
    public AudioSource ambientMusic;
    public AudioSource crowdSounds;
    
    [Header("=== АНИМАЦИИ ===")]
    public Animator bartenderAnimator;
    public GameObject[] flickeringLights;
    
    [Header("=== НАСТРОЙКИ ===")]
    public float minFlickerDelay = 0.1f;
    public float maxFlickerDelay = 0.5f;
    public Color[] neonColors = {
        new Color(0.1f, 0.8f, 1f),    // Голубой
        new Color(1f, 0.2f, 0.8f),    // Розовый  
        new Color(0.6f, 0.1f, 1f),    // Фиолетовый
        new Color(0.1f, 1f, 0.3f)     // Зеленый
    };
    
    private bool isFlickering = false;

    void Start()
    {
        Debug.Log("🔄 Инициализация освещения бара...");
        InitializeLighting();
        StartAmbience();
        StartCoroutine(FlickerRoutine());
    }
    
    void InitializeLighting()
    {
        // Настройка основного света
        if (mainBarLight != null)
        {
            mainBarLight.color = new Color(1f, 0.9f, 0.8f); // Теплый белый
            mainBarLight.intensity = 0.3f;
            mainBarLight.shadows = LightShadows.Soft;
        }
        
        // Настройка неоновых огней
        for (int i = 0; i < neonLights.Length; i++)
        {
            if (neonLights[i] != null)
            {
                Color randomColor = neonColors[Random.Range(0, neonColors.Length)];
                neonLights[i].color = randomColor;
                neonLights[i].intensity = Random.Range(1.5f, 3f);
                neonLights[i].range = Random.Range(4f, 8f);
            }
        }
        
        Debug.Log("✅ Освещение инициализировано");
    }
    
    void StartAmbience()
    {
        // Запуск эффектов
        if (smokeEffect != null) 
        {
            smokeEffect.Play();
            Debug.Log("💨 Дым запущен");
        }
        
        // Запуск звуков
        if (ambientMusic != null && !ambientMusic.isPlaying)
        {
            ambientMusic.Play();
            Debug.Log("🎵 Фоновая музыка запущена");
        }
        
        if (crowdSounds != null && !crowdSounds.isPlaying)
        {
            crowdSounds.Play();
            Debug.Log("👥 Шум толпы запущен");
        }
        
        // Запуск анимации бармена
        if (bartenderAnimator != null)
        {
            bartenderAnimator.SetBool("IsWorking", true);
            Debug.Log("👨‍💻 Анимация бармена запущена");
        }
    }
    
    IEnumerator FlickerRoutine()
    {
        isFlickering = true;
        
        while (isFlickering)
        {
            yield return new WaitForSeconds(Random.Range(minFlickerDelay, maxFlickerDelay));
            
            // Случайное мерцание
            foreach (GameObject lightObj in flickeringLights)
            {
                if (lightObj != null && Random.value > 0.8f)
                {
                    Light lightComp = lightObj.GetComponent<Light>();
                    if (lightComp != null)
                    {
                        StartCoroutine(SingleFlicker(lightComp));
                    }
                }
            }
        }
    }
    
    IEnumerator SingleFlicker(Light light)
    {
        float originalIntensity = light.intensity;
        
        // Быстрое мерцание
        light.intensity = 0f;
        yield return new WaitForSeconds(0.05f);
        light.intensity = originalIntensity;
        yield return new WaitForSeconds(0.1f);
        light.intensity = 0f;
        yield return new WaitForSeconds(0.02f);
        light.intensity = originalIntensity;
    }
    
    void OnDestroy()
    {
        isFlickering = false;
        StopAllCoroutines();
    }
    
    // Публичные методы для управления из других скриптов
    public void SetMusicVolume(float volume)
    {
        if (ambientMusic != null)
            ambientMusic.volume = volume;
    }
    
    public void SetCrowdVolume(float volume)
    {
        if (crowdSounds != null)
            crowdSounds.volume = volume;
    }
}
