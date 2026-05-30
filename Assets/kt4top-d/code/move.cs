using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class move : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Vector3 _position;
    private Rigidbody2D rb;
    private Vector2 _movementInput;
    private Vector2 _mousePosition;
    public Transform guntransform;
    [SerializeField] private Camera mainCamera;
    public float offset;
    public float startTime;
    private float time;
    public float nextFireTime = 0f;
    public float Bullet_number;

    public GameObject bullet;
    public Transform point;
    public TextMeshProUGUI Bullet_txt;
    public int valueToSubtract = 1;

    // Настройки камеры
    [Header("Camera Settings")]
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -10); // Смещение камеры
    [SerializeField] private bool smoothCameraFollow = true; // Плавное следование
    [SerializeField] private float cameraSmoothSpeed = 5f; // Скорость плавного следования

    private Vector3 cameraVelocity = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Находим камеру если не назначена
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Устанавливаем начальную позицию камеры
        if (mainCamera != null)
        {
            mainCamera.transform.position = transform.position + cameraOffset;
        }
    }

    void Update()
    {
        Vector2 moveDirection = new Vector2(_position.x, 0);
        transform.position += (Vector3)moveDirection * (_speed * Time.deltaTime);

        MoveCharacterTransform();
        AimWeapon();

        // Следим за камерой
        FollowCamera();
    }

    private void MoveCharacterTransform()
    {
        // Создаем вектор движения
        Vector3 movement = new Vector3(_movementInput.x, _movementInput.y, 0);

        // Нормализуем вектор для сохранения одинаковой скорости по диагонали
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Применяем движение
        transform.position += movement * (_speed * Time.deltaTime);
    }

    // Метод для следования камеры за игроком
    private void FollowCamera()
    {
        if (mainCamera == null) return;

        Vector3 targetPosition = transform.position + cameraOffset;

        if (smoothCameraFollow)
        {
            // Плавное следование камеры
            mainCamera.transform.position = Vector3.SmoothDamp(
                mainCamera.transform.position,
                targetPosition,
                ref cameraVelocity,
                1f / cameraSmoothSpeed
            );
        }
        else
        {
            // Мгновенное следование
            mainCamera.transform.position = targetPosition;
        }

        // ВАЖНО: Не меняем вращение камеры!
        // Камера остается с исходным вращением
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Получаем вектор ввода
        _movementInput = context.ReadValue<Vector2>();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        // Получаем позицию мыши для прицеливания
        _mousePosition = context.ReadValue<Vector2>();
    }

    private void AimWeapon()
    {
        if (mainCamera == null) return;

        // Получаем позицию мыши из сохраненного значения
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            _mousePosition.x,
            _mousePosition.y,
            -mainCamera.transform.position.z
        ));

        // Вычисляем направление от оружия к мыши
        Vector2 direction = mouseWorldPosition - guntransform.position;

        // Поворачиваем оружие
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        guntransform.rotation = Quaternion.Euler(0f, 0f, angle + offset);

        // Опционально: отражаем спрайт оружия если нужно
        if (direction.x < 0)
        {
            guntransform.localScale = new Vector3(1, -1, 1);
        }
        else
        {
            guntransform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void atack(InputAction.CallbackContext context)
    {
        // Проверяем, что кнопка нажата (performed) и прошло достаточно времени
        if (context.performed && Time.time >= nextFireTime)
        {
            int currentValue = int.Parse(Bullet_txt.text);
            currentValue -= valueToSubtract;
            Bullet_txt.text = currentValue.ToString();

            Hp();

            Instantiate(bullet, point.position, transform.rotation);
            nextFireTime = Time.time + startTime;  // Устанавливаем время следующего выстрела
        }
    }

    public void Hp()
    {
        int hp = int.Parse(Bullet_txt.text);
        if (hp == 0)
        {
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        Debug.Log("Player caught! Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}