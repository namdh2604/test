using System;
using System.Collections;
using System.Collections.Generic;

namespace Voltage.Witches.Util
{
	public class CircularArray<T> : IEnumerable<T>
	{
		private List<T> _data;
		int _currentOffset;

		public CircularArray()
		{
			_data = new List<T>();
			_currentOffset = 0;
		}

		public int Count
		{
			get { return _data.Count; }
		}

		public T this[int index]
		{
			get
			{
				int normalizedIndex = (index + _currentOffset + Count) % Count;
				return _data[normalizedIndex];
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(T element)
		{
			_data.Add(element);
		}

		public void RotateLeft(int offset)
		{
			Rotate(offset);
		}

		public void RotateRight(int offset)
		{
			Rotate(-offset);
		}

		private void Rotate(int offset)
		{
			if (Count == 0)
			{
				return;
			}

			_currentOffset = (_currentOffset + offset) % Count;
			if (_currentOffset < 0)
			{
				_currentOffset += Count;
			}
		}

		public struct Enumerator : IEnumerator<T>
		{
			CircularArray<T> _array;
			int _position;

			public Enumerator(CircularArray<T> array)
			{
				_array = array;
				_position = -1;
			}

			public bool MoveNext()
			{
				++_position;
				return (_position < _array.Count);
			}

			public void Reset()
			{
				_position = -1;
			}

			object IEnumerator.Current
			{
				get { return Current; }
			}

			public T Current
			{
				get { return _array[_position]; }
			}

			void IDisposable.Dispose()
			{
			}

		}

//		public IEnumerator GetEnumerator()
//		{
//			return GetEnumerator();
//		}
	}
}

