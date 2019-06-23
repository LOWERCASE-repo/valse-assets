using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Kruskal : MonoBehaviour {

  [SerializeField]
  private float spawnTime;
  [SerializeField]
  private int vertCount;
  [SerializeField]
  private int fieldSize;
  [SerializeField]
  private GameObject debugNode;
  [SerializeField]
  private GameObject debugEdge;
  
  private List<GameObject> debugEdges;
  private HashSet<Vector2> verts;
  private HashSet<Triangle> triangles;
  
  /*
  verts as a list
  for every ith, jth, kth node,
  check if it forms a delaunay triangle
  if it does, triple link
  
  custom triangle class
  containsPoint using determinant
  
  for each Vector2:
    if in triangle, break and reform
    else, connect to two max nearby
  
  once you have triangles, link all verts
  */
  
  private Vector2 center;
  private int CompareClockwise(Vector2 a, Vector2 b) {
    if (a.x - center.x >= 0 && b.x - center.x < 0)
        return -1;
    if (a.x - center.x < 0 && b.x - center.x >= 0)
        return 1;
    Debug.Log("clock");
    Debug.Log(a);
    Debug.Log(b);
    Debug.Log((a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y));
    return (int)((a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y));
  }
  
  HashSet<Triangle> rems = new HashSet<Triangle>();
  List<Vector2> poly = new List<Vector2>();
  
  private IEnumerator SpawnNodes(float time, int count) {
    yield return new WaitForSeconds(time);
    
    rems.Clear();
    poly.Clear();
    
    Vector2 pos = Random.insideUnitCircle * fieldSize;
    Instantiate(debugNode, pos, Quaternion.identity, transform);
    
    foreach (Triangle triangle in triangles) {
      if (triangle.CircleContains(pos)) {
        foreach (Vector2 vert in triangle.verts) {
          if (!poly.Contains(vert)) {
            poly.Add(vert);
          }
        }
        rems.Add(triangle);
      }
    }
    
    center = Vector2.zero;
    foreach (Vector2 vert in poly) {
      center += vert;
    }
    center /= poly.Count;
    poly.Sort(CompareClockwise);
    Debug.Log("sorted:");
    for (int i = 0; i < poly.Count; i++) {
      Debug.Log(poly[i]);
    }
    
    for (int i = 0; i < poly.Count; i++) { // WRONG sort them clockwise you foole
      if (i + 1 < poly.Count) {
        triangles.Add(new Triangle(pos, poly[i], poly[i + 1]));
      } else {
        triangles.Add(new Triangle(pos, poly[i], poly[0]));
      }
    }
    
    foreach (Triangle triangle in rems) {
      triangles.Remove(triangle);
    }
    
    verts.Add(pos);
    
    DebugEdges();
    
    if (--count > 0) {
      StartCoroutine(SpawnNodes(time, count));
    } else {
      StartCoroutine(LinkNodes());
    }
  }
  
  private void DebugEdges() {
    debugEdges.Clear();
    foreach (Triangle triangle in triangles) {
      for (int i = 0; i < triangle.verts.Length; i++) {
        for (int j = i + 1; j < triangle.verts.Length; j++) {
          GameObject edge = Instantiate(debugEdge, triangle.verts[i] + triangle.verts[j], Quaternion.identity, transform);
          LineRenderer lr = edge.GetComponent<LineRenderer>();
          lr.SetPositions(new Vector3[] { triangle.verts[i], triangle.verts[j] });
        }
      }
    }
  }
  
  private IEnumerator LinkNodes() {
    foreach (Triangle triangle in triangles) {
      Debug.Log("boo");
    }
    yield return null;
  }
  
  private void Awake() {
    verts = new HashSet<Vector2>();
    triangles = new HashSet<Triangle>();
    debugEdges = new List<GameObject>();
  }
  
  private void Start() {
    float xPos = fieldSize * Mathf.Sqrt(3f);
    Vector2 up = new Vector2(0f, fieldSize * 2f);
    Vector2 left = new Vector2(xPos, -fieldSize);
    Vector2 right = new Vector2(-xPos, -fieldSize);
    verts.Add(up);
    verts.Add(left);
    verts.Add(right);
    verts.Add(Vector2.zero);
    triangles.Add(new Triangle(up, right, Vector2.zero));
    triangles.Add(new Triangle(right, left, Vector2.zero));
    triangles.Add(new Triangle(left, up, Vector2.zero));
    Instantiate(debugNode, Vector2.zero, Quaternion.identity, transform);
    StartCoroutine(SpawnNodes(spawnTime / vertCount, vertCount - 1));
  }
}
