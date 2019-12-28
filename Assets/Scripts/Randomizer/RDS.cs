using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;


public class RandomizerTableResultEventArgs : EventArgs
{
	public RandomizerTableResultEventArgs(IReadOnlyList<IRandomizerTableItem> result) => Result = result;
	public IReadOnlyList<IRandomizerTableItem> Result { get; }
}

public interface IRandomizerTable : IRandomizerTableItem
{
    /// <summary>
    /// The maximum number of entries expected in the Result. The final count of items in the result may be lower
    /// if some of the entries may return a null result (no drop)
    /// </summary>
    Int32 ItemsToDropCount { get; set; }
    /// <summary>
    /// Gets or sets the contents of this table
    /// </summary>
	IReadOnlyList<IRandomizerTableItem> Items { get; }
    /// <summary>
    /// Gets the result. Calling this method will start the random pick process and generate the result.
    /// This result remains constant for the lifetime of this table object.
    /// Use the ResetResult method to clear the result and create a new one.
    /// </summary>
	IReadOnlyList<IRandomizerTableItem> GetResult { get; }
}


public interface IRandomizerTableItem
{
    /// <summary>
    /// Gets or sets the probability for this object to be (part of) the result
    /// </summary>
	Single Weight { get; set; }
    /// <summary>
    /// Gets or sets whether this object may be contained only once in the result set
    /// </summary>
    Boolean IsUnique { get; set; }
    /// <summary>
    /// Gets or sets whether this object will always be part of the result set
    /// (Probability is ignored when this flag is set to true)
    /// </summary>
    Boolean IsAlwaysDropped { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IRandomizerTableItem"/> is enabled.
    /// Only enabled entries can be part of the result of a RDSTable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    Boolean IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the table this Object belongs to.
    /// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
    /// </summary>
    RandomizerTable Table { get; set; }

    /// <summary>
    /// Occurs before all the probabilities of all items of the current RDSTable are summed up together.
    /// This is the moment to modify any settings immediately before a result is calculated.
    /// </summary>
    event EventHandler ResultEvaluationPreparing;
    /// <summary>
    /// Occurs when this RDSObject has been hit by the Result procedure.
    /// (This means, this object will be part of the result set).
    /// </summary>
    event EventHandler AddedToResult;
    /// <summary>
    /// Occurs after the result has been calculated and the result set is complete, but before
    /// the RDSTable's Result method exits.
    /// </summary>
    event EventHandler<RandomizerTableResultEventArgs> ResultEvaluated;

    /// <summary>
    /// Raises the <see cref="E:ResultEvaluationPreparing"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void OnResultEvaluationPreparing(EventArgs e);
    /// <summary>
    /// Raises the <see cref="E:AddedToResult"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void OnAddedToResult(EventArgs e);
    /// <summary>
    /// Raises the <see cref="E:ResultEvaluated"/> event.
    /// </summary>
    /// <param name="e">The <see cref="RandomizerTableResultEventArgs"/> instance containing the event data.</param>
    void OnResultEvaluated(RandomizerTableResultEventArgs e);


    /// <summary>
    /// Creates an instance of the object derived from <see cref="IRandomizerTableItem"/> where this method is implemented in.
    /// Override to instanciate more complex constructors.
    /// </summary>
    /// <returns>An instance of an object of the type derived from <see cref="IRandomizerTableItem"/></returns>
    IRandomizerTableItem CreateNewOrGetCurrentInstance();

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <param name="indentationlevel">The indentationlevel. 4 blanks at the beginning of each line for each level.</param>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    String ToString(Int32 indentationlevel);
}

public interface IRandomizerTableItemWithValue<T> : IRandomizerTableItem
{
    T Value { get; }
}

/// <summary>
/// Base implementation of the IRDSObject interface.
/// This class only implements the interface and provides all events required.
/// Most methods are virtual and ready to be overwritten. Unless there is a good reason,
/// do not implement IRDSObject for yourself, instead derive your base classes that shall interact
/// in *any* thinkable way as a result source with any RDSTable from this class.
/// </summary>
public class RandomizerTableItem : IRandomizerTableItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTableItem"/> class.
    /// </summary>
    public RandomizerTableItem() : this(0) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTableItem"/> class.
    /// </summary>
    /// <param name="probability">The probability.</param>
    public RandomizerTableItem(Single probability) : this(probability, false, false, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTableItem"/> class.
    /// </summary>
    /// <param name="probability">The probability.</param>
    /// <param name="unique">if set to <c>true</c> this object can only occur once per result.</param>
    /// <param name="always">if set to <c>true</c> [always] this object will appear always in the result.</param>
    /// <param name="enabled">if set to <c>false</c> [enabled] this object will never be part of the result (even if it is set to always=true!).</param>
    public RandomizerTableItem(Single probability, Boolean unique, Boolean always, Boolean enabled)
    {
        Weight = probability;
        IsUnique = unique;
        IsAlwaysDropped = always;
        IsEnabled = enabled;
        Table = null;
    }

    /// <summary>
    /// Occurs before all the probabilities of all items of the current RDSTable are summed up together.
    /// This is the moment to modify any settings immediately before a result is calculated.
    /// </summary>
    public event EventHandler ResultEvaluationPreparing;
    /// <summary>
    /// Occurs when this RDSObject has been hit by the Result procedure.
    /// (This means, this object will be part of the result set).
    /// </summary>
    public event EventHandler AddedToResult;
    /// <summary>
    /// Occurs after the result has been calculated and the result set is complete, but before
    /// the RDSTable's Result method exits.
    /// </summary>
    public event EventHandler<RandomizerTableResultEventArgs> ResultEvaluated;

    /// <summary>
    /// Raises the <see cref="E:PreResultEvaluation"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnResultEvaluationPreparing(EventArgs e) => ResultEvaluationPreparing?.Invoke(this, e);
	/// <summary>
    /// Raises the <see cref="E:Hit"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnAddedToResult(EventArgs e) => AddedToResult?.Invoke(this, e);
	/// <summary>
    /// Raises the <see cref="E:PostResultEvaluation"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnResultEvaluated(RandomizerTableResultEventArgs e) => ResultEvaluated?.Invoke(this, e);


    /// <summary>
    /// Gets or sets the probability for this object to be (part of) the result
    /// </summary>
    public Single Weight { get; set; }
    /// <summary>
    /// Gets or sets whether this object may be contained only once in the result set
    /// </summary>
    public Boolean IsUnique { get; set; }
    /// <summary>
    /// Gets or sets whether this object will always be part of the result set
    /// (Probability is ignored when this flag is set to true)
    /// </summary>
    public Boolean IsAlwaysDropped { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IRandomizerTableItem"/> is enabled.
    /// Only enabled entries can be part of the result of a RDSTable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public Boolean IsEnabled { get; set; }
    /// <summary>
    /// Gets or sets the table this Object belongs to.
    /// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
    /// </summary>
    public RandomizerTable Table { get; set; }


	/// <summary>
	/// Creates an instance of the object derived from <see cref="IRandomizerTableItem"/> where this method is implemented in.
	/// Override to instanciate more complex constructors.
	/// </summary>
	/// <returns>An instance of an object of the type derived from <see cref="IRandomizerTableItem"/></returns>
    public virtual IRandomizerTableItem CreateNewOrGetCurrentInstance() => this;


    #region TOSTRING
    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override String ToString()
    {
        return ToString(0);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public String ToString(Int32 indentationlevel)
    {
        String indent = "".PadRight(4 * indentationlevel, ' ');

        return String.Format(indent + "(RDSObject){0} Prob:{1},UAE:{2}{3}{4}",
            this.GetType().Name, Weight,
            (IsUnique ? "1" : "0"), (IsAlwaysDropped ? "1" : "0"), (IsEnabled ? "1" : "0"));
    }
    #endregion
}

/// <summary>
/// This is the default class for a "null" entry in a RDSTable.
/// It just contains a value that is null (if added to a table of RDSValue objects),
/// but is a class as well and can be checked via a "if (obj is RDSNullValue)..." construct
/// </summary>
public class RandomizerTableItemWithValueNull : RandomizerTableItemWithValue<Object>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RandomizerTableItemWithValueNull"/> class.
	/// </summary>
	/// <param name="probability">The probability.</param>
    public RandomizerTableItemWithValueNull(Single probability) : base(null, probability, false, false, true) { }
}


/// <summary>
/// This class holds a single RDS value.
/// It's a generic class to allow the developer to add any type to a RDSTable.
/// T can of course be either a value type or a reference type, so it's possible,
/// to add RDSValue objects that contain a reference type, too.
/// </summary>
/// <typeparam name="T">The type of the value</typeparam>
public class RandomizerTableItemWithValue<T> : IRandomizerTableItemWithValue<T>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RandomizerTableItemWithValue{T}"/> class.
	/// </summary>
    public RandomizerTableItemWithValue() : this(default, 0, false, false, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTableItemWithValue{T}"/> class.
    /// The Unique and Always flags are set to (default) false with this constructor, and Enabled is set to true.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="probability">The probability.</param>
    public RandomizerTableItemWithValue(T value, Single probability) : this(value, probability, false, false, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTableItemWithValue{T}"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="probability">The probability.</param>
    /// <param name="unique">if set to <c>true</c> [unique].</param>
    /// <param name="always">if set to <c>true</c> [always].</param>
    /// <param name="enabled">if set to <c>true</c> [enabled].</param>
    public RandomizerTableItemWithValue(T value, Single probability, Boolean unique, Boolean always, Boolean enabled)
    {
        _value = value;
        Weight = probability;
        IsUnique = unique;
        IsAlwaysDropped = always;
        IsEnabled = enabled;
        Table = null;
    }

    /// <summary>
    /// Occurs before all the probabilities of all items of the current RDSTable are summed up together.
    /// This is the moment to modify any settings immediately before a result is calculated.
    /// </summary>
    public event EventHandler ResultEvaluationPreparing;
    /// <summary>
    /// Occurs when this RDSObject has been hit by the Result procedure.
    /// (This means, this object will be part of the result set).
    /// </summary>
    public event EventHandler AddedToResult;
    /// <summary>
    /// Occurs after the result has been calculated and the result set is complete, but before
    /// the RDSTable's Result method exits.
    /// </summary>
    public event EventHandler<RandomizerTableResultEventArgs> ResultEvaluated;

    /// <summary>
    /// Raises the <see cref="E:PreResultEvaluation"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnResultEvaluationPreparing(EventArgs e) => ResultEvaluationPreparing?.Invoke(this, e);
	/// <summary>
    /// Raises the <see cref="E:Hit"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnAddedToResult(EventArgs e) => AddedToResult?.Invoke(this, e);
	/// <summary>
    /// Raises the <see cref="E:PostResultEvaluation"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnResultEvaluated(RandomizerTableResultEventArgs e) => ResultEvaluated?.Invoke(this, e);


    /// <summary>
    /// The value of this object
    /// </summary>
    public virtual T Value
    {
        get => _value;
		set => _value = value;
	}
    private T _value;


    /// <summary>
    /// Gets or sets the probability for this object to be (part of) the result
    /// </summary>
    public Single Weight { get; set; }
    /// <summary>
    /// Gets or sets whether this object may be contained only once in the result set
    /// </summary>
    public Boolean IsUnique { get; set; }
    /// <summary>
    /// Gets or sets whether this object will always be part of the result set
    /// (Probability is ignored when this flag is set to true)
    /// </summary>
    public Boolean IsAlwaysDropped { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IRandomizerTableItem"/> is enabled.
    /// Only enabled entries can be part of the result of a RDSTable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public Boolean IsEnabled { get; set; }
    /// <summary>
    /// Gets or sets the table this Object belongs to.
    /// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
    /// </summary>
    public RandomizerTable Table { get; set; }


	/// <summary>
	/// Creates an instance of the object derived from <see cref="IRandomizerTableItem"/> where this method is implemented in.
	/// Override to instanciate more complex constructors.
	/// </summary>
	/// <returns>An instance of an object of the type derived from <see cref="IRandomizerTableItem"/></returns>
    public virtual IRandomizerTableItem CreateNewOrGetCurrentInstance() => this;


	#region TOSTRING
    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override String ToString() => ToString(0);

	/// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public String ToString(Int32 indentationlevel)
    {
        String indent = "".PadRight(4 * indentationlevel, ' ');

        String valstr = "(null)";
        if (Value != null)
            valstr = Value.ToString();
        return String.Format(indent + "(RDSValue){0} \"{1}\",Prob:{2},UAE:{3}{4}{5}",
            this.GetType().Name, valstr, Weight,
            (IsUnique ? "1" : "0"), (IsAlwaysDropped ? "1" : "0"), (IsEnabled ? "1" : "0"));
    }
    #endregion
}

/// <summary>
/// Holds a table of RDS objects. This class is "the randomizer" of the RDS.
/// The Result implementation of the IRDSTable interface uses the RDSRandom class
/// to determine which elements are hit.
/// </summary>
public class RandomizerTable : IRandomizerTable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTable"/> class.
    /// </summary>
    public RandomizerTable() : this(null, 1, 1, false, false, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTable"/> class.
    /// </summary>
    /// <param name="contents">The contents.</param>
    /// <param name="count">The count.</param>
    /// <param name="probability">The probability.</param>
    public RandomizerTable(IEnumerable<IRandomizerTableItem> contents, Int32 count, Single probability) : this(contents, count, probability, false, false, true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RandomizerTable"/> class.
    /// </summary>
    /// <param name="contents">The contents.</param>
    /// <param name="count">The count.</param>
    /// <param name="probability">The probability.</param>
    /// <param name="unique">if set to <c>true</c> any item of this table (or contained sub tables) can be in the result only once.</param>
    /// <param name="always">if set to <c>true</c> the probability is disabled and the result will always contain (count) entries of this table.</param>
    /// <param name="enabled">if set to <c>true</c> [enabled].</param>
    public RandomizerTable(IEnumerable<IRandomizerTableItem> contents, Int32 count, Single probability, Boolean unique, Boolean always, Boolean enabled)
	{
		_items = new List<IRandomizerTableItem>();
		foreach (var content in contents)
			_items.Add(content);

        ItemsToDropCount = count;
        Weight = probability;
        IsUnique = unique;
        IsAlwaysDropped = always;
        IsEnabled = enabled;
    }

    /// <summary>
    /// Occurs before all the probabilities of all items of the current RDSTable are summed up together.
    /// This is the moment to modify any settings immediately before a result is calculated.
    /// </summary>
    public event EventHandler ResultEvaluationPreparing;
    /// <summary>
    /// Occurs when this RDSObject has been hit by the Result procedure.
    /// (This means, this object will be part of the result set).
    /// </summary>
    public event EventHandler AddedToResult;
    /// <summary>
    /// Occurs after the result has been calculated and the result set is complete, but before
    /// the RDSTable's Result method exits.
    /// </summary>
    public event EventHandler<RandomizerTableResultEventArgs> ResultEvaluated;

    /// <summary>
    /// Raises the <see cref="E:PreResultEvaluation"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnResultEvaluationPreparing(EventArgs e) => ResultEvaluationPreparing?.Invoke(this, e);
	/// <summary>
    /// Raises the <see cref="E:Hit"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnAddedToResult(EventArgs e) => AddedToResult?.Invoke(this, e);
	/// <summary>
    /// Raises the <see cref="E:PostResultEvaluation"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public virtual void OnResultEvaluated(RandomizerTableResultEventArgs e) => ResultEvaluated?.Invoke(this, e);



	/// <summary>
	/// Creates an instance of the object derived from <see cref="IRandomizerTableItem"/> where this method is implemented in.
	/// Override to instanciate more complex constructors.
	/// </summary>
	/// <returns>An instance of an object of the type derived from <see cref="IRandomizerTableItem"/></returns>
    public virtual IRandomizerTableItem CreateNewOrGetCurrentInstance() => this;


	/// <summary>
    /// The maximum number of entries expected in the Result. The final count of items in the result may be lower
    /// if some of the entries may return a null result (no drop)
    /// </summary>
    public Int32 ItemsToDropCount { get; set; }

    /// <summary>
    /// Gets or sets the contents of this table
    /// </summary>
    public IReadOnlyList<IRandomizerTableItem> Items => _items;
	private List<IRandomizerTableItem> _items;

    /// <summary>
    /// Clears the contents.
    /// </summary>
    public virtual void ClearItems() => _items = new List<IRandomizerTableItem>();

	/// <summary>
    /// Adds the given entry to contents collection.
    /// </summary>
    /// <param name="item">The entry.</param>
    public virtual void AddItem(IRandomizerTableItem item)
    {
        _items.Add(item);
        item.Table = this;
    }
    /// <summary>
    /// Adds a new entry to the contents collection and allows directly assigning of a probability for it.
    /// Use this signature if (for whatever reason) the base classes constructor does not support all
    /// constructors of RDSObject or if you implemented IRDSObject directly in your class and you need
    /// to (re)apply a new probability at the moment you add it to a RDSTable.
    /// NOTE: The probability given is written back to the given instance "entry".
    /// </summary>
    /// <param name="item">The entry.</param>
    /// <param name="weight">The weight.</param>
    public virtual void AddItem(IRandomizerTableItem item, Single weight)
    {
        _items.Add(item);
        item.Weight = weight;
        item.Table = this;
    }
    /// <summary>
    /// Adds a new entry to the contents collection and allows directly assigning of a probability and drop flags for it.
    /// Use this signature if (for whatever reason) the base classes constructor does not support all
    /// constructors of RDSObject or if you implemented IRDSObject directly in your class and you need
    /// to (re)apply a new probability and flags at the moment you add it to a RDSTable.
    /// NOTE: The probability, unique, always and enabled flags given are written back to the given instance "entry".
    /// </summary>
    /// <param name="item">The entry.</param>
    /// <param name="weight">The probability.</param>
    /// <param name="isUnique">if set to <c>true</c> this object can only occur once per result.</param>
    /// <param name="isAlwaysDropped">if set to <c>true</c> [always] this object will appear always in the result.</param>
    /// <param name="isEnabled">if set to <c>false</c> [enabled] this object will never be part of the result (even if it is set to always=true!).</param>
    public virtual void AddItem(IRandomizerTableItem item, Single weight, Boolean isUnique, Boolean isAlwaysDropped, Boolean isEnabled)
    {
        _items.Add(item);
        item.Weight = weight;
        item.IsUnique = isUnique;
        item.IsAlwaysDropped = isAlwaysDropped;
        item.IsEnabled = isEnabled;
        item.Table = this;
    }

    /// <summary>
    /// Removes the given entry from the contents. If it is not part of the contents, an exception occurs.
    /// </summary>
    /// <param name="item">The entry.</param>
    public virtual void RemoveItem(IRandomizerTableItem item)
    {
        _items.Remove(item);
        item.Table = null;
    }
    /// <summary>
    /// Removes the entry at the given index position.
    /// If the index is out-of-range of the current contents collection, an exception occurs.
    /// </summary>
    /// <param name="index">The index.</param>
    public virtual void RemoveItem(Int32 index)
    {
        var entry = _items[index];
        entry.Table = null;
        _items.RemoveAt(index);
    }

    private readonly HashSet<IRandomizerTableItem> _uniqueItems = new HashSet<IRandomizerTableItem>();

    private void AddToResult(List<IRandomizerTableItem> itemsToReturn, IRandomizerTableItem item)
    {
        if (!item.IsUnique || !_uniqueItems.Contains(item))
        {
            if (item.IsUnique)
                _uniqueItems.Add(item);

			var table = item as IRandomizerTable;
			if (table != null)
				itemsToReturn.AddRange(table.GetResult);
			else
			{
                // INSTANCE CHECK
                // Check if the object to add implements IRDSObjectCreator.
                // If it does, call the CreateInstance() method and add its return value
                // to the result set. If it does not, add the object o directly.
				var itemToAdd = item.CreateNewOrGetCurrentInstance();
                itemsToReturn.Add(itemToAdd);
                item.OnAddedToResult(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets the result. Calling this method will start the random pick process and generate the result.
    /// This result remains constant for the lifetime of this table object.
    /// Use the ResetResult method to clear the result and create a new one.
    /// </summary>
    public virtual IReadOnlyList<IRandomizerTableItem> GetResult
    {
        get
        {
            // The return value, a list of hit objects
            var itemsToReturn = new List<IRandomizerTableItem>();
            _uniqueItems.Clear();

            // Do the PreEvaluation on all objects contained in the current table
            // This is the moment where those objects might disable themselves.
            foreach (IRandomizerTableItem item in _items)
                item.OnResultEvaluationPreparing(EventArgs.Empty);

			var alwaysDroppedItemsCount = 0;

            // Add all the objects that are hit "Always" to the result
            // Those objects are really added always, no matter what "Count"
            // is set in the table! If there are 5 objects "always", those 5 will
            // drop, even if the count says only 3.
            foreach (var item in _items)
				if (item.IsAlwaysDropped && item.IsEnabled)
				{
					AddToResult(itemsToReturn, item);
					alwaysDroppedItemsCount++;
				}

            // Now calculate the real drop count, this is the table's count minus the
            // number of Always-drops.
            // It is possible, that the remaining drops go below zero, in which case
            // no other objects will be added to the result here.

			var randomlyDroppedItemsCount = ItemsToDropCount - alwaysDroppedItemsCount;

            // Continue only, if there is a Count left to be processed
            if (randomlyDroppedItemsCount > 0)
            {
                for (var i = 0; i < randomlyDroppedItemsCount; i++)
                {
                    // Find the objects, that can be hit now
                    // This is all objects, that are Enabled and that have not already been added through the Always flag
					var items = new List<IRandomizerTableItem>();

                    // This is the magic random number that will decide, which object is hit now
					var itemsWeightSum = 0f;

					for (var j = 0; j < _items.Count; j++)
					{
						var rdsObject = _items[j];

						if (!rdsObject.IsEnabled || rdsObject.IsAlwaysDropped)
							continue;

						items.Add(rdsObject);
						itemsWeightSum += rdsObject.Weight;
					}

					var randomWeight = Random.Range(0, itemsWeightSum);
					var currentWeight = 0f;
                    foreach (var item in items)
                    {
                        currentWeight += item.Weight;
                        if (randomWeight < currentWeight)
                        {
                            AddToResult(itemsToReturn, item);
                            break;
                        }
                    }
                }
            }

			//TODO: Change RandomizerTableResultEA -> add currently not selected items?
            // Now give all objects in the result set the chance to interact with
            // the other objects in the result set.
            var resultEventArgs = new RandomizerTableResultEventArgs(itemsToReturn);
            foreach (IRandomizerTableItem item in itemsToReturn)
                item.OnResultEvaluated(resultEventArgs);

            // Return the set now
            return itemsToReturn;
        }
    }

    /// <summary>
    /// Gets or sets the probability for this object to be (part of) the result
    /// </summary>
    public Single Weight { get; set; }
    /// <summary>
    /// Gets or sets whether this object may be contained only once in the result set
    /// </summary>
    public Boolean IsUnique { get; set; }
    /// <summary>
    /// Gets or sets whether this object will always be part of the result set
    /// (Probability is ignored when this flag is set to true)
    /// </summary>
    public Boolean IsAlwaysDropped { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="IRandomizerTableItem"/> is enabled.
    /// Only enabled entries can be part of the result of a RDSTable.
    /// </summary>
    /// <value>
    ///   <c>true</c> if enabled; otherwise, <c>false</c>.
    /// </value>
    public Boolean IsEnabled { get; set; }
    /// <summary>
    /// Gets or sets the table this Object belongs to.
    /// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
    /// </summary>
    public RandomizerTable Table { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public override String ToString()
    {
        return ToString(0);
    }

    /// <summary>
    /// Returns a <see cref="System.String"/> that represents this instance.
    /// </summary>
    /// <param name="indentationlevel">The indentationlevel. 4 blanks at the beginning of each line for each level.</param>
    /// <returns>
    /// A <see cref="System.String"/> that represents this instance.
    /// </returns>
    public String ToString(Int32 indentationlevel)
    {
        String indent = "".PadRight(4 * indentationlevel, ' ');

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(indent + "(RDSTable){0} Entries:{1},Prob:{2},UAE:{3}{4}{5}{6}",
            this.GetType().Name, _items.Count, Weight,
            (IsUnique ? "1" : "0"), (IsAlwaysDropped ? "1" : "0"), (IsEnabled ? "1" : "0"), (_items.Count > 0 ? "\r\n" : ""));

        foreach (IRandomizerTableItem o in _items)
            sb.AppendLine(indent + o.ToString(indentationlevel + 1));

        return sb.ToString();
    }
}