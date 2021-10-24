using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Util
{
    public static Color randColor()
    {
        float r = Random.Range(0.1f, 0.99f);
        float g = Random.Range(0.1f, 0.99f);
        float b = Random.Range(0.1f, 0.99f);

        return new Color(r, g, b);
    }
    public static Color lerp(Color c0, Color c1, float t)
    {
        float r = c0.r * (1 - t) + c1.r * t;
        float g = c0.g * (1 - t) + c1.g * t;
        float b = c0.b * (1 - t) + c1.b * t;
        return new Color(r, g, b);
    }
    public static Vector3 vectorFromColor(Color c)
    {
        return new Vector3(c.r, c.g, c.b);
    }
}

public static class Vector3Extensions
{
    public static Vector2 xy(this Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    public static Vector3 xz(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 flippedXY(this Vector3 v)
    {
        return new Vector3(v.y, v.x, v.z);
    }
    public static void setXY(this Vector3 v, Vector2 v2)
    {
        v.x = v2.x;
        v.y = v2.y;
    }
    public static Vector3 fromValue(this Vector3 v, float val)
    {
        return new Vector3(val, val, val);
    }
    public static Vector3 fromZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }
    public static Vector3 fromVec2(this Vector3 v3, Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, v3.z);
    }
    public static Vector3 add(this Vector3 v3, float v)
    {
        return new Vector3(v3.x + v, v3.y + v, v3.z + v);
    }
    public static Vector3 addX(this Vector3 v3, float x)
    {
        return new Vector3(v3.x + x, v3.y, v3.z);
    }
    public static Vector4 toVec4(this Vector3 v, float w)
    {
        return new Vector4(v.x, v.y, v.z, w);
    }
    public static Color toColor(this Vector3 v, float a)
    {
        return new Color(v.x, v.y, v.z, a);
    }
}

public static class TransformExtensions
{
    public static void setPositionXY(this Transform transform, Vector2 v2)
    {
        transform.position = transform.position.fromVec2(v2);
    }
    public static void setPositionZ(this Transform transform, float z)
    {
        transform.position = transform.position.fromZ(z);
    }
    
}

public static class ColorExtensions
{ 
    public static Color zeroAlpha(this Color c)
    {
        return new Color(c.r, c.g, c.b, 0);
    }
    public static float difference(this Color c, Color c2)
    {
        return Vector3.Distance(Util.vectorFromColor(c), Util.vectorFromColor(c2));
    }
    public static Vector3 toVec3(this Color c)
    {
        return new Vector3(c.r, c.g, c.b);
    }
}

