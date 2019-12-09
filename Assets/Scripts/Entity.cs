using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	[SerializeField]
	private BaseMovement _baseMovement;

	private void Awake()
	{
		_baseMovement = new BaseMovement(GetComponent<Transform>());
	}

	public void HandleMovement(EDirection direction) => _baseMovement.HandleMovement(direction);
	public Boolean CanMove() => _baseMovement.CanMove();
}
