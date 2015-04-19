using UnityEngine;
using System.Collections;

public class DropBin : MonoBehaviour {

	public BoxCollider2D m_box;

	// Use this for initialization
	void Start () {
		m_box = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
