using UnityEngine;
using System.Collections;

public class DropBin : MonoBehaviour {

	BoxCollider2D m_box;

	// Use this for initialization
	void Start () {
		m_box = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsOverlap() {
		Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		return m_box.OverlapPoint (wp);
	}
}
