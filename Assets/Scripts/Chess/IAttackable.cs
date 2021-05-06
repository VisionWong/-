using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    void TakeDamage(int damage, DamageType type, Direction dir, Action callback = null);
    void Paralyzed();
    void Burned();
    void Sleep();
    void Poisoned(float bloodPer);
    void Freezed();
    void Confused();
    void Dead();
}
