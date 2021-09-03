namespace Cobra.src
{
	public enum TokenType
	{
		// Sinlge-character tokens
		LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

		// Comparison tokens
		BANG, BANG_EQUAL,
		EQUAL, EQUAL_EQUAL,
		GREATER, GREATER_EQUAL,
		LESS, LESS_EQUAL,

		// Literals
		IDENTIFIER, STRING, NUMBER,

		// Keywords
		AND, OR, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL,
		PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

		EOF
	}
}