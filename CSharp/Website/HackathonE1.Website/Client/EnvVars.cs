using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Website.Client
{
	public class EnvVars : IReadOnlyDictionary<string, string>
	{
		public Dictionary<string, string> Variables { get; set; }

		public string this[string key] => Variables[key];

		public IEnumerable<string> Keys => Variables.Keys;

		public IEnumerable<string> Values => Variables.Values;

		public int Count => Variables.Count;

		public bool ContainsKey( string key )
		{
			return Variables.ContainsKey( key );
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return Variables.GetEnumerator();
		}

		public bool TryGetValue( string key, [MaybeNullWhen( false )] out string value )
		{
			return Variables.TryGetValue( key, out value );
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ( Variables as IEnumerable ).GetEnumerator();
		}
	}
}
