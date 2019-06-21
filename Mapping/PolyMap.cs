using UnityEngine;
using System.Collections.Generic;

public class PolyMap : MonoBehaviour {
  
  public NavNode start;
  private EdgeCollider2D ec;
  private LineRenderer lr;
  
  private HashSet<NavNode> mappedNodes;
  private List<Vector2> leftPoints;
  private List<Vector2> rightPoints;
  
  private void Awake() {
    ec = GetComponent<EdgeCollider2D>();
    lr = GetComponent<LineRenderer>();
    
    mappedNodes = new HashSet<NavNode>();
    leftPoints = new List<Vector2>();
    rightPoints = new List<Vector2>();
  }
  
  public void MapNodes(NavNode start) {
    mappedNodes.Clear();
    leftPoints.Clear();
    rightPoints.Clear();
    
    Vector2 prevPos = Vector2.zero;
    foreach (KeyValuePair<NavNode, float> link in start.links) {
      MapNodesRec(start.pos, link.Key);
      prevPos = link.Key.pos;
    }
    
    rightPoints.Reverse();
    rightPoints.Add(start.pos * 2f - prevPos);
    
    Vector2[] pointsArr = new Vector2[leftPoints.Count + rightPoints.Count + 1];
    leftPoints.CopyTo(pointsArr);
    rightPoints.CopyTo(pointsArr, leftPoints.Count);
    pointsArr[pointsArr.Length - 1] = leftPoints[0];
    ec.points = pointsArr;
    Vector3[] posArr = new Vector3[pointsArr.Length];
    for (int i = 0; i < pointsArr.Length; ++i) {
      posArr[i] = (Vector3)pointsArr[i];
    }
    
    lr.positionCount = posArr.Length;
    lr.SetPositions(posArr);
  }
  
  private void MapNodesRec(Vector2 prevPos, NavNode node) {
    mappedNodes.Add(node);
    Vector2 currPos = node.pos;
    
    bool ending = true;
    foreach (KeyValuePair<NavNode, float> link in node.links) {
      Vector2 nextPos = link.Key.pos;
      if (prevPos.Equals(nextPos)) {
        continue;
      }
      ending = false;
      
      Vector2 perp = Vector2.Perpendicular((nextPos - prevPos) / 2f);
      leftPoints.Add(currPos + perp);
      rightPoints.Add(currPos - perp);
      
      if (!mappedNodes.Contains(link.Key)) {
        MapNodesRec(currPos, link.Key);
      }
    }
    
    if (ending) {
      leftPoints.Add(currPos * 2f - prevPos);
    }
  }
}
