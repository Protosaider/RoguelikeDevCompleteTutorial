using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
	protected GameContext Context;
	protected GameManager GameManager;

    public virtual void SetContext(GameManager gameManager, GameContext context)
	{
		GameManager = gameManager;
		Context = context;
	}

	public virtual void OnTransitionTo() { }
	public virtual void OnTransitionFrom() { }

	public abstract void HandleUpdate((EInputAction, System.Object) inputAction);
}
