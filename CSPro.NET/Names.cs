using System;
using System.Text.RegularExpressions;

namespace CSPro
{
    public class Names
    {
        public const int MaxLength = 32;

        public static bool IsValid(string name)
        {
            if( name.Length < 1 || name.Length > MaxLength )
                return false;

            // the first character must be a letter
            if( !Char.IsLetter(name[0]) )
                return false;

            // the other characters must be capitalized letters, numbers, or underscores
            if( !Regex.IsMatch(name,@"^[A-Z0-9_]+$") )
                return false;

            // the last character cannot be an underscore
            return name[name.Length - 1] != '_';
        }
    }
}
