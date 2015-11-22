using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel2CSPro
{
    class WorkbookController
    {
        private static Excel.Application _application = null;
        private Excel.Workbook _workbook = null;
        private Excel.Worksheet _worksheet = null;

        public static void Close()
        {
            if( _application != null )
                _application.Quit();
        }

        public void OpenWorksheet(string filename)
        {
            CloseWorksheet();

            if( _application == null )
                _application = new Excel.Application();

            _workbook = _application.Workbooks.Open(filename,Type.Missing,true);
        }

        public void CloseWorksheet()
        {
            if( _workbook != null )
            {
                _workbook.Saved = true; // disable any prompts to save the data
                _workbook.Close();
                _workbook = null;
            }
        }

        public List<string> WorksheetNames
        {
            get
            {
                List<string> names = new List<string>();

                foreach( Excel.Worksheet worksheet in _workbook.Worksheets )
                    names.Add(worksheet.Name);

                return names;
            }
        }

        public Excel.Worksheet GetWorksheet(int i)
        {
            _worksheet = _workbook.Worksheets[i + 1]; // the worksheets are one-based
            return _worksheet;
        }

        public int ParseStartingRow(string text)
        {
            int value;

            if( !Int32.TryParse(text,out value) )
                throw new Exception(Messages.StartingRowInvalid);

            else if( value < 1 || value > _worksheet.UsedRange.Rows.Count )
                throw new Exception(String.Format(Messages.StartingRowOutOfRange,_worksheet.UsedRange.Rows.Count));

            return value;
        }        

    }
}
