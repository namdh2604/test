
using System;
using System.Collections.Generic;

namespace Voltage.Story.Expressions
{
	using Voltage.Story.General;
	using Voltage.Story.Mapper;


	public interface IExpressionFactory
	{
		IExpression CreateExpression(ExpressionState state);
	}

    public class ExpressionFactory : IExpressionFactory
    {
		public IMapping<string> VariableMap { get; set; }
		public IParser<ExpressionState> Parser { get; private set; }


		public ExpressionFactory (IParser<ExpressionState> parser, IMapping<string> variableMap)
		{
			Parser = parser;
			VariableMap = variableMap;
		}

		public IExpression CreateExpression (ExpressionState state)
		{
			IExpression left = GetExpression (state.LeftType, state.Left);
			IExpression right = GetExpression (state.RightType, state.Right);
			IOperator op = GetOperator(state.Operator);

			return new Expression(left, right, op);
		}

		private IExpression GetExpression(ExpressionType type, string token)
		{
			if(Parser != null && VariableMap != null)
			{
				Dictionary<ExpressionType,Func<string,IExpression>> expressionSwitch = new Dictionary<ExpressionType,Func<string,IExpression>> ()	// Dictionary<Type,Func<string,IExpression>> expressionSwitch = new Dictionary<Type,Func<string,IExpression>> ()
				{
					{ExpressionType.NUMERAL, (t) => new NumeralExpression(Int32.Parse(t))},
					{ExpressionType.STRING, (t) => new StringExpression(t)},
                    {ExpressionType.VARIABLE, CreateVariableExpression},
					{ExpressionType.EXPRESSION, (t) => CreateExpression(Parser.Parse(t))},
					{ExpressionType.NONE, (t) => default(IExpression)},							
				};

				return expressionSwitch[type](token);
			}
			else
			{
				throw new NullReferenceException("ExpressionFactory::GetExpression");
			}
		}

        private IVariableExpression CreateVariableExpression(string token)
        {
            if (SceneSelectionVariableExpression.HandlesPath(token))
            {
                return new SceneSelectionVariableExpression(token, VariableMap);
            }

            return new DefaultVariableExpression(token, VariableMap);
        }



		private IOperator GetOperator(string op)
		{
			// AND | OR | > | < | = | != | >= | <=
			switch(op)
			{
				case "AND": return new AndOperator();
				case "OR": return new OrOperator();
				case "=": return new EqualToOperator();
				case "!=": return new NotEqualToOperator();
				case ">": return new GreaterThanOperator();
				case ">=": return new GreaterThanEqualToOperator();
				case "<": return new LessThanOperator();
				case "<=": return new LessThanEqualToOperator();

				default: return default(IOperator); 	// NOTE: can have no operator, so return null
				//throw new ArgumentException("ExpressionFactory::GetOperator");
			}
		}

//		private bool ValidState(ExpressionState state)
//		{
//
//		}

    }
    
}




