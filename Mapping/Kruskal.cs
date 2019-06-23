using UnityEngine;
// using System;
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
  
  private NavNode start;
  private HashSet<NavNode> nodes;
  
  private IEnumerator SpawnNodes(float time, int count) {
    yield return new WaitForSeconds(time);
    
    Vector2 pos = Random.insideUnitCircle * fieldSize;
    Instantiate(debugNode, pos, Quaternion.identity, transform);
    nodes.Add(new NavNode(pos));
    
    if (--count > 0) {
      StartCoroutine(SpawnNodes(time, count));
    } else {
      yield return new WaitForSeconds(0.2f);
      StartCoroutine(LinkNodes(start));
    }
  }
  
  private bool CheckNeighbor(Vector2 pos1, Vector2 pos2) {
    Vector2 center = (pos1 + pos2) / 2f;
    NavNode closest = nodes.OrderBy(i => (center - i.pos).magnitude).FirstOrDefault();
    if (closest.pos.Equals(pos1) || closest.pos.Equals(pos2)) {
      GameObject edge = Instantiate(debugEdge, center, Quaternion.identity, transform);
      LineRenderer lr = edge.GetComponent<LineRenderer>();
      lr.SetPositions(new Vector3[] { pos1, pos2 });
      return true;
    }
    return false;
  }
  
  private IEnumerator LinkNodes(NavNode start) {
    foreach (NavNode node in nodes) {
      if (!node.Equals(start) && !start.links.ContainsKey(node) && CheckNeighbor(start.pos, node.pos)) {
        yield return new WaitForSeconds(0.2f);
        start.Link(node);
        yield return LinkNodes(node);
      }
    }
  }
  
  private void Awake() {
    nodes = new HashSet<NavNode>();
  }
  
  private void Start() {
    start = new NavNode(Vector2.zero);
    nodes.Add(start);
    StartCoroutine(SpawnNodes(spawnTime / vertCount, vertCount - 1));
  }
}
