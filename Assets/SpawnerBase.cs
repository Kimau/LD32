using UnityEngine;

public abstract class SpawnerBase : MonoBehaviour {

	public GamePiece m_currPiece;
	BoxCollider2D m_box;

	// Use this for initialization
	public virtual void Start () {
		m_box = GetComponent<BoxCollider2D> ();
	}

	protected abstract void SpawnPiece ();

	
	public bool IsOverlap() {
		Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		return m_box.OverlapPoint (wp);
	}
}

