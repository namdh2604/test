//
//using System;
//using System.Collections.Generic;
//
//namespace Voltage.Story.Variables
//{
//	using Voltage.Common.Logging;
//
//	using Voltage.Story.Mapper;
//	using Voltage.Story.Variables.Keys;
//	
//	
//
//
//	public abstract class VariableStateMapper<T> : IMapping<IVariableStateKey<T>> 
//    {
//
//		public Dictionary<string,Func<T,object>> Map { get; protected set; }	// NOTE: not really protected set
//		protected ILogger Logger { get; private set; }
//
//		public VariableStateMapper (ILogger logger)
//		{
//			Map = new Dictionary<string, Func<T,object>>();
//
//			Logger = logger;
//		}
//
//
//		public bool TryGetValue<U> (IVariableStateKey<T> key, out U value)
//		{
//
//			if(key != null && !string.IsNullOrEmpty(key.Key))
//			{
//				if(Map.ContainsKey(key.Key))
//				{
//					try
//					{
//						value = (U)Map[key.Key](key.State);
//						return true;
//					}
//					catch (InvalidCastException e)
//					{
//						Logger.Log ("VariableStateMapper::Retrieve >>> invalid cast " + e, LogLevel.WARNING);
//					}
//				}
//			}
//
//			value = default(U);
//			return false;
//		}
//
//	}
//
//}
//
//
//
//
////		public virtual U Retrieve<U>(IVariableStateKey<T> key)
////		{
////			try
////			{
////				if(key != null)
////				{
//////					if(key.State != null && Map.ContainsKey(key.Key))
////					if(Map.ContainsKey(key.Key))
////					{
////						return (U)Map[key.Key](key.State);
////					}
////				}
////			}
////			catch (InvalidCastException e)
////			{
////				Logger.Log ("VariableStateMapper::Retrieve >>> invalid cast " + e, LogLevel.WARNING);
////			}
////			
////			return default(U);
////		}
//
