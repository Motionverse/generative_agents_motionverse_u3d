using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUIController : MonoBehaviour {

	public Transform follow;
	public Transform fixedpoint;

	public void Dismiss() {
		gameObject.SetActive(false);
	}

	public void Follow() {
		ISRTSCamera.FollowForMain(follow);
	}

	public void FixedPoint() {
		ISRTSCamera.LockFixedPointForMain(fixedpoint);
	}
}
