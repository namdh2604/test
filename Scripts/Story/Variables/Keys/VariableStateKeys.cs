//
//using System;
//using System.Collections.Generic;
//
//namespace Voltage.Story.Variables.Keys
//{
//	using Voltage.Story.Models.Nodes;
//
//
//	public sealed class VariableNodeKey : VariableStateKey<INode>
//	{
//		public VariableNodeKey(string key, INode node) : base (key, node) {}
//	}
//
//
//	public class VariableStateKey<T> : IVariableStateKey<T>
//	{
//		public T State { get; private set; }
//		public string Key { get; private set; }
//
//		public VariableStateKey(string key, T state)
//		{
//			Key = key;
//			State = state;
//		}
//	}
//    
//}
//
//
//
//
