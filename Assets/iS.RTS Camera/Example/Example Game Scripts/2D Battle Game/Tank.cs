using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour {

	public Team team;
	public float health;
	float curHealth;
	public float enemyAttackRange;
	public float enemySearchRange;
	public float moveSpeed;
	public float rotSpeed;
	public ShootObject shootObject;
	public float weaponCooldown;

	public Transform healthBar;
	public GameObject healthBarObject;

	LayerMask enemyLayer;
	Tank currentEnemy;
	bool hasPendingOrder = false;
	Vector3 orderedPosition;
	bool isWeaponReady = true;

	void Start () {
		enemyLayer = LayerManager.GetEnemyLayer (team);

		curHealth = health;
		healthBarObject.SetActive (false);

		SetMovementOrder (transform.position + transform.up * 3f);
	}

	void FixedUpdate () {
		healthBar.localScale = new Vector3 (curHealth / health, 1, 1);

		SearchEnemy ();

		if (hasPendingOrder) {
			Quaternion wantedRot = Quaternion.LookRotation (orderedPosition - transform.position,-transform.TransformDirection(Vector3.forward));
			transform.rotation = Quaternion.Slerp (transform.rotation, wantedRot, rotSpeed * Time.deltaTime);
			transform.rotation = new Quaternion (0, 0, transform.rotation.z, transform.rotation.w);
			transform.Translate (Vector3.up * moveSpeed * Time.deltaTime,Space.Self);

			float dist2Pos = Vector3.Distance (transform.position, orderedPosition);
			if (dist2Pos <= 0.1f) hasPendingOrder = false;

		} else if (currentEnemy) {
			float dist = Vector3.Distance (transform.position, currentEnemy.transform.position);
			if (dist < enemyAttackRange) {

				Quaternion wantedRot = Quaternion.LookRotation (currentEnemy.transform.position - transform.position,-transform.TransformDirection(Vector3.forward));
				transform.rotation = Quaternion.Slerp (transform.rotation, wantedRot, rotSpeed * Time.deltaTime);
				transform.rotation = new Quaternion (0, 0, transform.rotation.z, transform.rotation.w);
				//transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.z);

				if (isWeaponReady) {

					GameObject obj = Instantiate (shootObject.gameObject, transform.position, transform.rotation) as GameObject;
					obj.GetComponent<ShootObject> ().Setup (enemyLayer);

					isWeaponReady = false;
					StartCoroutine (WeaponCooldown ());
				}
			} else {
				Quaternion wantedRot = Quaternion.LookRotation (currentEnemy.transform.position - transform.position,-transform.TransformDirection(Vector3.forward));
				transform.rotation = Quaternion.Slerp (transform.rotation, wantedRot, rotSpeed * Time.deltaTime);
				transform.rotation = new Quaternion (0, 0, transform.rotation.z, transform.rotation.w);
				//transform.eulerAngles = new Vector3 (0, 0, transform.eulerAngles.z);
				transform.Translate (Vector3.up * moveSpeed * Time.deltaTime,Space.Self);
			}
		}
			
	}

	IEnumerator WeaponCooldown(){
		yield return new WaitForSeconds(weaponCooldown);
		isWeaponReady = true;
	}

	public void SetMovementOrder(Vector3 position){
		hasPendingOrder = true;
		orderedPosition = position;
	}

	public void SetAttack(){
		hasPendingOrder = false;
	}

	void SearchEnemy(){
		Collider2D[] cols = Physics2D.OverlapCircleAll (new Vector2(transform.position.x,transform.position.y), hasPendingOrder ? enemyAttackRange : enemySearchRange, enemyLayer);
	
		if (cols.Length > 0) {
			float minDist = Mathf.Infinity;

			foreach (Collider2D col in cols) {
				float dist = Vector3.Distance (transform.position, col.transform.position);
				if (dist < minDist) {
					minDist = dist;
					currentEnemy = col.GetComponent<Tank> ();
				}
			}
		} else {
			currentEnemy = null;
		}
	}

	public void ApplyDamage(float amount){
		curHealth -= amount;
		if (curHealth <= 0f) Dead ();
	}

	public void Dead(){
		Destroy (gameObject);
	}

	public void OnSelectedByCommander(){
		healthBarObject.SetActive (true);
	}

	public void OnDeselectedByCommander(){
		healthBarObject.SetActive (false);
	}
}

public enum Team{
	Red,Blue,Pink,Green
}