using UnityEngine;

// Это интерфейс - контракт, который говорит: 
// "любой класс, использующий меня, обязан иметь метод TakeDamage"
public interface IDamageable
{
    void TakeDamage(int damage);
}