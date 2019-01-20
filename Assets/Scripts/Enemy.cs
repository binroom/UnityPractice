using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : MonoBehaviour {

	NavMeshAgent pathFinder;
	Transform target;

	// Use this for initialization
	void Start () {
		pathFinder = GetComponent<NavMeshAgent> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;

		StartCoroutine (UpdatePath ());
	}

	// Update is called once per frame
	void Update () { }

	IEnumerator UpdatePath () {
		float refreshRate = .25f;

		while (target != null) {
			Vector3 targetPosition = new Vector3 (
				target.position.x, 0, target.position.z);
			pathFinder.SetDestination (targetPosition);
			yield return new WaitForSeconds (refreshRate);
		}
	}
}