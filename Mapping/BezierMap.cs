using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BezierMap {

  public Bezier trunk; // multiple cubic beziers? curvier
  public NavNode start;

  public BezierMap(Map map) {
    trunk = CreatePath(
      Vector2.zero, 
      map.trunkPivotCount, 
      map.trunkLength, 
      Random.value * Mathf.PI * 2f
    );
    trunk.end = trunk.Eval(1.0);
    start = trunk.Graph(map.trunkWidth);
  }

  private Bezier CreatePath(Vector2 start, int pivotCount, float length, float ang) {
    Vector2[] pivots = new Vector2[pivotCount];

    pivots[0] = start;
    pivots[pivotCount - 1] = start + Vector2.up * length;
    Vector2 mid = (pivots[0] + pivots[pivotCount - 1]) / 2f;
    float radius = length / 2f;

    for (int i = 1; i < pivotCount - 1; ++i) {
      pivots[i] = mid + Random.insideUnitCircle * radius;
    }
    pivots.OrderBy(i => i.y);

    Bezier bez = new Bezier(pivots);
    bez.Rotate(ang);

    return bez;
  }
}
