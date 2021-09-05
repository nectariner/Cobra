using System;
using Cobra;

namespace cobra
{
	public abstract class Expression
	{
		public abstract R Accept<R>(IVisitor<R> visitor);
		void x()
		{
			return;
		}
	}


	public interface IVisitor<R>
	{
		R VisitBinaryExpression(Binary expression);
		R VisitGroupingExpression(Grouping expression);
		R VisitLiteralExpression(Literal expression);
		R VisitUnaryExpression(Unary expression);
	}


	public class Binary : Expression
	{
		public Expression lhs { get; }
		public Token operation { get; }
		public Expression rhs { get; }
		public Binary(Expression left, Token operation, Expression right)
		{
			this.lhs = left;
			this.operation = operation;
			this.rhs = right;
		}
		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitBinaryExpression(this);
		}
	}

	public class Grouping : Expression
	{
		public Expression Expression { get; }
		public Grouping(Expression expression)
		{
			Expression = expression;
		}
		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitGroupingExpression(this);
		}
	}

	public class Literal : Expression
	{
		public Object Value { get; }
		public Literal(Object value)
		{
			Value = value;
		}
		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitLiteralExpression(this);
		}
	}

	public class Unary : Expression
	{
		public Token operation { get; }
		public Expression rhs { get; }
		public Unary(Token operation, Expression rhs)
		{
			this.operation = operation;
			this.rhs = rhs;
		}
		public override R Accept<R>(IVisitor<R> visitor)
		{
			return visitor.VisitUnaryExpression(this);
		}
	}
}