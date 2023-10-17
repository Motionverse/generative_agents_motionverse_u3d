using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Commander : MonoBehaviour {

	public Camera mainCamera;
	public RectTransform selectionRect;
	public List<Tank> ownedTanks = new List<Tank> ();
	public List<Tank> selected = new List<Tank>();

	bool selecting;

	void Start () {
		selectionRect.gameObject.SetActive (false);
	}

	Vector3 startingPosition;
	void Update () {

		//---------------------------
		//Checking Dead
		//---------------------------
		for (int i = 0; i < ownedTanks.Count; i++) {
			if (!ownedTanks [i])
				ownedTanks.RemoveAt (i);
		}
		for (int i = 0; i < selected.Count; i++) {
			if (!selected [i])
				selected.RemoveAt (i);
		}

		//---------------------------
		//Selection Check
		//---------------------------
		if (Input.GetMouseButtonDown (0)) {
			selecting = true;

			startingPosition = Input.mousePosition;
			selectionRect.anchoredPosition3D = startingPosition;
		}

		selectionRect.gameObject.SetActive (selecting);

		Vector3 dist = Input.mousePosition - startingPosition;
		Rect screenSpaceSelectionArea = new Rect (startingPosition.x, startingPosition.y, dist.x, dist.y);

		selectionRect.sizeDelta = new Vector2 (Mathf.Abs(screenSpaceSelectionArea.width), Mathf.Abs(screenSpaceSelectionArea.height));
		selectionRect.localScale = new Vector3 (screenSpaceSelectionArea.width > 0f ? 1 : -1, screenSpaceSelectionArea.height > 0f ? 1 : -1, 1);

		if (Input.GetMouseButtonUp (0)) {
			selecting = false;

			Vector3 worldSpaceStarting = mainCamera.ScreenToWorldPoint (startingPosition);
			Vector3 worldSpaceEnding = mainCamera.ScreenToWorldPoint (Input.mousePosition);
			Vector3 worldSpaceDist = worldSpaceEnding - worldSpaceStarting;

			Rect worldSpaceSelectionArea = new Rect (worldSpaceStarting.x, worldSpaceStarting.y, worldSpaceDist.x, worldSpaceDist.y);

			List<Tank> preselected = new List<Tank> ();

			foreach (Tank tank in ownedTanks) {
				if (tank && worldSpaceSelectionArea.Contains(tank.transform.position,true)) {
					preselected.Add (tank);
				}
			}

			if (Input.GetKey (KeyCode.LeftShift)) {
				foreach (Tank tank in preselected) {
					if (!selected.Contains (tank)) {
						selected.Add (tank);
						tank.OnSelectedByCommander ();
					}
				}
			} else {
				foreach (Tank tank in selected) tank.OnDeselectedByCommander ();
				selected = preselected;
				foreach (Tank tank in preselected) tank.OnSelectedByCommander ();
			}
		}

		//---------------------------
		//Ordering
		//---------------------------
		if (Input.GetMouseButtonDown (1) && selected.Count > 0) {

			Vector3 centerPoint = selected[0].transform.position;
			if(selected.Count > 1) 
				for(int i=1;i<selected.Count;i++) 
					centerPoint = new Vector2 ((centerPoint.x + selected [i].transform.position.x) / 2, (centerPoint.y + selected [i].transform.position.y) / 2);

			Vector3 targetPosition = mainCamera.ScreenToWorldPoint (Input.mousePosition);
			targetPosition.z = 0;

			foreach (Tank tank in selected) {
				Vector3 offsetPosition = tank.transform.position - centerPoint;
				tank.SetMovementOrder (targetPosition + offsetPosition);
			}
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			foreach (Tank tank in selected) {
				tank.SetAttack ();
			}
		}
	}
}
