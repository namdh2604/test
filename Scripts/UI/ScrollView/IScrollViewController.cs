
using System;
using System.Collections.Generic;

namespace Voltage.Witches.UI
{
	using UnityEngine;

	public interface IScrollView<T>		// a bit large, should break this into smaller interfaces (and MAYBE compose another interface from them)
	{
		T GoToElementAt (int index);
		T GoToElement (T element);
		void AddElement (T element, bool refresh=true);
		void InsertElementAt (int index, T element);
		void Populate(IList<T> elements);
		void RemoveElement(T element, bool refresh=true);
		void RemoveElementAt (int index);
		void RemoveFirst();
		void RemoveLast();
		void Clear();
	}

//	public interface IScrollViewElement		// maybe not necessary
//	{
//
//	}

//	public interface IScrollViewController		// maybe not necessary
//	{
//
//	}


	public class ScrollOptions
	{
		public enum Type
		{
			HORIZONTAL,
			VERTICAL,
//			CIRCULAR
		}

		public enum HorizontalDirection
		{
			LEFT_TO_RIGHT,
			RIGHT_TO_LEFT,
		}

		public enum VerticalDirection
		{
			TOP_TO_BOTTOM,
			BOTTOM_TO_TOP
		}
	}



	public class RectTransformConfigs		// necessary??? sealed???
	{
		public interface IConfigs
		{
			Vector2 LeftTop { get; }
//			Vector2 PosXY { get; }
//			float PosZ { get; }
			Vector2 RightBottom { get; }	
//			Vector2 WidthHeight { get; }
			Vector2 AnchorMin { get; }
			Vector2 AnchorMax { get; }
			Vector2 Pivot { get; }

			Vector3 RotationEuler { get; }
			Vector3 Scale { get; }
		}

		public struct Fill : IConfigs
		{
			public Vector2 LeftTop { get { return Vector2.zero; } }
			public Vector2 RightBottom { get { return Vector2.zero; } }	
//			public Vector2 WidthHeight { get; }
			public Vector2 AnchorMin { get { return Vector2.zero; } }
			public Vector2 AnchorMax { get { return Vector2.one; } }
			public Vector2 Pivot { get { return new Vector2 (0.5f, 0.5f); } }
			
			public Vector3 RotationEuler { get { return Vector3.zero; } }
			public Vector3 Scale { get { return Vector3.one; } }
		}

	}

}



