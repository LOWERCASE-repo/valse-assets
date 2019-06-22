using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Kruskal : MonoBehaviour {

  [SerializeField]
  private float spawnTime;
  [SerializeField]
  private int nodeCount;
  [SerializeField]
  private int fieldSize;
  [SerializeField]
  private GameObject debugNode;
  
  private HashSet<NavNode> nodes;
  private HashSet<NavNode> linkedNodes;
  
  private HashSet<NavNode> uncheckedNodes;
  
  private IEnumerator SpawnNodes(float time, int count) {
    yield return new WaitForSeconds(time);
    Vector2 pos = Random.insideUnitCircle * fieldSize;
    nodes.Add(new NavNode(pos));
    Instantiate(debugNode, pos, Quaternion.identity, transform);
    count--;
    if (count > 0) {
      StartCoroutine(SpawnNodes(time, count));
    } else {
      StartCoroutine(LinkNodes());
    }
  }
  
  private IEnumerator LinkNodes() {
    HashSet<NavNode> potNodes = new HashSet<NavNode>();
    foreach (NavNode node in linkedNodes) {
      potNodes.Add(FindNearest(node, nodes));
    }
    
    yield return null;
  }
  
  private NavNode FindNearest(NavNode node, HashSet<NavNode> nodes) {
    return nodes
			.OrderBy(i => (node.pos - i.pos).sqrMagnitude)
			.FirstOrDefault();
  }
  
  private void Awake() {
    nodes = new HashSet<NavNode>();
    linkedNodes = new HashSet<NavNode>();
  }
  
  private void Start() {
    linkedNodes.Add(new NavNode(Vector2.zero));
    Instantiate(debugNode, Vector2.zero, Quaternion.identity, transform);
    StartCoroutine(SpawnNodes(spawnTime / nodeCount, nodeCount - 1));
  }
}
