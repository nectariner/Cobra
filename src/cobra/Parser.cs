using static cobra.Program;
using System;
using cobra;
using System.Collections.Generic;

namespace Cobra
{
	public class Parser
	{
		private class ParseError : Exception
		{

		}

		private readonly List<Token> tokens;
		private int current = 0;
		public Parser(List<Token> tokens)
		{
			this.tokens = tokens;
		}

		private Expression expression()
		{
			return equality();
		}

		private Expression equality()
		{
			Expression expression = comparison();
			while (match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
			{
				Token operation = previous();
				Expression right = comparison();
				expression = new Binary(expression, operation, right);
			}
			return expression;
		}

		private bool match(params TokenType[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				TokenType type = types[i];
				if (check(type))
				{
					Advance();
					return true;
				}
			}
			return false;
		}

		private bool check(TokenType type)
		{
			if (IsAtEnd()) return false;
			return Peek().Type == type;
		}

		private Token Advance()
		{
			if (!IsAtEnd()) current++;
			return previous();
		}

		private bool IsAtEnd()
		{
			return Peek().Type == TokenType.EOF;
		}

		private Token Peek()
		{
			return tokens[current];
		}

		private Token previous()
		{
			return tokens[current - 1];
		}

		private Expression comparison()
		{
			Expression expression = term();

			while (match(TokenType.GREATER, TokenType.GREATER_EQUAL,
				TokenType.LESS, TokenType.LESS_EQUAL))
			{
				Token operation = previous();
				Expression right = term();
				expression = new Binary(expression, operation, right);
			}
			return expression;
		}

		private Expression term()
		{
			Expression expression = factor();
			while (match(TokenType.MINUS, TokenType.PLUS))
			{
				Token operation = previous();
				Expression right = factor();
				expression = new Binary(expression, operation, right);
			}
			return expression;
		}

		private Expression factor()
		{
			Expression expression = unary();
			while (match(TokenType.SLASH, TokenType.STAR))
			{
				Token operation = previous();
				Expression right = unary();
				expression = new Binary(expression, operation, right);
			}
			return expression;
		}

		private Expression unary()
		{
			if (match(TokenType.BANG, TokenType.MINUS))
			{
				Token operation = previous();
				Expression right = unary();
				return new Unary(operation, right);
			}
			return Primary();
		}

		private Expression Primary()
		{
			if (match(TokenType.FALSE)) return new Literal(false);
			if (match(TokenType.TRUE)) return new Literal(true);
			if (match(TokenType.NIL)) return new Literal(null);
			if (match(TokenType.NUMBER, TokenType.STRING))
			{
				return new Literal(previous().Literal);
			}
			if (match(TokenType.LEFT_PAREN))
			{
				Expression expression_var = expression();
				Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
				return new Grouping(expression_var);
			}

			error(Peek(), "Expression expected.");
			throw new Exception();
		}

		private Token Consume(TokenType type, string message)
		{
			if (check(type)) return Advance();
			throw error(Peek(), message);
		}

		private ParseError error(Token token, string message)
		{
			error(token, message);
			return new ParseError();
		}

		private void synchronize()
		{
			Advance();
			while (!IsAtEnd())
			{
				if (previous().Type == TokenType.SEMICOLON) return;

				switch (Peek().Type)
				{
					case TokenType.CLASS:
					case TokenType.FUN:
					case TokenType.VAR:
					case TokenType.FOR:
					case TokenType.IF:
					case TokenType.WHILE:
					case TokenType.PRINT:
					case TokenType.RETURN:
						return;
				}
				Advance();
			}
		}

		public Expression parse()
		{
			try
			{
				return expression();
			}
			catch (ParseError)
			{
				return null;
			}
		}
	}
}