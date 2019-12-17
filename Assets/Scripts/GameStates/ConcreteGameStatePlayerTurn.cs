using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteGameStatePlayerTurn : GameState
{
	public override void HandleUpdate()
	{
        Debug.Log("ConcreteGameStateMainMenu handles request.");
        //Debug.Log("ConcreteGameStateMainMenu wants to change the state of the context.");
        Context.TransitionTo(EGameState.ConcreteGameStateEnemyTurn);
    }
}
