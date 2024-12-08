using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRinoBehaviour : EnemyBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        float contactAngle = Vector2.Angle(collision.contacts[0].normal, Vector2.up);
        float damageAngleRange = 10;
        float minAngle = (180 - damageAngleRange) / 2;
        float maxAngle = 90 + damageAngleRange / 2;
        bool dealDamage = minAngle < contactAngle && contactAngle < maxAngle;
        GameObject other = collision.gameObject;
        IDamgable damgable;
        if (dealDamage)
        {
            if (other.TryGetComponent<IDamgable>(out damgable))
            {
                if (other.tag == "Player")
                {
                    damgable.Hit(damage);
                }
            }
        }
    }
}
