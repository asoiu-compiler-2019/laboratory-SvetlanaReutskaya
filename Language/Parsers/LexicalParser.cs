using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class LexicalParser
    {
        static string[] ReservedMainWords = { "bind", "unbind", "create", "delete", "getall"};
        static string[] ReservedHelpWords = { "check", "with" };
        static string[] AccessWords = { "pacient", "disease", "region", "medicine", "count"};
        static char[] Limiters = {':'};
        static char[] CommandLimiters = {';'};
        static char[] AccessLimiters = { '(', ')'};
        static string[] Comparators = { "==", "~~", "!=" };
        static string[] Delimiters = { "&&" };
        static string[] KeyTextWords = { "name", "region", "heavy" };
        static string[] KeyDigitWords = {"age", "code"};

        public List<Tuple<Lexemes, string>> LexemesList = new List<Tuple<Lexemes, string>>();


        public void Parse(string path)
        {
            LexemesList.Clear();
            try
            {
                string[] lines = System.IO.File.ReadAllLines(path);
                
                foreach (string line in lines)
                {
                    string word = "";
                    foreach (char symbol in line)
                    {
                        if (symbol != ' ')
                        {
                            bool WordEnd = false;
                            var sb = Convert.ToChar(symbol.ToString().ToLower());

                            if (Limiters.Contains(sb) || AccessLimiters.Contains(sb) || CommandLimiters.Contains(sb) || (sb=='&' && word !="" && word.Last()!='&')) WordEnd = true;
                            else word += sb;

                            if (word != "")
                            {
                                if (ReservedMainWords.Contains(word))
                                {
                                    LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.ReservedMainWord, word.ToString()));
                                    word = "";
                                }
                                else if (ReservedHelpWords.Contains(word))
                                {
                                    LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.ReservedHelpWord, word.ToString()));
                                    word = "";
                                }
                                else if (AccessWords.Contains(word))
                                {
                                    LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.AccessWord, word.ToString()));
                                    word = "";
                                }
                                else if (Comparators.Contains(word))
                                {
                                    LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.Comparator, word.ToString()));
                                    word = "";
                                }
                                else if (KeyTextWords.Contains(word))
                                {
                                    LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.KeyTextWord, word.ToString()));
                                    word = "";
                                }
                                else if (Delimiters.Contains(word))
                                {
                                    LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.Delimiter, word.ToString()));
                                    word = "";
                                }
                                else if (KeyDigitWords.Contains(word))
                                {
                                    LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.KeyDigitWord, word.ToString()));
                                    word = "";
                                }
                                else if (WordEnd)
                                {
                                    if (word.Any(c => char.IsLetter(c))) LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.KeyTextValue, word.ToString()));
                                    else LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.KeyDigitValue, word.ToString()));
                                    word = "";
                                }
                            }

                            if (Limiters.Contains(sb))
                            {
                                LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.Limiter, sb.ToString()));
                                WordEnd = true;
                            }
                            else if (AccessLimiters.Contains(sb))
                            {
                                LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.AccessLimiter, sb.ToString()));
                                WordEnd = true;
                            }
                            else if (CommandLimiters.Contains(sb))
                            {
                                LexemesList.Add(new Tuple<Lexemes, string>(Lexemes.CommandLimiter, sb.ToString()));
                                WordEnd = true;
                            }

                            if(sb == '&' && word == "" && LexemesList.Last().Item1!=Lexemes.Delimiter)
                                word += sb;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void Print()
        {
            foreach(var item in LexemesList)
            {
                Console.WriteLine("LexemType: " + item.Item1 + ", lexemValue: " + item.Item2);
            }
        }
    }
}
