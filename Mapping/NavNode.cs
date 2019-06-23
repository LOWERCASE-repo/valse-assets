using UnityEngine;
using System;
using System.Collections.Generic;

public class NavNode {

// nav map?

  public Vector2 pos;
  public Dictionary<NavNode, float> links;
  
  public void Link(NavNode node) {
    float dist = (pos - node.pos).magnitude;
    links.Add(node, dist);
    node.links.Add(this, dist);
  }
  
  public void Unlink(NavNode node) {
    links.Remove(node);
    node.links.Remove(this);
  }
  
  // public override bool Equals(System.Object obj) {
  //   if (obj == null || !this.GetType().Equals(obj.GetType())) {
  //     return false;
  //   } else { 
  //     NavNode other = (NavNode)obj;
  //     return pos.Equals(other.pos);
  //   }   
  // }
  
  public NavNode(Vector2 pos) {
    this.pos = pos;
    links = new Dictionary<NavNode, float>();
  }
}
