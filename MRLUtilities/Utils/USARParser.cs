using System;
using System.Collections;
using System.Collections.Specialized;

namespace MRL.Utils
{

    /// <summary>
    /// this class created for parsing simulation messages
    /// and get value of each parts that splited by delimiter
    /// </summary>
    public class USARParser
    {
        public string rawData;
        public string type;
        public int size;

        public NameValueCollection[] segments;


        private string delimiter;

        /// <summary>Creates a new instance of USARParser </summary>
        public USARParser(string data)
        {
            rawData = data;
            delimiter = " \t\n\r\f}";
            parse();
        }

        public USARParser(string data, string del)
        {
            rawData = data;
            delimiter = del;
            parse();
        }

        protected internal void parse()
        {
            Tokenizer st = new Tokenizer(rawData, "{");

            try
            {
                if (st.HasMoreTokens())
                    type = st.NextToken().Trim();
                else
                    return;

                size = st.Count;

                segments = new NameValueCollection[size];

                string Tmp, Name, Value;

                Tokenizer tst = null;

                for (int i = 0; st.HasMoreTokens(); i++)
                {
                    Tmp = st.NextToken().Trim();
                    Tmp = Tmp.Substring(0, Tmp.Length - 1);
                    tst = new Tokenizer(Tmp, delimiter);

                    segments[i] = new NameValueCollection();

                    while (tst.HasMoreTokens())
                    {
                        Name = tst.NextToken();
                        Value = tst.NextToken();
                        segments[i].Add(Name, Value);
                    }
                }
            }
            catch (Exception e)
            {
                USARLog.println(" >> " + e.ToString(), this.ToString());
            }
        }

        public NameValueCollection getSegment(int index)
        {
            if (index < 0 || index >= size) return null;
            return segments[index];
        }

        public NameValueCollection getSegment(string name)
        {
            int i = 0;
            for (i = 0; i < size; i++) if (segments[i].Get(name) != null) break;
            if (i == size) return null;

            return segments[i];
        }

        public String getString(string name)
        {
            int i = 0;
            for (i = 0; i < size; i++) if (segments[i].Get(name) != null) break;
            if (i == size) return null;

            return segments[i].Get(name);
        }

