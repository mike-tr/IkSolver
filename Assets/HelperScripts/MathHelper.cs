using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathHelper
{

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static float AngleBetween(Vector2 A, Vector2 B)
    {
        A -= B;
        float angle = Mathf.Atan2(A.y, A.x);
        return angle * Mathf.Rad2Deg;
    }

    public static float AngleFromDirection(Vector2 dir)
    {
        //Get the angle by using some Acos on x, flipping the rotation when y is lower then 0.
        dir = dir.normalized;
        var angle = Mathf.Acos(dir.x) * Mathf.Rad2Deg;
        return dir.y > 0 ? angle : 360 - angle;
    }

    public static Vector2 RadianToVector2(float radian, float length)
    {
        return RadianToVector2(radian) * length;
    }
    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }
    public static Vector2 DegreeToVector2(float degree, float length)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad) * length;
    }

    public static Vector3 DirectionToRotation(Vector3 direction)
    {
        Vector3 rotation = Vector3.zero;
        rotation.x = Angle(direction.z, direction.y);
        rotation.y = -Angle(direction.x, direction.z);
        rotation.z = -Angle(direction.x, direction.y);
        return rotation;
    }

    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }

    public static float Angle(float x, float y)
    {
        if (x < 0)
        {
            return 360 - (Mathf.Atan2(x, y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(x, y) * Mathf.Rad2Deg;
        }
    }

    public static float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    public static float FakeDSigmoid(float y)
    {
        //return Sigmoid(x) * (1 - Sigmoid(x));
        return y * (1 - y);
    }

}
