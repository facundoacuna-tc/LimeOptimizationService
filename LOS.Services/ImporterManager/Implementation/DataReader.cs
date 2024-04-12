using LOS.Services.ImporterManager.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LOS.Services.ImporterManager.Implementation
{
    public class CSVDataReader : ICSVDataReader
    {
        private StreamReader _file;
        private char _delimiter;
        /* stores the header and values of csv and also virtual*/
        private string _csvHeaderstring = "", _csvlinestring = "", _virtuallineString = "";
        private bool _firstRowHeader = true;

        private string[] _header;


        protected readonly ILogger<CSVDataReader> _logger;

        public CSVDataReader(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CSVDataReader>();
        }


        public string[] Header
        {
            get { return _header; }
        }

        private OrderedDictionary headercollection = new OrderedDictionary();

        private string[] _line;

        /// <summary>
        /// Returns an array of strings from the current line of csv file. Call Read() method to read the next line/record of csv file. 
        /// </summary>
        public string[] Line
        {
            get
            {
                return _line;
            }
        }

        private int recordsaffected;
        private bool _iscolumnlocked = false;

        /// <summary>
        /// Sets configuration for CSV reader
        /// </summary>
        /// <param name="filePath">Path to the csv file.</param>
        /// <param name="delimiter">delimiter charecter used in csv file.</param>
        /// <param name="firstRowHeader">specify the csv got a header in first row or not. Default is true and if argument is false then auto header 'ROW_xx will be used as per the order of columns.</param>
        public void DatareaderConfiguration(string filePath, char delimiter, string fileType, bool firstRowHeader = true)
        {
            SetAccessFileType(filePath, fileType);

            _delimiter = delimiter;
            _firstRowHeader = firstRowHeader;
            if (_firstRowHeader == true)
            {
                Read();
                _csvHeaderstring = _csvlinestring;
                _header = ReadRow(_csvHeaderstring);
                foreach (var item in _header) //check for duplicate headers and create a header record.
                {
                    if (headercollection.Contains(item) == true)
                    {
                        var message = "Duplicate found in CSV header. Cannot create a CSV reader instance with duplicate header";
                        _logger.LogError(message);
                    }
                    headercollection.Add(item, null);
                }
            }
            else
            {
                //just open and close the file with read of first line to determine how many rows are there and then add default rows as  row1,row2 etc to collection.
                Read();
                _csvHeaderstring = _csvlinestring;
                _header = ReadRow(_csvHeaderstring);
                int i = 0;
                _csvHeaderstring = "";
                foreach (var item in _header)//read each column and create a dummy header.
                {
                    headercollection.Add("COL_" + i.ToString(), null);
                    _csvHeaderstring = _csvHeaderstring + "COL_" + i.ToString() + _delimiter;
                    i++;
                }
                _csvHeaderstring.TrimEnd(_delimiter);
                _header = ReadRow(_csvHeaderstring);
                Close(); //close and repoen to get the record position to begining.

                SetAccessFileType(filePath, fileType);
            }
            _iscolumnlocked = false; //setting this to false since above read is called internally during constructor and actual user read() didnot start.
            _csvlinestring = "";
            _line = null;
            recordsaffected = 0;
        }


        /// <summary>
        /// Configures the access type
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileType"></param>
        private void SetAccessFileType(string filePath, string fileType)
        {
            try
            {
                if (fileType == "Local")
                    _file = File.OpenText(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }


            if (fileType == "Remote")
                _file = GetCSVFromRemoteUrl(filePath);
        }


        /// <summary>
        /// Gets stream from remote input file
        /// </summary>
        /// <param name="url">remote url for input file</param>
        /// <returns>data stream</returns>
        private StreamReader GetCSVFromRemoteUrl(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                var stream = response.GetResponseStream();
                return new StreamReader(stream);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <returns>array of strings from csv line</returns>
        public bool Read()
        {
            var result = !_file.EndOfStream;
            if (result == true)
            {
                _csvlinestring = _file.ReadLine();
                if (_virtuallineString == "")
                    _line = ReadRow(_csvlinestring);
                else
                    _line = ReadRow(_virtuallineString + _delimiter + _csvlinestring);
                recordsaffected++;
            }
            if (_iscolumnlocked == false)
                _iscolumnlocked = true;
            return result;
        }


        /// <summary>
        /// Validates a row of data from a CSV file
        /// </summary>
        /// <returns>array of strings from csv line</returns>
        private string[] ReadRow(string line)
        {
            List<string> lines = new List<string>();
            if (String.IsNullOrEmpty(line) == true)
                return null;

            int pos = 0;
            int rows = 0;
            while (pos < line.Length)
            {
                string value;

                // Special handling for quoted field
                if (line[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < line.Length)
                    {
                        // Test for quote character
                        if (line[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= line.Length || line[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = line.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < line.Length && line[pos] != _delimiter)
                        pos++;
                    value = line.Substring(start, pos - start);
                }
                // Add field to list
                if (rows < lines.Count)
                    lines[rows] = value;
                else
                    lines.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < line.Length && line[pos] != _delimiter)
                    pos++;
                if (pos < line.Length)
                    pos++;
            }
            return lines.ToArray();
        }

        public void Close()
        {
            _file.Close();
            _file.Dispose();
            _file = null;
        }


        /// <summary>
        /// Gets a value that indicates the depth of nesting for the current row.
        /// </summary>
        public int Depth
        {
            get { return 1; }
        }

        public DataTable GetSchemaTable()
        {
            DataTable t = new DataTable();
            t.Rows.Add(Header);
            return t;
        }

        public bool IsClosed
        {
            get { return _file == null; }
        }

        public bool NextResult()
        {
            return Read();
        }


        /// <summary>
        /// Retuens how many records read so far.
        /// </summary>
        public int RecordsAffected
        {
            get { return recordsaffected; }
        }

        public void Dispose()
        {
            if (_file != null)
            {
                _file.Dispose();
                _file = null;
            }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get { return Header.Length; }
        }

        public bool GetBoolean(int i)
        {
            return Boolean.Parse(Line[i]);
        }

        public byte GetByte(int i)
        {
            return Byte.Parse(Line[i]);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return Char.Parse(Line[i]);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            return (IDataReader)this;
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return DateTime.Parse(Line[i]);
        }

        public decimal GetDecimal(int i)
        {
            return Decimal.Parse(Line[i]);
        }

        public double GetDouble(int i)
        {
            return Double.Parse(Line[i]);
        }

        public Type GetFieldType(int i)
        {
            return typeof(String);
        }

        public float GetFloat(int i)
        {
            return float.Parse(Line[i]);
        }

        public Guid GetGuid(int i)
        {
            return Guid.Parse(Line[i]);
        }

        public short GetInt16(int i)
        {
            return Int16.Parse(Line[i]);
        }

        public int GetInt32(int i)
        {
            return Int32.Parse(Line[i]);
        }

        public long GetInt64(int i)
        {
            return Int64.Parse(Line[i]);
        }

        public string GetName(int i)
        {
            return Header[i];
        }

        public int GetOrdinal(string name)
        {
            int result = -1;
            for (int i = 0; i < Header.Length; i++)
                if (Header[i] == name)
                {
                    result = i;
                    break;
                }
            return result;
        }

        public string GetString(int i)
        {
            return Line[i];
        }

        public object GetValue(int i)
        {
            return Line[i];
        }

        public int GetValues(object[] values)
        {
            values = Line;
            return 1;
        }

        public bool IsDBNull(int i)
        {
            return string.IsNullOrWhiteSpace(Line[i]);
        }

        public object this[string name]
        {
            get { return Line[GetOrdinal(name)]; }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }

    }
}
