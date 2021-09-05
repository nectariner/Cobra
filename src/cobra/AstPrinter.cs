using System.Text;
using cobra;
namespace Cobra
{
	public class AstPrinter : IVisitor<string>
	{
		public string print(cobra.Expression expression)
		{
			return expression.Accept(this);
		}

		private string Parenthesize(string name, params cobra.Expression[] expressions)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("(").Append(name);
			foreach (cobra.Expression expression in expressions)
			{
				builder.Append(" ");
				builder.Append(expression.Accept(this));
			}
			builder.Append(")");
			return builder.ToString();
		}

		public string VisitGroupingExpression(Grouping expression)
		{
			return Parenthesize("group", expression.Expression);
		}

		public string VisitBinaryExpression(Binary expression)
		{
			return Parenthesize(expression.operation.Lexeme, expression.lhs, expression.rhs);
		}

		public string VisitLiteralExpression(Literal expression)
		{
			if (expression.Value == null)
			{
				return "nil";
			}
			return expression.Value.ToString();
		}

		public string VisitUnaryExpression(Unary expression)
		{
			return Parenthesize(expression.operation.Lexeme, expression.rhs);
		}
	}
}