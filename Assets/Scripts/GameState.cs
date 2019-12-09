using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
	protected GameContext Context;

	public virtual void SetContext(GameContext context)
	{
		Context = context;
	}

	public abstract void HandleUpdate();
}
