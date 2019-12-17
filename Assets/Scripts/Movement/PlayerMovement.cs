using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private InGameInputHandler _inputHandler;

	[SerializeField]
	private IMoving _movement;

	private Entity _entity;

	private void Awake()
	{
		_entity = GetComponent<Entity>();
        _movement = _entity.BaseMovement;
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
		if (_movement.CanMove(_entity.Map, _entity, direction))
		{
			//if (_entity.GetBlockingEntity())
			//_entity.Map.SetCellProperties(_entity.CurrentPosition, _entity.EntityTileData.InitialCellData);
            _movement.HandleMovement(_entity.Map, _entity, direction);
			//_entity.Map.SetCellProperties(_entity.CurrentPosition, _entity.EntityTileData.InitialCellData);
		}
	}
}
