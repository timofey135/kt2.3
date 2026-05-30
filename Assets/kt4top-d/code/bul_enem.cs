using UnityEngine;
using TMPro;

public class SimpleBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public GameObject player;
    public TextMeshProUGUI healthText;

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        
        if (collision.gameObject.CompareTag("Player"))
        {
            int currentHealth = int.Parse(healthText.text);
            currentHealth -= damage;
            healthText.text = currentHealth.ToString();
            Debug.Log($"Здоровье игрока: {currentHealth}");
            Destroy(gameObject);
        }

        // Попадание в стены (по тегам)
        else if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Пуля попала в препятствие");
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}