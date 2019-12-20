using System.Collections;
using System.Collections.Generic;

public static partial class UnityMath
{
	public static float Remap(float value, float inputFrom, float inputTo, float outputFrom, float outputTo)
	{
		return (value - inputFrom) / (inputTo - inputFrom) * (outputTo - outputFrom) + outputFrom;
	}
}
