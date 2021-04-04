using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void TakeDamage(int damage);
    void Paralyzed();
    void Burned();
    void Sleep();
    void Poisoned();
    void Freezed();
}
