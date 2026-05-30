using UnityEngine;

public class enemy : MonoBehaviour
{
    public Transform player;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    public float rotationSpeed = 7f;
    public float shootRange = 10f;
    public bool smoothRotation = true;

    // Параметры для отступления
    public float retreatDistance = 5f;
    public float retreatSpeed = 3f;
    public float minimumDistance = 3f;

    // Параметры здоровья
    public int health = 1; // Сколько попаданий выдерживает враг

    private float lineOfSightTimer;
    private bool hasLineOfSight = false;
    public bool requireLineOfSight = true;
    public LayerMask obstacleMask;
    public float checkFrequency = 0.2f;

    private float fireTimer;
    private bool canShoot = false;

    void Start()
    {
        fireTimer = fireRate;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Проверяем, может ли враг стрелять (в красной зоне и есть видимость)
        canShoot = distanceToPlayer <= shootRange && hasLineOfSight;

        // Отступаем если игрок слишком близко
        if (distanceToPlayer < retreatDistance)
        {
            Retreat();
        }

        // Всегда поворачиваемся к игроку
        RotateToPlayer();

        // Проверка прямой видимости
        lineOfSightTimer -= Time.deltaTime;
        if (lineOfSightTimer <= 0f)
        {
            hasLineOfSight = !requireLineOfSight || CheckLineOfSight();
            lineOfSightTimer = checkFrequency;
        }

        // Логика стрельбы с интервалом
        if (canShoot)
        {
            // Уменьшаем таймер
            fireTimer -= Time.deltaTime;

            // Когда таймер доходит до 0 - стреляем и сбрасываем таймер
            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = fireRate;
            }
        }
        else
        {
            // Если не можем стрелять - держим таймер полным
            fireTimer = fireRate;
        }
    }

    void Retreat()
    {
        Vector2 directionFromPlayer = (transform.position - player.position).normalized;
        transform.position += (Vector3)directionFromPlayer * retreatSpeed * Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer < minimumDistance)
        {
            transform.position += (Vector3)directionFromPlayer * retreatSpeed * Time.deltaTime * 2f;
        }
    }

    void RotateToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    bool CheckLineOfSight()
    {
        if (player == null) return false;

        Vector2 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, obstacleMask);
        Debug.DrawRay(transform.position, direction, hit.collider == null ? Color.green : Color.red);

        return hit.collider == null;
    }

    // Метод для получения урона
    

    // Метод смерти врага
    void Die()
    {
        Debug.Log("Враг уничтожен!");

        // Уничтожаем врага
        Destroy(gameObject);
    }

    // Обработка столкновений с пулями
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Уничтожаем объект при касании с любым объектом, у которого тег "Bullet"
        if (other.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }

    void OnGUI()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            string status = canShoot ? "СТРЕЛЯЕТ" : "НЕ СТРЕЛЯЕТ";
            GUI.color = canShoot ? Color.red : Color.white;
            GUI.Label(new Rect(10, 10, 200, 20), $"Статус: {status}");
            GUI.Label(new Rect(10, 30, 200, 20), $"Дистанция: {distanceToPlayer:F1} / {shootRange}");
            GUI.Label(new Rect(10, 50, 200, 20), $"До выстрела: {fireTimer:F1} сек");
            GUI.Label(new Rect(10, 70, 200, 20), $"Здоровье врага: {health}");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, retreatDistance);

        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, minimumDistance);

        if (firePoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(firePoint.position, firePoint.right * 2f);
        }

        if (Application.isPlaying && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            Gizmos.color = distanceToPlayer <= shootRange ? Color.green : Color.gray;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}