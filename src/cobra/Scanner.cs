using System;
using System.Collections.Generic;
using static cobra.Program;

namespace Cobra
{
	public class Scanner
	{
		private readonly string Source;
		private readonly List<Token> Tokens;

		private int _start = 0;
		private int _current = 0;
		private int _line = 1;
		private bool _isAtEnd
		{
			get
			{
				return _current >= Source.Length;
			}
		}

		private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
		{
			{ "and",    TokenType.AND },
			{ "class",  TokenType.CLASS },
			{ "else",   TokenType.ELSE },
			{ "false",  TokenType.FALSE },
			{ "for",    TokenType.FOR },
			{ "while",  TokenType.WHILE },
			{ "fun",    TokenType.FUN },
			{ "if",     TokenType.IF },
			{ "nil",    TokenType.NIL },
			{ "or",     TokenType.OR },
			{ "print",  TokenType.PRINT },
			{ "return", TokenType.RETURN },
			{ "super",  TokenType.SUPER },
			{ "this",   TokenType.THIS },
			{ "true",   TokenType.TRUE },
			{ "var",    TokenType.VAR },
		};

		public Scanner(string source)
		{
			Source = source;
			Tokens = new List<Token>();
		}

		public List<Token> ScanTokens()
		{
			while (!_isAtEnd)
			{
				// We are at the beginning of the next lexeme
				_start = _current;
				ScanToken();
			}

			Tokens.Add(new Token(TokenType.EOF, "", null, _line));
			return Tokens;
		}

		private void ScanToken()
		{
			char c = Advance();
			switch (c)
			{
				case '(': AddToken(TokenType.LEFT_PAREN); break;
				case ')': AddToken(TokenType.RIGHT_PAREN); break;
				case '{': AddToken(TokenType.LEFT_BRACE); break;
				case '}': AddToken(TokenType.RIGHT_BRACE); break;
				case ',': AddToken(TokenType.COMMA); break;
				case '.': AddToken(TokenType.DOT); break;
				case '-': AddToken(TokenType.MINUS); break;
				case '+': AddToken(TokenType.PLUS); break;
				case ';': AddToken(TokenType.SEMICOLON); break;
				case '*': AddToken(TokenType.STAR); break;

				case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
				case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
				case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
				case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;

				case '/':
					if (Match('/'))
					{
						// A comment goes until the end of the line
						while (Peek() != '\n' && !_isAtEnd)
							Advance();
					}
					else
					{
						AddToken(TokenType.SLASH);
					}
					break;

				case ' ':
				case '\r':
				case '\t':
					// Ignore whitespace
					break;

				case '\n':
					_line++;
					break;

				case '"': MatchString(); break;

				default:
					if (IsDigit(c))
						MatchNumber();
					else if (IsAlpha(c))
						MatchIdentifier();
					else
						error(_line, String.Format("Unexpected character: '{0}'.", Source[_current - 1]));

					break;
			}
		}
		#region Helpers


		private bool IsDigit(char c)
		{
			return c >= '0' && c <= '9';
		}

		private bool IsAlpha(char c)
		{
			return (c >= 'a' && c <= 'z') ||
				   (c >= 'A' && c <= 'Z') ||
					c == '_';
		}

		private bool IsAlphaNumeric(char c)
		{
			return IsAlpha(c) || IsDigit(c);
		}

		private char Peek()
		{
			if (_isAtEnd) return '\0';
			return Source[_current];
		}

		private char PeekNext()
		{
			if (_current + 1 >= Source.Length)
				return '\0';

			return Source[_current + 1];
		}

		private bool Match(char expected)
		{
			if (_isAtEnd)
				return false;

			if (Source[_current] != expected)
				return false;

			_current++;
			return true;
		}

		private void MatchString()
		{
			while (Peek() != '"' && !_isAtEnd)
			{
				if (Peek() == '\n')
					_line++;

				Advance();
			}

			// Enterminated string
			if (_isAtEnd)
			{
				error(_line, "Unterminated string.");
				return;
			}

			// The closing ".
			Advance();

			// Trim the surrounding quotes
			var value = Source.Substring(_start + 1, _current - _start - 2);
			AddToken(TokenType.STRING, value);
		}

		private void MatchIdentifier()
		{
			while (IsAlphaNumeric(Peek()))
				Advance();

			// See if the identifier is a reserved word
			var text = Source.Substring(_start, _current - _start);

			if (Keywords.TryGetValue(text, out var type))
				AddToken(type);
			else
				AddToken(TokenType.IDENTIFIER);
		}

		private void MatchNumber()
		{
			while (IsDigit(Peek()))
				Advance();

			// Look for a fractional part.
			if (Peek() == '.' && IsDigit(PeekNext()))
			{
				// Consume the '.'
				Advance();

				while (IsDigit(Peek()))
					Advance();
			}

			AddToken(TokenType.NUMBER,
				Double.Parse(Source.Substring(_start, _current - _start)));
		}

		private void AddToken(TokenType type, object literal = null)
		{
			var text = Source.Substring(_start, _current - _start);
			Tokens.Add(new Token(type, text, literal, _line));
		}

		private char Advance()
		{
			_current++;
			return Source[_current - 1];
		}

		#endregion	}
	}
}