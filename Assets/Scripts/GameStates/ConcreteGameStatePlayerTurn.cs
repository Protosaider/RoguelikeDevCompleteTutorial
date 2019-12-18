using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteGameStatePlayerTurn : GameState
{
	public override void OnTransitionFrom()
	{
		base.OnTransitionFrom();
	}

	public override void HandleUpdate((EInputAction, System.Object) inputAction)
	{
		Debug.Log("ConcreteGameStateMainMenu handles request.");

		if (inputAction.Item1 == EInputAction.Wait)
		{
			Debug.Log("Waiting...");
			Context.TransitionTo(EGameState.ConcreteGameStateEnemyTurn);
            return;
		}

        if (inputAction.Item1 == EInputAction.MoveRight ||
			inputAction.Item1 == EInputAction.MoveLeft ||
			inputAction.Item1 == EInputAction.MoveUp ||
			inputAction.Item1 == EInputAction.MoveDown
			)
		{
			var inputActionData = (EDirection)inputAction.Item2;
			var playerEntity = GameManager._entitiesHolder.PlayerEntity;

			if (GameManager._world.CurrentMap.CanMove(playerEntity.CurrentPosition, inputActionData))
			{
				var moveTo = playerEntity.CurrentPosition + inputActionData.ToVector2Int();

                foreach (var entity in GameManager._entitiesHolder)
				{
					if (entity.CurrentPosition == moveTo && !entity.EntityTileData.InitialCellData.IsWalkable)
					{
						//revert
						Debug.Log("Can't move: Entity blocks");
						return;
					}
				}

				playerEntity.BaseMovement.HandleMovement(inputActionData);

				//TODO Switch input handler to enemy turn input handler
				Context.TransitionTo(EGameState.ConcreteGameStateEnemyTurn);
            }
			else
			{
				Debug.Log("Can't move: map cell blocked");
            }
		}
    }
}
