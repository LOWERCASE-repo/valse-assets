using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaTest : MonoBehaviour {
  private SpriteRenderer sr;
  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
  }
  private void Update() {
    Debug.Log("hi");
    sr.material.SetVector("_PlayerPos", Pool.player.rb.position);
  }
}
