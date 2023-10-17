using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TankHealthBarObject : MonoBehaviour {

	public Transform tank;
	public Vector3 localOffset;

	void Update () {
		transform.position = tank.position + localOffset;
		transform.eulerAngles = Vector3.zero;
	}
}
