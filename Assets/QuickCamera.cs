using UnityEngine;

public class NoClipCameraRaycast : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 2f;
    public float playerRadius = 0.5f; // Радиус камеры
    public LayerMask collisionMask = -1; // С какими слоями сталкиваться
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        HandleMouseLook();
        HandleMovementWithCollision();
        HandleCursorToggle();
    }
    
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
    
    void HandleMovementWithCollision()
    {
        Vector3 desiredMove = Vector3.zero;
        
        // Обычное движение
        desiredMove = transform.forward * Input.GetAxis("Vertical") + 
                     transform.right * Input.GetAxis("Horizontal");
        
        // Вверх/вниз
        if (Input.GetKey(KeyCode.Space)) desiredMove.y = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) desiredMove.y = -1f;
        
        // Применяем движение с учетом столкновений
        if (desiredMove.magnitude > 0)
        {
            MoveWithCollision(desiredMove.normalized * moveSpeed * Time.deltaTime);
        }
    }
    
    void MoveWithCollision(Vector3 moveDelta)
    {
        // Разбиваем движение на компоненты для раздельной проверки
        Vector3 newPosition = transform.position;
        
        // Сначала проверяем движение по X
        if (moveDelta.x != 0)
        {
            Vector3 xMove = new Vector3(moveDelta.x, 0, 0);
            if (!Physics.CheckSphere(newPosition + xMove, playerRadius, collisionMask))
            {
                newPosition += xMove;
            }
        }
        
        // Затем проверяем движение по Z
        if (moveDelta.z != 0)
        {
            Vector3 zMove = new Vector3(0, 0, moveDelta.z);
            if (!Physics.CheckSphere(newPosition + zMove, playerRadius, collisionMask))
            {
                newPosition += zMove;
            }
        }
        
        // Затем проверяем движение по Y (вверх/вниз)
        if (moveDelta.y != 0)
        {
            Vector3 yMove = new Vector3(0, moveDelta.y, 0);
            if (!Physics.CheckSphere(newPosition + yMove, playerRadius, collisionMask))
            {
                newPosition += yMove;
            }
        }
        
        transform.position = newPosition;
    }
    
    // Альтернативный метод - более плавное движение вдоль стен
    void MoveWithCollisionSmooth(Vector3 moveDelta)
    {
        Vector3 direction = moveDelta.normalized;
        float distance = moveDelta.magnitude;
        
        // Пытаемся двигаться в желаемом направлении
        if (!Physics.SphereCast(transform.position, playerRadius, direction, out RaycastHit hit, distance, collisionMask))
        {
            // Если нет препятствий - просто двигаемся
            transform.position += moveDelta;
        }
        else
        {
            // Если есть препятствие, пытаемся двигаться вдоль него
            Vector3 slideDirection = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
            
            // Проверяем можно ли двигаться вдоль стены
            if (!Physics.SphereCast(transform.position, playerRadius, slideDirection, out _, distance, collisionMask))
            {
                transform.position += slideDirection * distance;
            }
        }
    }
    
    void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? 
                CursorLockMode.None : CursorLockMode.Locked;
        }
    }
    
    // Визуализация радиуса камеры в редакторе
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerRadius);
    }
}