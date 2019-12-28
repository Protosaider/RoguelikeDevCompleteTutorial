using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Randomizer
{
	public class Randomizer
	{

        /// <summary>
        /// Calculate drop chance for one specific item
        /// </summary>
        /// <param name="maxItemsCountToDrop"></param>
        /// <param name="droppedItemsCount"></param>
        /// <param name="desirableAmountOfTries"></param>
        /// <param name="currentAmountOfTries"></param>
        /// <param name="earliestSuccessChanceShift">If coef > 1 - increase chance to drop on earliest tries
        /// Coef = 1 - spread uniformly
        /// Coef > 0 but less then 1 - increase chance to drop on latest tries </param>
        /// <returns></returns>
        Single CalculateCurrentDropChance(Int32 maxItemsCountToDrop, Int32 droppedItemsCount, Int32 desirableAmountOfTries, Int32 currentAmountOfTries, 
			Single earliestSuccessChanceShift)
		{
			var currentChance =
				(Single)(maxItemsCountToDrop - droppedItemsCount) / (desirableAmountOfTries - currentAmountOfTries) *
				Mathf.Pow(earliestSuccessChanceShift, maxItemsCountToDrop - droppedItemsCount);

			return currentChance;
		}

		Single CurveWeightedRandom(AnimationCurve curve) => curve.Evaluate(Random.value);

		T[] ChooseSetWithoutRepetitions<T>(int numRequired, T[] chooseFrom, bool shuffle)
		{
			T[] result = new T[numRequired];

            //TODO: if (shuffle) chooseFrom.Shuffle();

            int numToChoose = numRequired;

			for (int numLeft = chooseFrom.Length; numLeft > 0; numLeft--)
			{
				float prob = (float)numToChoose / (float)numLeft;

				if (Random.value <= prob)
				{
					numToChoose--;
					result[numToChoose] = chooseFrom[numLeft - 1];

					if (numToChoose == 0)
						break;
				}
			}
			return result;
		}

		T Choose<T>(float[] probabilities, T[] chooseFrom)
		{
			float total = 0;

			foreach (float elem in probabilities)
				total += elem;

			float randomPoint = Random.value * total;

			for (int i = 0; i < probabilities.Length; i++)
			{
				if (randomPoint < probabilities[i])
					return chooseFrom[i];

				randomPoint -= probabilities[i];
			}
			return chooseFrom[chooseFrom.Length - 1];
		}
	}


	public class CostOfGroup<T>
	{
		private List<Single> _itemDropProbabilities;
		private List<Single> _itemCosts;
		private List<T> _items;

        Single GetCost()
		{
			var cost = 0f;
			for (var i = 0; i < _itemDropProbabilities.Count; i++)
				cost += _itemCosts[i] * _itemDropProbabilities[i];

			return cost;
		}
	}

	public class PseudoRandomDistributionDota
	{
        //Probability(Number of try) = ProbabilityOdd(20%) * (Number of try)
        public decimal ProbabilityOddFromProbability(decimal actualProbability)
		{
			decimal oddUpper = actualProbability;
			decimal oddLower = 0m;
			decimal oddAverage;
			decimal p1;
			decimal p2 = 1m;
			while (true)
			{
				oddAverage = (oddUpper + oddLower) / 2m;
				p1 = ProbabilityFromProbabilityOdd(oddAverage);
				if (Math.Abs(p1 - p2) <= 0m)
					break;

				if (p1 > actualProbability)
					oddUpper = oddAverage;
				else
					oddLower = oddAverage;

				p2 = p1;
			}

			return oddAverage;
		}

        private decimal ProbabilityFromProbabilityOdd(decimal odd)
        {
            decimal pProcOnN = 0m;
            decimal pProcByN = 0m;
            decimal sumNpProcOnN = 0m;

            int maxFails = (int)Math.Ceiling(1m / odd);
            for (int N = 1; N <= maxFails; ++N)
            {
                pProcOnN = Math.Min(1m, N * odd) * (1m - pProcByN);
                pProcByN += pProcOnN;
                sumNpProcOnN += N * pProcOnN;
            }

            return 1m / sumNpProcOnN;
        }
    }

    public class WeightedChooser<T>
	{
		private List<Single> _itemWeights;
		private List<T> _items;
		public Single _allItemsWeight;

		private List<Single> _itemDropProbabilities;
		private List<Single> _itemCosts;
		private Single _allItemsCost;

        public WeightedChooser(Single[] itemWeights, T[] items)
		{
			_itemWeights = new List<Single>(itemWeights);
			_itemDropProbabilities = new List<Single>(itemWeights.Length);
            _items = new List<T>(items);

			foreach (var itemWeight in itemWeights)
				_allItemsWeight += itemWeight;

			foreach (var itemWeight in _itemWeights)
				_itemDropProbabilities.Add(UnityMath.Remap(itemWeight, 0, _allItemsWeight, 0, 1));
        }

		Single GetActualCost()
		{
			var cost = 0f;
			for (var i = 0; i < _itemDropProbabilities.Count; i++)
				cost += _itemCosts[i] * _itemDropProbabilities[i];

			return cost;
		}

        public Single GetAverageCost() => _allItemsCost / _items.Count;


		private void RecalculateDropProbabilities(Single newWeight)
		{
			for (var i = 0; i < _itemDropProbabilities.Count; i++)
			{
				var itemWeight = _itemDropProbabilities[i];
				_itemDropProbabilities[i] = UnityMath.Remap(itemWeight, 0, _allItemsWeight, 0, 1);
			}

			_itemDropProbabilities.Add(UnityMath.Remap(newWeight, 0, _allItemsWeight, 0, 1));
        }

		public void AddItem(Single weight, T item)
		{
			_allItemsWeight += weight;
			_itemWeights.Add(weight);
			_items.Add(item);
			RecalculateDropProbabilities(weight);
		}

		public T ChooseWeighted()
		{
			var randomWeight = Random.Range(0, _allItemsWeight);

			var currentWeight = 0f;

            for (var i = 0; i < _itemWeights.Count; i++)
			{
				currentWeight += _itemWeights[i];

				if (randomWeight < currentWeight)
					return _items[i];
			}

			return _items[_items.Count - 1];
		}
    }
}
