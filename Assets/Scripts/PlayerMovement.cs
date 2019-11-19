using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private Transform _transform;

	public event Action OnMove;

	private void Awake() => _transform = GetComponent<Transform>();

    // Update is called once per frame
	private void Update()
	{
		HandleMovement();
    }

	private void HandleMovement()
	{
		//TODO Check if currentTurn != player
		var movement = Vector3.zero;

		//TODO Try to use Enumerator to animate and create delay in movement
		if (Input.GetKeyDown(KeyCode.D))
			movement = Vector3.right;
		else if (Input.GetKeyDown(KeyCode.A))
			movement = Vector3.left;
		else if (Input.GetKeyDown(KeyCode.W))
			movement = Vector3.forward;
		else if (Input.GetKeyDown(KeyCode.S))
			movement = Vector3.back;

		if (movement == Vector3.zero)
			return;

		_transform.position += movement;
		OnMove?.Invoke();
	}
}
