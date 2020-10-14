﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class ParallaxLayer : MonoBehaviour {
	[Range(-10f, 1f)]
	public float ParallaxSpeed = 1f;
	public Transform BaseObject;

	private Transform _transform;
	private Vector3 _offset;
	private Vector3 _newPosition;

	private void Awake() {
		_transform = GetComponent<Transform>();
		_newPosition = _transform.position;
		_offset = BaseObject.position - _newPosition;
	}

	private void Update() {
		_newPosition.x = _offset.x + BaseObject.position.x * ParallaxSpeed;
		_transform.position = _newPosition;
	}
}
