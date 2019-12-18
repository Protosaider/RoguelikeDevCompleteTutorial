using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteGameStateEnemyTurn : GameState
{
	public override void HandleUpdate((EInputAction, System.Object) inputAction)
	{
		foreach (var entity in GameManager._entitiesHolder)
		{
			Debug.Log($"Entity {entity.EntityTileData.InitialTileRenderData.Name} ponders the meaning of its existence.");
		}
		Context.TransitionTo(EGameState.ConcreteGameStatePlayerTurn);
    }
}
