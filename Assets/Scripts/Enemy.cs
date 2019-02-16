using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

	public enum State { Idle, Chasing, Attacking };

	State currentState;

	NavMeshAgent pathFinder;
	Transform target;
	LivingEntity targetEntity;

	Material skinMaterial;
	Color originalColor;
	float attackDistanceThreshold = 1.5f;
	float timeBetweenAttacks = 1;
	float damage = 1;

	float nextAttackTime;

	float myCollisionRadius;
	float targetCollisionRadius;

	bool hasTarget;

	// Use this for initialization
	protected override void Start () {
		base.Start ();

		pathFinder = GetComponent<NavMeshAgent> ();

		skinMaterial = GetComponent<Renderer> ().material;
		originalColor = skinMaterial.color;

		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			currentState = State.Chasing;

			hasTarget = true;

			target = GameObject.FindGameObjectWithTag ("Player").transform;
			targetEntity = target.GetComponent<LivingEntity> ();
			targetEntity.OnDeath += OnTargetDeath;

			myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;

			StartCoroutine (UpdatePath ());
		}
	}

	void OnTargetDeath () {
		hasTarget = false;
		currentState = State.Idle;
	}

	// Update is called once per frame
	void Update () {
		if (hasTarget) {
			if (Time.time > nextAttackTime) {
				float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
				if (sqrDstToTarget < Mathf.Pow (attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) {
					nextAttackTime = Time.time + timeBetweenAttacks;
					StartCoroutine (Attack ());
				}
			}
		}
	}

	IEnumerator Attack () {
		currentState = State.Attacking;
		pathFinder.enabled = false;

		Vector3 originalPosition = transform.position;
		Vector3 dirToTarget = (target.position - transform.position).normalized;
		Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius);

		float attackSpeed = 3f;
		float percent = 0f;

		skinMaterial.color = Color.red;
		bool hasAppliedDamage = false;

		while (percent <= 1) {

			if (percent >= .5f && !hasAppliedDamage) {
				hasAppliedDamage = true;
				targetEntity.TakeDamage (damage);
			}
			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-Mathf.Pow (percent, 2) + percent) * 4;
			transform.position = Vector3.Lerp (originalPosition, attackPosition, interpolation);

			yield return null;
		}

		skinMaterial.color = originalColor;

		pathFinder.enabled = true;
		currentState = State.Chasing;
	}

	IEnumerator UpdatePath () {
		float refreshRate = .25f;

		while (hasTarget) {
			if (currentState == State.Chasing) {
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
				if (!dead) {
					pathFinder.SetDestination (targetPosition);
				}
			}
			yield return new WaitForSeconds (refreshRate);
		}
	}
}