        public static bool[] parseBools(string str, string delim, bool inverse)
        {
            Tokenizer st = new Tokenizer(str, delim);
            try
            {
                bool[] res = new bool[st.Count];

                for (int i = 0; st.HasMoreTokens(); i++)
                {
                    int res_i = int.Parse(st.NextToken());
                    if (!inverse)
                        res[i] = (res_i == 1) ? true : false;
                    else
                        res[i] = (res_i == 1) ? false : true;
                }

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static int[] parseInts(string str, string delim)
        {
            Tokenizer st = new Tokenizer(str, delim);
            try
            {
                int[] res = new int[st.Count];

                for (int i = 0; st.HasMoreTokens(); i++)
                    res[i] = int.Parse(st.NextToken());

                return res;
            }
            catch (Exception e)
            {
                USARLog.println("USARParser.parseInts" + " >> " + e.ToString(), "USARParser");
                return null;
            }
        }

        public static float[] parseFloats(string str, string delim)
        {
            Tokenizer st = new Tokenizer(str, delim);
            try
            {
                float[] res = new float[st.Count];

                for (int i = 0; st.HasMoreTokens(); i++)
                    res[i] = System.Single.Parse(st.NextToken());

                return res;
            }
            catch (Exception e)
            {
                USARLog.println("USARParser.parseFloats" + " >> " + e.ToString(), "USARParser");
                return null;
            }
        }

        public static double[] parseDoubles(string str, string delim)
        {
            Tokenizer st = new Tokenizer(str, delim);
            try
            {
                double[] res = new double[st.Count];

                for (int i = 0; st.HasMoreTokens(); i++)
                {
                    string tmp = st.NextToken();
                    res[i] = tmp.Contains("null") ? -1 : double.Parse(tmp);
                }

                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string[] parseStrings(string str, string delim)
        {
            Tokenizer st = new Tokenizer(str, delim);
            try
            {
                string[] res = new string[st.Count];

                for (int i = 0; st.HasMoreTokens(); i++)
                    res[i] = st.NextToken();

                return res;
            }
            catch (Exception e)
            {
                USARLog.println("USARParser.parseStrings" + " >> " + e.ToString(), "USARParser");
                return null;
            }
        }

        public static float getConvertionFactor(UnitType src, UnitType des)
        {
            if ((int)src >= USARConstant.UNIT_NAME.Length)
                src = (UnitType)Enum.ToObject(typeof(UnitType), USARConstant.UNIT_NAME.Length - 1);

            if ((int)des >= USARConstant.UNIT_NAME.Length)
                des = (UnitType)Enum.ToObject(typeof(UnitType), USARConstant.UNIT_NAME.Length - 1);

            return USARConstant.UNIT_TRANS[(int)src * USARConstant.UNIT_NAME.Length + (int)des];
        }

        public static float convertUnit(int input, UnitType src, UnitType des)
        {
            return input * getConvertionFactor(src, des);
        }

        public static float convertUnit(float input, UnitType src, UnitType des)
        {
            return input * getConvertionFactor(src, des);
        }

        public static double convertUnit(double input, UnitType src, UnitType des)
        {
            return input * getConvertionFactor(src, des);
        }

        public static float[] convertUnit(int[] inputs, UnitType src, UnitType des)
        {
            float[] res;

            float factor = getConvertionFactor(src, des);

            res = new float[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                res[i] = inputs[i] * factor;

            return res;
        }

        public static float[] convertUnit(float[] inputs, UnitType src, UnitType des)
        {
            float[] res;

            float factor = getConvertionFactor(src, des);

            res = new float[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                res[i] = inputs[i] * factor;

            return res;
        }

        public static double[] convertUnit(double[] inputs, UnitType src, UnitType des)
        {
            double[] res;

            float factor = getConvertionFactor(src, des);

            res = new double[inputs.Length];
            for (int i = 0; i < inputs.Length; i++)
                res[i] = inputs[i] * factor;

            return res;
        }

        public override string ToString()
        {
            string res = "type=" + type + "\n";

            for (int i = 0; i < size; i++)
            {
                res += ("seg_" + i + "=" + segments[i].ToString() + "\n");
            }

            return res;
        }

    }

    /// <summary>
    /// The class performs token processing in strings
    /// </summary>
    public class Tokenizer : IEnumerator
    {
        /// Position over the string
        private long currentPos = 0;

        /// Include demiliters in the results.
        private bool includeDelims = false;

        /// Char representation of the String to tokenize.
        private char[] chars = null;

        //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
        private string delimiters = " \t\n\r\f";

        /// <summary>
        /// Initializes a new class instance with a specified string to process
        /// </summary>
        /// <param name="source">String to tokenize</param>
        public Tokenizer(string source)
        {
            this.chars = source.ToCharArray();
        }

        /// <summary>
        /// Initializes a new class instance with a specified string to process
        /// and the specified token delimiters to use
        /// </summary>
        /// <param name="source">String to tokenize</param>
        /// <param name="delimiters">String containing the delimiters</param>
        public Tokenizer(string source, string delimiters)
            : this(source)
        {
            this.delimiters = delimiters;
        }

        /// <summary>
        /// Initializes a new class instance with a specified string to process, the specified token 
        /// delimiters to use, and whether the delimiters must be included in the results.
        /// </summary>
        /// <param name="source">String to tokenize</param>
        /// <param name="delimiters">String containing the delimiters</param>
        /// <param name="includeDelims">Determines if delimiters are included in the results.</param>
        public Tokenizer(string source, string delimiters, bool includeDelims)
            : this(source, delimiters)
        {
            this.includeDelims = includeDelims;
        }

        /// <summary>
        /// Returns the next token from the token list
        /// </summary>
        /// <returns>The string value of the token</returns>
        public string NextToken()
        {
            return NextToken(this.delimiters);
        }

        /// <summary>
        /// Returns the next token from the source string, using the provided
        /// token delimiters
        /// </summary>
        /// <param name="delimiters">String containing the delimiters to use</param>
        /// <returns>The string value of the token</returns>
        public string NextToken(string delimiters)
        {
            //According to documentation, the usage of the received delimiters should be temporary (only for this call).
            //However, it seems it is not true, so the following line is necessary.
            this.delimiters = delimiters;

            //at the end 
            if (this.currentPos == this.chars.Length)
                throw new ArgumentOutOfRangeException();

            //if over a delimiter and delimiters must be returned
            else if ((Array.IndexOf(delimiters.ToCharArray(), chars[this.currentPos]) != -1)
                      && this.includeDelims)
                return "" + this.chars[this.currentPos++];

            //need to get the token wo delimiters.
            else
                return nextToken(delimiters.ToCharArray());
        }

        //Returns the nextToken wo delimiters
        private string nextToken(char[] delimiters)
        {
            string token = "";
            long pos = this.currentPos;

            //skip possible delimiters
            while (Array.IndexOf(delimiters, this.chars[currentPos]) != -1)
                //The last one is a delimiter (i.e there is no more tokens)
                if (++this.currentPos == this.chars.Length)
                {
                    this.currentPos = pos;
                    throw new ArgumentOutOfRangeException();
                }

            //getting the token
            while (Array.IndexOf(delimiters, this.chars[this.currentPos]) == -1)
            {
                token += this.chars[this.currentPos];
                //the last one is not a delimiter
                if (++this.currentPos == this.chars.Length)
                    break;
            }
            return token;
        }

        /// <summary>
        /// Determines if there are more tokens to return from the source string
        /// </summary>
        /// <returns>True or false, depending if there are more tokens</returns>
        public bool HasMoreTokens()
        {
            //keeping the current pos
            long pos = this.currentPos;

            try
            {
                this.NextToken();
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            finally
            {
                this.currentPos = pos;
            }
            return true;
        }

        /// <summary>
        /// Remaining tokens count
        /// </summary>
        public int Count
        {
            get
            {
                //keeping the current pos
                long pos = this.currentPos;
                int i = 0;

                try
                {
                    while (true)
                    {
                        this.NextToken();
                        i++;
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    this.currentPos = pos;
                    return i;
                }
            }
        }

        /// <summary>
        ///  Performs the same action as NextToken.
        /// </summary>
        public Object Current
        {
            get
            {
                return (Object)this.NextToken();
            }
        }

        /// <summary>
        //  Performs the same action as HasMoreTokens.
        /// </summary>
        /// <returns>True or false, depending if there are more tokens</returns>
        public bool MoveNext()
        {
            return this.HasMoreTokens();
        }

        /// <summary>
        /// do nothing in reset
        /// </summary>
        public void Reset()
        {
        }

    }

}
