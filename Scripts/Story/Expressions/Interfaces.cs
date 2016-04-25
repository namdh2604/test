//	Expression := <Token> [<Operator> <Token>]
//
//	Operator := AND | OR | > | < | = | >= | <=
//		Token := <Number> | <String Literal> | <Variable> | (<Expression>)
//			Number := “7” etc
//			String Literal := “\”Hello\”” etc
//		Variable := { “_class”: “Variable”, “text”: “Path/To/Variable” }
//
//
//	JSON structure of expression:
//		
//	{
//	“_class”: “Expression”,
//	“left”: <Token>,
//	“right”: <Token>,
//	“op”: <Operator>
//	}

using System.Collections.Generic;

namespace Voltage.Story.Expressions
{	
	public interface IExpression
	{
		bool Evaluate();
        List<string> GetDependencies();
	}
	
	public interface IOperator
	{
		bool Operate (IExpression left, IExpression right);
	}
}


