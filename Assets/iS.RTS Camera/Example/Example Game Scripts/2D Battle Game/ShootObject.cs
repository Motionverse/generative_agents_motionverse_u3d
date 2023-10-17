using UnityEngine;
using System.Collections;

public class ShootObject : MonoBehaviour {

	public float moveSpeed;
	public float damageRange;
	public float damage;

	LayerMask targetMask;

	void Update () {
		transform.Translate (Vector3.up * moveSpeed * Time.deltaTime,Space.Self);

		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, damageRange, targetMask);
		if (cols.Length > 0) {
			foreach (Collider2D col in cols) {
				col.GetComponent<Tank> ().ApplyDamage (damage);
			}
			Destroy (gameObject);
		}
	}

	public void Setup(LayerMask layerMask){
		targetMask = layerMask;
	}
}
