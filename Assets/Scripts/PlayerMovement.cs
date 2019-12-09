using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoving
{
	void HandleMovement(EDirection direction);
	Boolean CanMove();
}

public enum EDirection
{
	None,
	Up,
	Right,
	Down,
	Left,
}

public static class EDirectionHelper
{
	public static Vector3 ToVector3(this EDirection direction)
	{
		var vector = Vector3.zero;

		switch (direction) {
			case EDirection.Right:
				vector = Vector3.right;
				break;

			case EDirection.Left:
				vector = Vector3.left;
				break;

			case EDirection.Up:
				vector = Vector3.forward;
				break;

			case EDirection.Down:
				vector = Vector3.back;
				break;

			case EDirection.None:
                break;
		}

		return vector;
    }
}

public class BaseMovement : IMoving
{
    private readonly Transform _transform;

	public BaseMovement(Transform transform)
	{
		_transform = transform;
	}

	public virtual void HandleMovement(EDirection direction)
    {
        var moveTo = direction.ToVector3();
		_transform.position += moveTo;
    }

    public virtual Boolean CanMove() => true;
}

public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private InGameInputHandler _inputHandler;

	[SerializeField]
	private BaseMovement _baseMovement;

    private Transform _transform;

	private void Awake()
	{
        //_transform = GetComponent<Transform>();
        _baseMovement = new BaseMovement(GetComponent<Transform>());
    }

	private void OnEnable()
	{
		_inputHandler.OnKeyDownW += HandleMovement;
		_inputHandler.OnKeyDownA += HandleMovement;
		_inputHandler.OnKeyDownS += HandleMovement;
		_inputHandler.OnKeyDownD += HandleMovement;
    }

	private void OnDisable()
	{
		_inputHandler.OnKeyDownW -= HandleMovement;
		_inputHandler.OnKeyDownA -= HandleMovement;
		_inputHandler.OnKeyDownS -= HandleMovement;
		_inputHandler.OnKeyDownD -= HandleMovement;
	}

	//TODO Check if currentTurn != player
    public void HandleMovement(EDirection direction)
	{
		Debug.Log("Handled Player Movement");
		_baseMovement.HandleMovement(direction);
    }

    public Boolean CanMove()
	{
		return _baseMovement.CanMove();
    }
}
