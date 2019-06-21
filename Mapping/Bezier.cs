using UnityEngine;
using System;
using System.Collections;

public class Bezier {

  public Vector2 start;
  public Vector2 end;
  public Vector2[] pivots;

  public Bezier(Vector2[] pivots) {
    this.pivots = pivots;
    start = pivots[0];
    end = pivots[pivots.Length - 1];
  }
  
  public void Rotate(float ang) {
    for (int i = 1; i < pivots.Length; ++i) {
      pivots[i] = pivots[i] - start;
      pivots[i] = pivots[i].Rotate(ang);
      pivots[i] = pivots[i] + start;
    }
  }

  public Vector2 Eval(double time) {
    return EvalRec(time, pivots);
  }

  private Vector2 EvalRec(double time, Vector2[] pivots) {
    if (pivots.Length == 1) {
      return pivots[0];
    }

    Vector2[] cPivots = new Vector2[pivots.Length - 1];
    Array.Copy(pivots, 0, cPivots, 0, pivots.Length - 1);
    Vector2 start = EvalRec(time, cPivots);
    Array.Copy(pivots, 1, cPivots, 0, pivots.Length - 1);
    Vector3 end = EvalRec(time, cPivots);

    return Vector2.Lerp(start, end, (float)time);
  }
  
  public NavNode Graph(float width) {
    Vector2 startPos = Eval(0.0);
    Vector2 endPos = Eval(1.0);

    NavNode start = new NavNode(startPos);
    NavNode end = new NavNode(endPos);
    start.Link(end);
    
    GraphRec(width, start, end, 0.0, 1.0);

    return start;
  }

  private void GraphRec(float width, NavNode prev, NavNode next, double prevTime, double nextTime) {
    double midTime = (prevTime + nextTime) / 2.0;
    Vector2 midPos = Eval(midTime);

    NavNode mid = new NavNode(midPos);
    prev.Unlink(next);
    mid.Link(prev);
    mid.Link(next);
    
    double prevDist = (midPos - prev.pos).magnitude;
    double nextDist = (midPos - next.pos).magnitude;
    if (prevDist >= width) {
      GraphRec(width, prev, mid, prevTime, midTime);
    }
    if (nextDist >= width) {
      GraphRec(width, mid, next, midTime, nextTime);
    }
  }
}
