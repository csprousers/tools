using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace Excel2CSPro
{
    class WorkbookController
    {
        private static Excel.Application _application = null;
        private static List<Excel.Workbook> _openWorkbooks = new List<Excel.Workbook>();
        private Excel.Workbook _workbook = null;
        private Excel.Worksheet _worksheet = null;

        public static void Close()
        {
            if( _application != null )
            {
                // ignore any errors closing the workbooks or Excel
                try
                {
                    for( int i = _openWorkbooks.Count - 1; i >= 0; i-- )
                        CloseWorkbook(_openWorkbooks[i]);
                }

                catch( Exception )
                {
                }

                try
                {
                    _application.Quit();
                }

                catch( Exception )
                {
                }
            }
        }

        public void OpenWorkbook(string filename)
        {
            if( _workbook != null )
            {
                CloseWorkbook(_workbook);
                _workbook = null;
            }

            if( _application == null )
                _application = new Excel.Application();

            _workbook = _application.Workbooks.Open(filename,Type.Missing,true);
            _openWorkbooks.Add(_workbook);
        }

        public static void CloseWorkbook(Excel.Workbook workbook)
        {
            workbook.Saved = true; // disable any prompts to save the data
            workbook.Close();
            _openWorkbooks.Remove(workbook);
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
