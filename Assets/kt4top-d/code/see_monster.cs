using UnityEngine;

public class CollisionActivator : MonoBehaviour
{
    // ▼▼▼ НАСТРОЙКИ ▼▼▼
    public GameObject objectToActivate; // Объект для активации
    public string requiredTag = "Player"; // Тег объекта, который должен коснуться
                                          // ▲▲▲ НАСТРОЙКИ ▲▲▲

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Если тег подходит или тег не указан (пустая строка)
        if (string.IsNullOrEmpty(requiredTag) || collision.gameObject.CompareTag(requiredTag))
        {
            // ▼▼▼ ВСТАВЬ СВОЙ КОД ЗДЕСЬ ▼▼▼

            // Пример активации объекта:
            if (objectToActivate != null)
                objectToActivate.SetActive(true);

        }
    }
}

