using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Randomizer {
	public class Dice
	{
		public Int32 Roll(Int32 dicesCount, Int32 facesCount, Int32 offset)
		{
			var sum = 0;

			for (var i = 0; i < dicesCount; i++)
				sum += Random.Range(0, facesCount) + 1;

			return sum - offset;
		}

		public Int32 Roll(Int32 dicesCount, Int32 facesCount, Int32 offset, out List<Int32> rolledDices)
		{
			var sum = 0;
			rolledDices = new List<Int32>(dicesCount);

			for (var i = 0; i < dicesCount; i++)
			{
				rolledDices.Add(Random.Range(0, facesCount) + 1);
				sum += rolledDices[rolledDices.Count - 1];
			}

			return sum - offset;
		}

		public Int32 Roll(Int32 dicesCount, List<Int32> facesValues, Int32 offset)
		{
			var sum = 0;

			for (var i = 0; i < dicesCount; i++)
				sum += facesValues[Random.Range(0, facesValues.Count)];

			return sum - offset;
		}

		public Int32 Roll(Int32 dicesCount, List<Int32> facesValues, Int32 offset, out List<Int32> rolledDices)
		{
			var sum = 0;
			rolledDices = new List<Int32>(dicesCount);

			for (var i = 0; i < dicesCount; i++)
			{
				rolledDices.Add(facesValues[Random.Range(0, facesValues.Count)]);
				sum += rolledDices[rolledDices.Count - 1];
			}

			return sum - offset;
		}
	}
}