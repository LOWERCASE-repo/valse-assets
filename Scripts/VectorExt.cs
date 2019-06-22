using UnityEngine;

public static class VectorExt {
  
  public static Vector2 Rotate(this Vector2 vec, float ang) {
    // rotate about origin
    float sin = Mathf.Sin(ang);
    float cos = Mathf.Cos(ang);
    return new Vector2(vec.x * cos - vec.y * sin, vec.y * cos + vec.x * sin);
  }
}