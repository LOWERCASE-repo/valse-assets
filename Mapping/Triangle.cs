using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Triangle {

  public Vector2[] verts;
  
  public bool CircleContains(Vector2 point) {
    Matrix4x4 mat = new Matrix4x4();
    Vector4 row;
    for (int i = 0; i < verts.Length; i++) {
      row = new Vector4(
        verts[i].x * verts[i].x + verts[i].y * verts[i].y,
        verts[i].x,
        verts[i].y,
        1
      );
      mat.SetRow(i, row);
    }
    row = new Vector4(
      point.x * point.x + point.y * point.y,
      point.x,
      point.y,
      1
    );
    mat.SetRow(3, row);
    return (mat.determinant < -Mathf.Epsilon);
  }
  
  private void SortVerts() { // flc, mat this shit
    float det = (verts[1].y - verts[0].y) * (verts[2].x - verts[1].x)
              - (verts[1].x - verts[0].x) * (verts[2].y - verts[1].y);
    if (det > 0) {
      Vector2 swap = verts[0];
      verts[0] = verts[1];
      verts[1] = swap;
      Debug.Log("sanotehusnotah");
    }
  }
  
  public override bool Equals(System.Object obj) {
    if (obj == null || !this.GetType().Equals(obj.GetType())) {
      return false;
    } else { 
      Triangle other = (Triangle)obj;
      return (Enumerable.Count(verts.Intersect(other.verts)) == verts.Length);
    }   
  }
  
  public Triangle(Vector2 vert1, Vector2 vert2, Vector2 vert3) {
    this.verts = new Vector2[] { vert1, vert2, vert3 };
    SortVerts();
  }
}
