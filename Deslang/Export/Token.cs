using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace Glory
{
	#region Token
	/// <summary>
	/// Represents a single token produced from a lexer/tokenizer, and consumed by a parser
	/// A token contains the symbol, the value, and the location information for each lexeme returned from a lexer/tokenizer
	/// </summary>
	struct Token
	{
		/// <summary>
		/// Indicates the name of the symbol
		/// </summary>
		public string Symbol;
		/// <summary>
		/// Indicates the symbol's id
		/// </summary>
		public int SymbolId;
		/// <summary>
		/// Indicates the one based line on which the token occurs
		/// </summary>
		public int Line;
		/// <summary>
		/// Indicates the one based column on which the token occurs
		/// </summary>
		public int Column;
		/// <summary>
		/// Indicates the zero based position on which the token occurs
		/// </summary>
		public long Position;
		/// <summary>
		/// Indicates the token value
		/// </summary>
		public string Value;
		/// <summary>
		/// Gets a string representation of the token
		/// </summary>
		/// <returns>A string that represents the token</returns>
		public override string ToString()
		{
			// Looks like: Example(123) : Foo
			return string.Concat(Symbol, "(", string.Concat(SymbolId.ToString(), ") : ", Value));
		}
	}
	#endregion
}
