using cobra;
using System;
using static cobra.Program;

namespace Cobra
{
	public class Interpreter : IVisitor<object>
	{
		static void RuntimeException(RuntimeException error)
		{
			Console.WriteLine(error + "\n[line " + error.Token.Line + "]");
			HadRuntimeError = true;
		}
		public void interpret(Expression expression)
		{
			try
			{
				object value = Evaluate(expression);
				Console.WriteLine(stringify(value));
			}
			catch
			{
			}
		}

		private string stringify(object obj)
		{
			if (obj == null)
			{
				Console.WriteLine("obj is null");
				return "nil";
			}

			return obj.ToString();
		}

		private bool isEqual(object lhs, object rhs)
		{
			if ((lhs == null) && (rhs == null)) return true;
			if (lhs == null) return false;

			return lhs.Equals(rhs);
		}
		public object VisitBinaryExpression(Binary expression)
		{
			object lhs = Evaluate(expression.lhs);
			object rhs = Evaluate(expression.rhs);

			switch (expression.operation.Type)
			{
				case TokenType.GREATER:
					CheckNumberOperands(expression.operation, lhs, rhs);
					return (double)lhs > (double)rhs;
				case TokenType.GREATER_EQUAL: return (double)lhs >= (double)rhs;
				case TokenType.LESS: return (double)lhs < (double)rhs;
				case TokenType.LESS_EQUAL: return (double)lhs <= (double)rhs;
				case TokenType.BANG_EQUAL: return !isEqual(lhs, rhs);
				case TokenType.EQUAL_EQUAL: return isEqual(lhs, rhs);
				case TokenType.MINUS:
					if ((lhs.GetType() == typeof(double)) && (rhs.GetType() == typeof(double)))
					{
						return (double)lhs + (double)rhs;
					}
					if ((lhs.GetType() == typeof(string)) && (rhs.GetType() == typeof(string)))
					{
						return (string)lhs + (string)rhs;
					}
					throw new RuntimeException(expression.operation, "Operands must be two numbers or two strings.");
				case TokenType.PLUS:
					if ((lhs.GetType() == typeof(double)) && (rhs.GetType() == typeof(double)))
					{
						return (double)lhs + (double)rhs;
					}

					if ((lhs.GetType() == typeof(string)) && (rhs.GetType() == typeof(string)))
					{
						return (string)lhs + (string)rhs;
					}
					break;
				case TokenType.SLASH: return (double)lhs / (double)rhs;
				case TokenType.STAR: return (double)lhs * (double)rhs;
			}
			return null;
		}

		public object VisitGroupingExpression(Grouping expression) { return Evaluate(expression.Expression); }

		public object VisitLiteralExpression(Literal expression) { return expression.Value; }

		public object VisitUnaryExpression(Unary expression)
		{
			object rhs = Evaluate(expression.rhs);

			switch (expression.operation.Type)
			{
				case TokenType.MINUS:
					CheckNumberOperands(expression.operation, rhs);
					return -(double)rhs;
				case TokenType.BANG: return !isTruthy(rhs);
			}

			return null;
		}

		private void CheckNumberOperands(Token operation, object operand)
		{
			if (operand.GetType() == typeof(double)) return;
			throw new RuntimeException(operation, "Operand must be a number.");
		}

		private void CheckNumberOperands(Token operation, object lhs, object rhs)
		{
			if ((lhs.GetType() == typeof(double)) && (rhs.GetType() == typeof(double))) return;

			throw new RuntimeException(operation, "Operands must be numbers.");
		}

		private bool isTruthy(object obj)
		{
			if (obj == null) return false;
			if (obj.GetType() == typeof(bool)) return (bool)obj;
			return true;
		}

		private object Evaluate(Expression expression) { return expression.Accept(this); }
	}
}