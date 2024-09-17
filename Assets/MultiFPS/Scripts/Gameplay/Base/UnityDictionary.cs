using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class UnityDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
	[SerializeField]
	private List<TKey> _keys = new List<TKey>();

	[SerializeField]
	private List<TValue> _values = new List<TValue>();

	public UnityDictionary()
	{

	}

	public UnityDictionary(int capacity)
	{
		_keys.Capacity = capacity;
		_values.Capacity = capacity;
	}

	public bool ContainsKey(TKey key)
	{
		return _keys.Contains(key);
	}

	public void Add(TKey key, TValue value)
	{
		if (key == null) throw new ArgumentNullException("key null");
		if (_keys.Contains(key)) throw new ArgumentException("key already exsits");
		_keys.Add(key);
		_values.Add(value);
	}

	public bool Remove(TKey key)
	{
		if (key == null) throw new ArgumentNullException("key null");
		if (!_keys.Contains(key)) return false;
		var index = _keys.IndexOf(key);
		_keys.RemoveAt(index);
		_values.RemoveAt(index);
		return true;
	}

	// Private method for removing the key/value pair at a particular index
	// This should never be public; dictionaries aren't supposed to have any
	// ordering on their elements, so the idea of an element at a particular
	// index isn't valid in the outside world. That we're using indexable
	// lists for storing keys/values is an implementation detail.
	private void RemoveAt(int index)
	{
		if (index >= _keys.Count) throw new ArgumentOutOfRangeException("Out of index");
		_keys.RemoveAt(index);
		_values.RemoveAt(index);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		value = default(TValue);
		if (key == null) throw new ArgumentNullException("key null");
		if (!_keys.Contains(key)) return false;
		value = _values[_keys.IndexOf(key)];
		return true;
	}

	TValue IDictionary<TKey, TValue>.this[TKey key]
	{
		get { return this[key]; }
		set { this[key] = value; }
	}

	public TValue this[TKey key]
	{
		get
		{
			if (key == null) throw new ArgumentNullException("key null");
			if (!_keys.Contains(key)) throw new ArgumentException("key doesn't exsit");
			return _values[_keys.IndexOf(key)];
		}
		set
		{
			if (key == null) throw new ArgumentNullException("key null");
			if (!_keys.Contains(key)) throw new ArgumentException("key doesn't exsit");
			_values[_keys.IndexOf(key)] = value;
		}
	}

	#region ICollection implementation
	public void Add(KeyValuePair<TKey, TValue> item)
	{
		Add(item.Key, item.Value);
	}

	public void Clear()
	{
		_keys.Clear();
		_values.Clear();
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		return ContainsKey(item.Key);
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		return Remove(item.Key);
	}
	public int Count
	{
		get
		{
			return _keys.Count;
		}
	}
	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}
	#endregion
	#region IEnumerable implementation
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		for (int i = 0; i < _keys.Count; i++)
		{
			yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
		}
	}


	#endregion
	#region IEnumerable implementation
	IEnumerator IEnumerable.GetEnumerator()
	{
		for (int i = 0; i < _keys.Count; i++)
		{
			yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
		}
	}
	#endregion

	#region IDictionary implementation

	public ICollection<TKey> Keys
	{
		get
		{
			return _keys.ToArray();
		}
	}

	public ICollection<TValue> Values
	{
		get
		{
			return _values.ToArray();
		}
	}

	#endregion
}

[System.Serializable]
public class UIMenuDic : UnityDictionary<MENUNAME_E, UIMenu> { }
[System.Serializable]
public class UIPopupDic : UnityDictionary<POPUPNAME_E, UIPopup> { }