using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSPro
{
    public class Names
    {
        public static bool IsValid(string name)
        {
            if( name.Length < 1 || name.Length > Defines.MaxNameLength )
                return false;

            // the first character must be a letter
            if( !Char.IsLetter(name[0]) )
                return false;

            // the other characters must be capitalized letters, numbers, or underscores
            if( !Regex.IsMatch(name,@"^[A-Z0-9_]+$") )
                return false;

            // the last character cannot be an underscore
            if( name[name.Length - 1] == '_' )
                return false;

            return !IsReserved(name);
        }

        public static string CreateFromLabel(string label)
        {
            string name = label.Trim().ToUpper();

            // first remove any non-letter characters at the beginning of the string
            while( name.Length != 0 && !Char.IsLetter(name[0]) )
                name = name.Substring(1);

            // remove any invalid characters from the string and replace them with underscores
            bool lastCharWasUnderscore = false;

            for( int i = 0; i < name.Length; i++ )
            {
                if( name[i] ==  '_' )
                    lastCharWasUnderscore = true;

                else if( !Char.IsLetterOrDigit(name[i]) )
                {
                    name = String.Format("{0}{1}{2}",name.Substring(0,i),lastCharWasUnderscore ? "" : "_",name.Substring(i + 1));
                    i--;
                }

                else
                    lastCharWasUnderscore = false;
            }

            // if the last character is an underscore, remove it
            if( name.Length != 0 && name[name.Length - 1] == '_' )
                name = name.Substring(0,name.Length - 1);

            // we potentially have some more work to get to a valid name
            string baseName = name.Length == 0 ? "NAME" : name;
            int itr = 1;

            while( !IsValid(name) )
            {
                string itrString = itr.ToString();

                if( ( baseName.Length + itrString.Length ) > Defines.MaxNameLength )
                    baseName = baseName.Substring(0,Defines.MaxNameLength - itrString.Length);

                name = baseName + itrString;

                itr++;                
            }

            return name;
        }

        private static HashSet<string> _reservedNames = null;

        public static bool IsReserved(string name)
        {
            if( _reservedNames == null )
            {
                string[] csproReservedNames = new string[]
                {
                    "ABS",
                    "ACCEPT",
                    "ADD",
                    "ADJLBA",
                    "ADJLBI",
                    "ADJUBA",
                    "ADJUBI",
                    "ADVANCE",
                    "ALIAS",
                    "ALL",
                    "ALPHA",
                    "AND",
                    "ARRAY",
                    "ASCENDING",
                    "AVERAGE",
                    "BOX",
                    "BREAK",
                    "BY",
                    "CASE",
                    "CASE_ID",
                    "CELL",
                    "CHANGEKEYBOARD",
                    "CLEAR",
                    "CLOSE",
                    "CMCODE",
                    "COLUMN",
                    "COMPARE",
                    "CONCAT",
                    "CONFIRM",
                    "CONNECTION",
                    "COUNT",
                    "COUNTNONSPECIAL",
                    "COUNTVALID",
                    "CROSSTAB",
                    "CUROCC",
                    "DATEADD",
                    "DATEDIFF",
                    "DATEVALID",
                    "DECLARE",
                    "DEFAULT",
                    "DELCASE",
                    "DELETE",
                    "DEMENU",
                    "DEMODE",
                    "DENOM",
                    "DESCENDING",
                    "DIRCREATE",
                    "DIREXIST",
                    "DIRLIST",
                    "DISJOINT",
                    "DISPLAY",
                    "DO",
                    "EACH",
                    "EDIT",
                    "EDITNOTE",
                    "ELSE",
                    "ELSEIF",
                    "END",
                    "ENDBOX",
                    "ENDCASE",
                    "ENDDO",
                    "ENDFOR",
                    "ENDGROUP",
                    "ENDIF",
                    "ENDLEVEL",
                    "ENDLOGIC",
                    "ENDRECODE",
                    "ENDRECODE)",
                    "ENDSECT",
                    "ENDUNIT",
                    "ENTER",
                    "ERRMSG",
                    "EXCLUDE",
                    "EXEC",
                    "EXECPFF",
                    "EXECSYSTEM",
                    "EXIT",
                    "EXP",
                    "EXPLICIT",
                    "EXPORT",
                    "FILE",
                    "FILECONCAT",
                    "FILECOPY",
                    "FILECREATE",
                    "FILEDELETE",
                    "FILEEMPTY",
                    "FILEEXIST",
                    "FILENAME",
                    "FILEREAD",
                    "FILERENAME",
                    "FILESIZE",
                    "FILEWRITE",
                    "FIND",
                    "FLOAT",
                    "FOR",
                    "FORMAT",
                    "FREQ",
                    "FREQUENCY",
                    "FUNCTION",
                    "GETBUFFER",
                    "GETCAPTURETYPE",
                    "GETDECK",
                    "GETDEVICEID",
                    "GETIMAGE",
                    "GETLABEL",
                    "GETLANGUAGE",
                    "GETNOTE",
                    "GETOCCLABEL",
                    "GETOPERATORID",
                    "GETORIENTATION",
                    "GETOS",
                    "GETRECORD",
                    "GETSYMBOL",
                    "GETUSERNAME",
                    "GETVALUE",
                    "GETVALUEALPHA",
                    "GETVALUENUMERIC",
                    "GLOBAL",
                    "GPS",
                    "GROUP",
                    "HAS",
                    "HIDEOCC",
                    "HIGH",
                    "HIGHEST",
                    "HIGHLIGH",
                    "HIGHLIGHTED",
                    "HOTDECK",
                    "IF",
                    "IMPLICIT",
                    "IMPUTE",
                    "IN",
                    "INC",
                    "INCLUDE",
                    "INSERT",
                    "INT",
                    "INTERVAL",
                    "INTERVALS",
                    "INVALUESET",
                    "IOERROR",
                    "ISPARTIAL",
                    "ISSA",
                    "ITEM",
                    "ITEMLIST",
                    "KEY",
                    "KILLFOCUS",
                    "LAYER",
                    "LENGTH",
                    "LEVEL",
                    "LEVELID",
                    "LINKED",
                    "LINT",
                    "LIST",
                    "LOADCASE",
                    "LOCATE",
                    "LOCKED",
                    "LOG",
                    "LOW",
                    "LOWERS",
                    "MAKETEXT",
                    "MAX",
                    "MAXOCC",
                    "MAXVALUE",
                    "MEAN",
                    "MIN",
                    "MINVALUE",
                    "MISSING",
                    "MODIFY",
                    "MOVE",
                    "MULTIPLE",
                    "NEXT",
                    "NMEMBERS",
                    "NOAUTO",
                    "NOBREAK",
                    "NOCCURS",
                    "NOCONFIRM",
                    "NOFREQ",
                    "NOINPUT",
                    "NOPRINT",
                    "NOT",
                    "NOTAPPL",
                    "NOWRITE",
                    "NUMERIC",
                    "ONFOCUS",
                    "ONOCCCHANGE",
                    "OPEN",
                    "OR",
                    "OUTOFRANGE",
                    "PAGE",
                    "PATHNAME",
                    "POS",
                    "POSCHAR",
                    "POSTCALC",
                    "POSTPROC",
                    "PREPROC",
                    "PROC",
                    "PROMPT",
                    "PUBLISHDATE",
                    "PUTDECK",
                    "PUTNOTE",
                    "RANDOM",
                    "RANDOMIN",
                    "RANDOMIZEVS",
                    "REC_NAME",
                    "REC_TYPE",
                    "RECODE",
                    "RECORD",
                    "REENTER",
                    "RELATION",
                    "RETRIEVE",
                    "ROUND",
                    "ROW",
                    "SAVE",
                    "SAVEPARTIAL",
                    "SEED",
                    "SEEK",
                    "SEEKMAX",
                    "SEEKMIN",
                    "SELCASE",
                    "SELECT",
                    "SET",
                    "SETCAPTUREPOS",
                    "SETCAPTURETYPE",
                    "SETFILE",
                    "SETFONT",
                    "SETLANGUAGE",
                    "SETLB",
                    "SETOCCLABEL",
                    "SETORIENTATION",
                    "SETOUTPUT",
                    "SETUB",
                    "SETVALUE",
                    "SETVALUESET",
                    "SETVALUESETS",
                    "SHOW",
                    "SHOWARRAY",
                    "SHOWOCC",
                    "SINT",
                    "SKIP",
                    "SMEAN",
                    "SOCCURS",
                    "SORT",
                    "SPECIAL",
                    "SPECIALVALUES",
                    "SPECIFIC",
                    "SPLIT",
                    "SQRT",
                    "STABLE",
                    "STAT",
                    "STATISTICS",
                    "STOP",
                    "STRING",
                    "STRIP",
                    "STUB",
                    "SUBTABLE",
                    "SUM",
                    "SUMMARY",
                    "SWAP",
                    "SYNC",
                    "SYSDATE",
                    "SYSPARM",
                    "SYSTIME",
                    "TABLE",
                    "TABLOGIC",
                    "TALLY",
                    "TBD",
                    "TBLCOL",
                    "TBLLAY",
                    "TBLMED",
                    "TBLROW",
                    "TBLSUM",
                    "THEN",
                    "TITLE",
                    "TO",
                    "TOLOWER",
                    "TONUMBER",
                    "TOTOCC",
                    "TOUPPER",
                    "TRACE",
                    "UNDEFINED",
                    "UNIT",
                    "UNIVERSE",
                    "UNTIL",
                    "UPDATE",
                    "USERBAR",
                    "USING",
                    "VARYING",
                    "VERIFY",
                    "VISUALVALUE",
                    "VOCCURS",
                    "VSET",
                    "WEIGHTED",
                    "WHERE",
                    "WHILE",
                    "WORKDICT",
                    "WRITE",
                    "WRITEACL",
                    "WRITECASE",
                    "WRITEFORM",
                    "WRITEFORMHTML",
                    "XTAB"
                };

                // set up the HashSet with the reserved words
                _reservedNames = new HashSet<string>();

                foreach( string text in csproReservedNames )
                    _reservedNames.Add(text);
            }

            return _reservedNames.Contains(name);
        }
 
    }
}
