using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("=== НАСТРОЙКИ ДВИЖЕНИЯ ===")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 7f;
    public float gravity = 20f;
    
    [Header("=== НАСТРОЙКИ КАМЕРЫ ===")]
    public Camera playerCamera;
    public float mouseSensitivity = 2f;
    public float cameraLimit = 80f;
    
    [Header("=== INPUT SYSTEM ===")]
    public InputActionAsset inputActions;
    
    // Приватные переменные
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true;
    
    // Input Actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction runAction;
    private InputAction interactAction;
    
    // Переменные для ввода
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool runPressed;
    private bool interactPressed;
    
    void Start()
    {
        // Получаем компоненты
        characterController = GetComponent<CharacterController>();
        
        // Если камера не назначена - ищем основную
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Инициализация Input System
        InitializeInputSystem();
        
        // Блокируем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        Debug.Log("🎮 Инициализация управления персонажем");
    }
    
    void InitializeInputSystem()
    {
        if (inputActions == null)
        {
            Debug.LogError("❌ Input Action Asset не назначен!");
            return;
        }
        
        // Включаем Action Map
        inputActions.FindActionMap("PlayerMovement").Enable();
        
        // Получаем ссылки на действия
        moveAction = inputActions.FindAction("PlayerMovement/Move");
        lookAction = inputActions.FindAction("PlayerMovement/Look");
        jumpAction = inputActions.FindAction("PlayerMovement/Jump");
        runAction = inputActions.FindAction("PlayerMovement/Run");
        interactAction = inputActions.FindAction("PlayerMovement/Interact");
        
        // Подписываемся на события
        jumpAction.started += OnJumpStarted;
        interactAction.started += OnInteractStarted;
    }
    
    void OnDestroy()
    {
        // Отписываемся от событий
        jumpAction.started -= OnJumpStarted;
        interactAction.started -= OnInteractStarted;
    }
    
    void Update()
    {
        if (canMove)
        {
            // Читаем ввод
            ReadInput();
            
            // Поворот персонажа и камеры
            HandleRotation();
            
            // Движение персонажа
            HandleMovement();
        }
        
        // Выход из игры по ESC
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleCursor();
        }
    }
    
    void ReadInput()
    {
        // Получаем значения ввода
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
        runPressed = runAction.IsPressed();
    }
    
    void HandleRotation()
    {
        // Вращение персонажа по горизонтали (мышь X)
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(0, mouseX, 0);
        
        // Вращение камеры по вертикали (мышь Y)
        rotationX -= lookInput.y * mouseSensitivity * Time.deltaTime;
        rotationX = Mathf.Clamp(rotationX, -cameraLimit, cameraLimit);
        
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }
    
    void HandleMovement()
    {
        // Проверяем находится ли персонаж на земле
        bool isGrounded = characterController.isGrounded;
        
        // Вычисляем направление движения
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        
        // Отключаем движение по Y для направления
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        
        // Желаемое направление движения
        Vector3 desiredDirection = (forward * moveInput.y) + (right * moveInput.x);
        
        // Применяем скорость
        float currentSpeed = runPressed ? runSpeed : walkSpeed;
        Vector3 targetVelocity = desiredDirection * currentSpeed;
        
        // Плавное изменение скорости
        moveDirection.x = Mathf.Lerp(moveDirection.x, targetVelocity.x, Time.deltaTime * 10f);
        moveDirection.z = Mathf.Lerp(moveDirection.z, targetVelocity.z, Time.deltaTime * 10f);
        
        // Обработка прыжка
        if (isGrounded)
        {
            if (jumpPressed)
            {
                moveDirection.y = jumpForce;
                jumpPressed = false; // Сбрасываем флаг
                Debug.Log("🦘 Прыжок!");
            }
            else if (moveDirection.y < 0)
            {
                moveDirection.y = -2f; // Небольшая сила прижатия к земле
            }
        }
        else
        {
            // Применяем гравитацию в воздухе
            moveDirection.y -= gravity * Time.deltaTime;
        }
        
        // Применяем движение
        characterController.Move(moveDirection * Time.deltaTime);
    }
    
    // ОБРАБОТЧИКИ СОБЫТИЙ INPUT SYSTEM
    void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (characterController.isGrounded && canMove)
        {
            jumpPressed = true;
        }
    }
    
    void OnInteractStarted(InputAction.CallbackContext context)
    {
        interactPressed = true;
        Debug.Log("🔄 Взаимодействие с объектом");
        // Здесь можно добавить логику взаимодействия
        StartCoroutine(ResetInteract());
    }
    
    System.Collections.IEnumerator ResetInteract()
    {
        yield return new WaitForSeconds(0.1f);
        interactPressed = false;
    }
    
    void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canMove = true;
        }
    }
    
    // Методы для внешнего управления
    public void EnableMovement()
    {
        canMove = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inputActions.FindActionMap("PlayerMovement").Enable();
    }
    
    public void DisableMovement()
    {
        canMove = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        inputActions.FindActionMap("PlayerMovement").Disable();
    }
}