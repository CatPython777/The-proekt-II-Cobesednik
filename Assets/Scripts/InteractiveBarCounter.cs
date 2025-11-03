using UnityEngine;
using System.Collections;

public class InteractiveBarCounter : MonoBehaviour
{
    [Header("=== ПОЗИЦИИ ДЛЯ БОКАЛОВ ===")]
    public Transform[] glassPositions;
    
    [Header("=== ПРЕФАБЫ ===")]
    public GameObject[] drinkPrefabs;
    
    [Header("=== ЭФФЕКТЫ ===")]
    public ParticleSystem pourEffect;
    public AudioClip[] drinkSounds;
    
    [Header("=== НАСТРОЙКИ ===")]
    public float serveAnimationHeight = 0.3f;
    public float serveAnimationDuration = 0.5f;
    
    private AudioSource audioSource;
    private int currentGlassIndex = 0;
    private bool canInteract = true;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // 3D звук
            audioSource.volume = 0.7f;
        }
        
        Debug.Log("🍸 Интерактивная стойка бара готова!");
    }
    
    void Update()
    {
        // Обработка клика мыши
        if (canInteract && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("🎯 Клик по стойке бара в точке: " + hit.point);
                    ServeDrink(hit.point);
                }
            }
        }
    }
    
    public void ServeDrink(Vector3 servePosition)
    {
        if (!canInteract || glassPositions.Length == 0) return;
        
        StartCoroutine(ServeDrinkRoutine(servePosition));
    }
    
    private IEnumerator ServeDrinkRoutine(Vector3 servePosition)
    {
        canInteract = false;
        
        // 1. Эффект наливания
        if (pourEffect != null)
        {
            pourEffect.transform.position = servePosition + Vector3.up * 0.5f;
            pourEffect.Play();
            Debug.Log("💧 Эффект наливания запущен");
        }
        
        // 2. Звук наливания
        if (audioSource != null && drinkSounds.Length > 0)
        {
            AudioClip randomSound = drinkSounds[Random.Range(0, drinkSounds.Length)];
            audioSource.PlayOneShot(randomSound);
            Debug.Log("🔊 Воспроизведение звука: " + randomSound.name);
        }
        
        // 3. Ждем немного перед созданием бокала
        yield return new WaitForSeconds(0.3f);
        
        // 4. Создаем бокал
        if (drinkPrefabs.Length > 0 && glassPositions.Length > 0)
        {
            Transform spawnPoint = glassPositions[currentGlassIndex];
            GameObject selectedDrink = drinkPrefabs[Random.Range(0, drinkPrefabs.Length)];
            
            GameObject newDrink = Instantiate(
                selectedDrink,
                spawnPoint.position,
                spawnPoint.rotation
            );
            
            newDrink.name = "Drink_" + currentGlassIndex;
            Debug.Log("🍹 Создан бокал: " + newDrink.name);
            
            // 5. Анимация подачи
            yield return StartCoroutine(AnimateDrinkServing(newDrink));
            
            // 6. Переход к следующей позиции
            currentGlassIndex = (currentGlassIndex + 1) % glassPositions.Length;
        }
        
        canInteract = true;
    }
    
    private IEnumerator AnimateDrinkServing(GameObject drink)
    {
        Vector3 startPos = drink.transform.position;
        Vector3 endPos = startPos + Vector3.up * serveAnimationHeight;
        
        float elapsed = 0f;
        
        // Поднимаем бокал
        while (elapsed < serveAnimationDuration)
        {
            drink.transform.position = Vector3.Lerp(startPos, endPos, elapsed / serveAnimationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Небольшая пауза наверху
        yield return new WaitForSeconds(0.1f);
        
        // Опускаем обратно
        elapsed = 0f;
        while (elapsed < serveAnimationDuration)
        {
            drink.transform.position = Vector3.Lerp(endPos, startPos, elapsed / serveAnimationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("✅ Бокал подан!");
    }
    
    // Визуальная обратная связь при наведении
    void OnMouseEnter()
    {
        if (canInteract)
        {
            // Можно добавить подсветку стойки
            Debug.Log("🎯 Курсор над стойкой бара");
        }
    }
    
    void OnMouseExit()
    {
        // Убрать подсветку
    }
}