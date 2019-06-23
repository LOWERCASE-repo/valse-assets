using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
delaunay triangulation subgraphs
gabriel graph
urquhart graph
euclidean minimum spanning tree
rel neighbor
nearest neighbor
*/

public class Triangle {

  public Vector2[] verts;
  
  private float Sign (Vector2 p1, Vector2 p2, Vector2 p3) {
    return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
  }
  
  public bool Overlap(Vector2 point) {
    float d1, d2, d3;
    bool hasNeg, hasPos;
    
    d1 = Sign(point, verts[0], verts[1]);
    d2 = Sign(point, verts[1], verts[2]);
    d3 = Sign(point, verts[2], verts[0]);
    
    hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
    hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
    
    return !(hasNeg && hasPos);
  }
  
  public bool OverlapCircle(Vector2 point) {
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
  
  private Vector2 center;
  private int CompareClockwise(Vector2 a, Vector2 b) {
    if (a.x - center.x >= 0f && b.x - center.x < 0f)
        return -1;
    if (a.x - center.x < 0f && b.x - center.x >= 0f)
        return 1;
    Debug.Log("clock");
    Debug.Log(a);
    Debug.Log(b);
    Debug.Log((a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y));
    return (int)((a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y));
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
    center = Vector2.zero;
    foreach (Vector2 vert in verts) {
      center += vert;
    }
    center /= verts.Length;
    Array.Sort(verts, CompareClockwise);
  }
}
