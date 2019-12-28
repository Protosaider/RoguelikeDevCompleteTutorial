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

				GameManager._fieldOfView.Clear(playerEntity.CurrentPosition, playerEntity.FovRadius);
				GameManager._fieldOfView.Recompute(GameManager._world.CurrentMap, playerEntity.CurrentPosition, playerEntity.FovRadius);

				GameManager._mapRenderer.RefreshVisibility(GameManager._fieldOfView.FovMap, GameManager._fieldOfView.LightMap);

                //TODO: if entity on tile in fov => show entity
				foreach (var entity in GameManager._entitiesHolder)
				{
					entity.BaseRenderer.FovTest(
						GameManager._fieldOfView.FovMap.GetItem(entity.CurrentPosition.x, entity.CurrentPosition.y),
						GameManager._fieldOfView.LightMap.GetItem(entity.CurrentPosition.x, entity.CurrentPosition.y)
					);
				}

				//TODO Switch input handler to enemy turn input handler?
                Context.TransitionTo(EGameState.ConcreteGameStateEnemyTurn);
            }
			else
			{
				Debug.Log("Can't move: map cell blocked");
            }
		}
    }
}
