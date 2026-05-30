using System.Collections;
using System.Collections.Generic;
using TMPro; // Исправлено: убран System.Drawing
using UnityEngine;
using UnityEngine.InputSystem;

public class bull : MonoBehaviour
{
    public float speed;
    public float distance;
    public int damage = 1; // Урон по умолчанию
    //public GameObject destroyEff;
    //public GameObject blood;
    public LayerMask mask;

    public TextMeshProUGUI Bullet_txt;
    public string requiredTag = "Player";
    public GameObject objectToActivate;

    void Start()
    {

    }

    void Update()
    {
        // Raycast для обнаружения целей (оставлено для совместимости)
        RaycastHit2D other = Physics2D.Raycast(transform.position, transform.up, distance, mask);
        Debug.DrawRay(transform.position, transform.up * distance, Color.red);

        if (other.collider != null)
        {
            // Проверяем, является ли объект игроком
            if (other.collider.CompareTag(requiredTag))
            {
                // Вычитаем -1 из текстового поля при попадании в игрока
                if (Bullet_txt != null)
                {
                    UpdateHealthText();
                }
                DestroyBullet();
            }
            else if (other.collider.CompareTag("Ground"))
            {
                DestroyBullet();
            }
        }

        // Движение пули вперед
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    // Метод для обработки столкновений через физику (OnCollisionEnter2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, что объект имеет нужный тег
        if (collision.gameObject.CompareTag(requiredTag))
        {
            // Вычитаем -1 из текстового поля
            UpdateHealthText();

            // Активируем объект если нужно
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            // Уничтожаем пулю при попадании
            DestroyBullet();
        }
        // Проверка на землю или другие препятствия
        else if (collision.gameObject.CompareTag("Ground") ||
                 collision.gameObject.CompareTag("Obstacle") ||
                 collision.gameObject.CompareTag("Wall"))
        {
            DestroyBullet();
        }
    }

    // Метод для обработки триггеров (OnTriggerEnter2D) - более простой способ
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что объект имеет нужный тег
        if (other.CompareTag(requiredTag))
        {
            Debug.Log($"Пуля попала в игрока: {other.gameObject.name}");

            // Вычитаем -1 из текстового поля
            UpdateHealthText();

            // Активируем объект если нужно
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            // Уничтожаем пулю при попадании
            DestroyBullet();
        }
        // Проверка на землю или другие препятствия
        else if (other.CompareTag("Ground") ||
                 other.CompareTag("Obstacle") ||
                 other.CompareTag("Wall"))
        {
            Debug.Log("Пуля попала в препятствие");
            DestroyBullet();
        }
    }

    // Метод для обновления текста здоровья
    private void UpdateHealthText()
    {
        if (Bullet_txt != null)
        {
            // Безопасное преобразование текста в число
            int currentValue;
            if (int.TryParse(Bullet_txt.text, out currentValue))
            {
                currentValue -= damage; // Вычитаем урон (по умолчанию 1)
                Bullet_txt.text = currentValue.ToString();
                Debug.Log($"Здоровье игрока: {Bullet_txt.text}");

                // Можно добавить проверку на смерть
                if (currentValue <= 0)
                {
                    Debug.Log("Игрок умер!");
                    // Здесь можно добавить логику смерти
                }
            }
            else
            {
                Debug.LogError($"Невозможно преобразовать '{Bullet_txt.text}' в число! Убедитесь, что текст содержит только цифры.");
                // Если текст не число, устанавливаем значение по умолчанию
                Bullet_txt.text = "100";
            }
        }
        else
        {
            Debug.LogError("Bullet_txt не назначен! Перетащите TextMeshProUGUI компонент в инспекторе.");
        }
    }

    private void DestroyBullet()
    {
        //Instantiate(destroyEff, transform.position, Quaternion.identity);
        //Instantiate(blood, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    private void OnBecameInvisible()
    {
        DestroyBullet();
    }
}