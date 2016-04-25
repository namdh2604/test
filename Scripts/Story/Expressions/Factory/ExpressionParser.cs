
using System;
using System.Collections.Generic;

namespace Voltage.Story.Expressions
{
	using Voltage.Story.General;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;


	public enum ExpressionType
	{
		NONE = 0,
		STRING,
		NUMERAL,
		VARIABLE,
		EXPRESSION
	}

	public struct ExpressionState
	{
		public ExpressionType LeftType { get; private set; } 	// public Type LeftType { get; private set; }
		public string Left { get; private set; }

		public ExpressionType RightType { get; private set; } 	// public Type RightType { get; private set; }
		public string Right { get; private set; }

		public string Operator { get; private set; }


		public ExpressionState (ExpressionType lType, string left, ExpressionType rType, string right, string op) : this()
		{
			LeftType = lType;
			Left = left;

			RightType = rType;
			Right = right;

			Operator = op;

//			Console.WriteLine (string.Format ("{0} <{1}>, {2} <{3}>, {4}", Left, LeftType.ToString (), Right, RightType.ToString (), Operator));
		}
	}

    public class ExpressionParser : IParser<ExpressionState>
    {

		private struct Tuple
		{
			public ExpressionType Type { get; private set; } //public Type Type { get; private set; }
			public string Value { get; private set; }

			public Tuple (ExpressionType type, string value) : this()
			{
				Type = type;
				Value = value;
			}
		}


		public ExpressionState Parse (string json)
		{
			JObject obj = JObject.Parse (json);

			string op = GetOperator(obj);

			Tuple left = GetExpression (obj, "left");
			Tuple right = GetExpression (obj, "right");		// if (!string.IsNullOrEmpty(op))

			return new ExpressionState (left.Type, left.Value, right.Type, right.Value, op);
		}

		private string GetOperator (JObject obj)
		{	
			JToken opToken;
			if(obj != null && obj.TryGetValue ("op", out opToken))
			{
				return opToken.ToString();
			}
			else
			{
				return string.Empty;
			}
		}


		private Tuple GetExpression (JObject obj, string property)
		{
			JToken token;
			if(obj != null && obj.TryGetValue(property, out token))
			{
				ExpressionType type;
				if(token.Type != JTokenType.Null)
				{
					type = DetermineType (token.ToString());
					string value = GetValue(token.ToString(), type);

					return new Tuple(type, value);
				}
			}
//			else
//			{
//				throw new ArgumentException("ExpressionParser::GetExpression");
//			}

			return new Tuple (ExpressionType.NONE, string.Empty);
		}


//		private Type DetermineType (string token)
		private ExpressionType DetermineType (string token)
		{
			if(!string.IsNullOrEmpty(token))
			{
				if(token[0] == '"')
				{
					return ExpressionType.STRING;	// return Type.GetType("Voltage.Story.Expressions.StringExpression");
				}

				if(token[0] == '{')
				{
					JObject obj = JObject.Parse(token);

					JToken classType;
					if(obj.TryGetValue("_class", out classType))
					{
						switch(classType.ToString())
						{
							case "Variable":
								return ExpressionType.VARIABLE; // return Type.GetType("Voltage.Story.Expressions.VariableExpression");
							case "Expression":
								return ExpressionType.EXPRESSION;
						}
					}
				}

				int numeralTest;
				if(Int32.TryParse(token, out numeralTest))
				{
					return ExpressionType.NUMERAL; // return Type.GetType("Voltage.Story.Expressions.NumeralExpression");		// Voltage.Story.Expressions.NumeralExpression, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
				}
			}
//			else
//			{
				return ExpressionType.NONE;		// NOTE: field can be empty, return null
//				throw new NullReferenceException("ExpressionParse::GetType");
//			}
		}

		private string GetValue (string value, ExpressionType type)
		{
			switch(type)
			{
				case ExpressionType.STRING:
				case ExpressionType.NUMERAL:
				case ExpressionType.EXPRESSION:
					return value;
				case ExpressionType.VARIABLE:
					JObject obj = JObject.Parse (value);
					
					JToken variable;
					if(obj.TryGetValue("text", out variable))
					{	
						return variable.ToString();
					}
					else
					{
						return string.Empty;	// NOTE: too bad can't fall thru
					}

				case ExpressionType.NONE:
				default:
					return string.Empty;
			}
		}



    }
    
}




