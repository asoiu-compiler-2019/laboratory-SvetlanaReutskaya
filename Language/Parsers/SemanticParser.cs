using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class SemanticParser
    {
        public List<Tree> Commands = new List<Tree>();
        public int Errors;

        public void Analise()
        {
            int counter = 1;
            foreach(var command in Commands)
            {
                var cmdW = command.ChildValues;

                if (cmdW.Count == 1)
                {
                    if (cmdW.First().Value != "create" && cmdW.First().Value != "delete")
                    {
                        Console.WriteLine("Error! Need command word in command " + counter);
                        Errors += 1;
                    }
                    if (cmdW.First().ChildValues.Count != 1)
                    {
                        Console.WriteLine("Error! Need one item to " + cmdW.First().Value + " in command " + counter);
                        Errors += 1;
                    }

                    checkParams(cmdW.First().ChildValues.First(), counter);

                    var expressions = cmdW.First().ChildValues.First().ChildValues;

                    if (expressions.Count == 1)
                    {
                        if (expressions.First().ChildValues[1].Lexeme == Lexemes.KeyTextValue)
                        {
                            expressions.First().ChildValues[0].Lexeme = Lexemes.KeyTextWord;
                            expressions.First().ChildValues[0].Value = "name";
                        }
                    }
                    else
                    {
                        if (expressions.Where(x => x.Value != "==").Count() > 0)
                        {
                            Console.WriteLine("Error! Need only == comparator in command " + counter);
                            Errors += 1;
                        }

                        List<string> par = new List<string>();
                        foreach (var ex in expressions)
                        {
                            if ((ex.ChildValues[0].Lexeme == Lexemes.KeyTextWord && ex.ChildValues[1].Lexeme == Lexemes.KeyDigitValue) ||
                                (ex.ChildValues[0].Lexeme == Lexemes.KeyDigitWord && ex.ChildValues[1].Lexeme == Lexemes.KeyTextValue)
                                )
                            {
                                Console.WriteLine("Error! use digits with digitFields and texts with TextFields in command " + counter);
                                Errors += 1;
                            }
                            if (par.IndexOf(ex.ChildValues[0].Value) != -1)
                            {
                                Console.WriteLine("Error! more then one  qual option parametr in command " + counter);
                                Errors += 1;
                            }
                            else par.Add(ex.ChildValues[0].Value);
                        }
                    }
                }
                else
                {
                    if (cmdW.First().Value == "bind" || cmdW.First().Value == "unbind")
                    {
                        if (cmdW[1].Value != "with")
                        {
                            Console.WriteLine("Error! use with after bind in command " + counter);
                            Errors += 1;
                        }

                        var bindOpt = cmdW[0].ChildValues;
                        var withOpt = cmdW[1].ChildValues;

                        if (bindOpt.Count != 1)
                        {
                            Console.WriteLine("Error! cant bind more then 1 in command " + counter);
                            Errors += 1;
                        }
                        if (bindOpt[0].Value == "count")
                        {
                            Console.WriteLine("Error! count must be after GetAll in command " + counter);
                            Errors += 1;
                        }
                        if (bindOpt[0].Value == "pacient")
                        {
                            Console.WriteLine("Error! pacient must be after with in command " + counter);
                            Errors += 1;
                        }

                        checkParams(bindOpt[0], counter);

                        if (bindOpt[0].ChildValues.Count > 1 ||
                            bindOpt[0].ChildValues[0].Value != "==" ||
                            (bindOpt[0].ChildValues[0].ChildValues[0].Value != "name" &&
                            bindOpt[0].ChildValues[0].ChildValues[0].Value != "code") ||
                            (bindOpt[0].ChildValues[0].ChildValues[0].Lexeme == Lexemes.KeyTextWord && bindOpt[0].ChildValues[0].ChildValues[1].Lexeme == Lexemes.KeyDigitValue) ||
                            (bindOpt[0].ChildValues[0].ChildValues[0].Lexeme == Lexemes.KeyDigitWord && bindOpt[0].ChildValues[0].ChildValues[1].Lexeme == Lexemes.KeyTextValue)
                            )
                        {
                            Console.WriteLine("Error! bind only things with concrete identity in command " + counter);
                            Errors += 1;
                        }

                        if (withOpt.Count > 2)
                        {
                            Console.WriteLine("Error! wrong acces after with in command " + counter);
                            Errors += 1;
                        }
                        if (withOpt[0].Value != "pacient")
                        {
                            Console.WriteLine("Error! must bind with pacient in command " + counter);
                            Errors += 1;
                        }
                        if ((bindOpt[0].Value == "disease" || bindOpt[0].Value == "region") && withOpt.Count > 1)
                        {
                            Console.WriteLine("Error! must bind with only pacient in command " + counter);
                            Errors += 1;
                        }
                        if (bindOpt[0].Value == "medicine" && withOpt.Count == 1)
                        {
                            Console.WriteLine("Error! medicine must bind with pacient and disease in command " + counter);
                            Errors += 1;
                        }
                        if (withOpt.Count == 2 && (withOpt[1].Value != "disease" || bindOpt[0].Value != "medicine"))
                        {
                            Console.WriteLine("Error! only medicine must bind with pacient and disease in command " + counter);
                            Errors += 1;
                        }
                        if (withOpt.Count == 2)
                        {
                            var disOpt = withOpt[1].ChildValues;
                            checkParams(withOpt[1], counter);
                            List<string> dis = new List<string>();
                            foreach (var opt in disOpt)
                            {
                                if ((opt.ChildValues[0].Lexeme == Lexemes.KeyTextWord && opt.ChildValues[1].Lexeme == Lexemes.KeyDigitValue) ||
                                (opt.ChildValues[0].Lexeme == Lexemes.KeyDigitWord && opt.ChildValues[1].Lexeme == Lexemes.KeyTextValue))
                                {
                                    Console.WriteLine("Error! wrong types of disease parametrs in command " + counter);
                                    Errors += 1;
                                }
                                if (dis.IndexOf(opt.ChildValues[0].Value) != -1)
                                {
                                    Console.WriteLine("Error! more then one  qual  disease option parametr in command " + counter);
                                    Errors += 1;
                                }
                                else dis.Add(opt.ChildValues[0].Value);
                            }
                        }

                        var pacientOpt = withOpt[0].ChildValues;
                        checkParams(withOpt[0], counter);
                        List<string> par = new List<string>();
                        foreach (var opt in pacientOpt)
                        {
                            if ((opt.ChildValues[0].Lexeme == Lexemes.KeyTextWord && opt.ChildValues[1].Lexeme == Lexemes.KeyDigitValue) ||
                            (opt.ChildValues[0].Lexeme == Lexemes.KeyDigitWord && opt.ChildValues[1].Lexeme == Lexemes.KeyTextValue))
                            {
                                Console.WriteLine("Error! wrong types of pacient parametrs in command " + counter);
                                Errors += 1;
                            }
                            if (par.IndexOf(opt.ChildValues[0].Value) != -1)
                            {
                                Console.WriteLine("Error! more then one  qual  pacient option parametr in command " + counter);
                                Errors += 1;
                            }
                            else par.Add(opt.ChildValues[0].Value);
                        }

                    }
                    else
                    {
                        if (cmdW.First().Value == "create" || cmdW.First().Value == "delete")
                        {
                            Console.WriteLine("Error! not matched commandWords in command " + counter);
                            Errors += 1;
                        }
                        if (cmdW[1].Value != "check" || cmdW.Count > 2)
                        {
                            Console.WriteLine("Error! use getall with check in command " + counter);
                            Errors += 1;
                        }
                        var getOpt = cmdW[0].ChildValues;
                        var checkOpt = cmdW[1].ChildValues;

                        if (checkOpt.Count > 2 ||
                            (checkOpt.Count == 1 && (checkOpt[0].Value != "pacient" && checkOpt[0].Value != "count")) ||
                            (checkOpt.Count == 2 && (checkOpt[0].Value != "pacient" || checkOpt[1].Value != "count")))
                        {
                            Console.WriteLine("Error! use check with pacient or count in command " + counter);
                            Errors += 1;
                        }
                        checkParams(checkOpt[0], counter);
                        if (checkOpt[0].Value == "pacient")
                        {
                            var pacientOpt = checkOpt[0].ChildValues;
                            List<string> par = new List<string>();
                            foreach (var opt in pacientOpt)
                            {
                                if ((opt.ChildValues[0].Lexeme == Lexemes.KeyTextWord && opt.ChildValues[1].Lexeme == Lexemes.KeyDigitValue) ||
                                (opt.ChildValues[0].Lexeme == Lexemes.KeyDigitWord && opt.ChildValues[1].Lexeme == Lexemes.KeyTextValue))
                                {
                                    Console.WriteLine("Error! wrong types of pacient parametrs in command " + counter);
                                    Errors += 1;
                                }
                                if (par.IndexOf(opt.ChildValues[0].Value) != -1)
                                {
                                    Console.WriteLine("Error! more then one  qual pacient option parametr in command " + counter);
                                    Errors += 1;
                                }
                                else par.Add(opt.ChildValues[0].Value);
                            }
                        }
                        if (checkOpt[0].Value == "count")
                        {
                            if (checkOpt[0].ChildValues.Count > 1 ||
                                checkOpt[0].ChildValues[0].Value != "==" ||
                                checkOpt[0].ChildValues[0].ChildValues[0].Lexeme != Lexemes.KeyDigitWord ||
                                checkOpt[0].ChildValues[0].ChildValues[1].Lexeme != Lexemes.KeyDigitValue)
                            {
                                Console.WriteLine("Error! count has only digit parametr in command " + counter);
                                Errors += 1;
                            }
                        }
                        if (checkOpt.Count==2 && checkOpt[1].Value == "count")
                        {
                            checkParams(checkOpt[1], counter);
                            if (checkOpt[1].ChildValues.Count > 1 ||
                                checkOpt[1].ChildValues[0].Value != "==" ||
                                checkOpt[1].ChildValues[0].ChildValues[0].Lexeme != Lexemes.KeyDigitWord ||
                                checkOpt[1].ChildValues[0].ChildValues[1].Lexeme != Lexemes.KeyDigitValue)
                            {
                                Console.WriteLine("Error! count has only digit parametr in command " + counter);
                                Errors += 1;
                            }
                        }

                        List<string> op = new List<string>();
                        foreach (var opt in getOpt)
                        {
                            var accessOpt = opt.ChildValues;
                            checkParams(opt, counter);

                            List<string> par = new List<string>();
                            foreach (var ac in accessOpt)
                            {
                                if ((ac.ChildValues[0].Lexeme == Lexemes.KeyTextWord && ac.ChildValues[1].Lexeme == Lexemes.KeyDigitValue) ||
                                (ac.ChildValues[0].Lexeme == Lexemes.KeyDigitWord && ac.ChildValues[1].Lexeme == Lexemes.KeyTextValue))
                                {
                                    Console.WriteLine("Error! wrong types of parametrs in command " + counter);
                                    Errors += 1;
                                }
                                if (par.IndexOf(ac.ChildValues[0].Value) != -1)
                                {
                                    Console.WriteLine("Error! more then one  qual option parametr in command " + counter);
                                    Errors += 1;
                                }
                                else par.Add(ac.ChildValues[0].Value);
                            }

                            if (op.IndexOf(opt.Value) != -1)
                            {
                                Console.WriteLine("Error! more then one qual access parametr in command " + counter);
                                Errors += 1;
                            }
                            else op.Add(opt.Value);
                        }

                        if (getOpt.Count > 3)
                        {
                            Console.WriteLine("Error! wrong access in command " + counter);
                            Errors += 1;
                        }

                    }
                }
                counter++;
            }
        }

        void checkParams(Tree access, int cmd)
        {
            if (access.Value == "pacient")
            {
                foreach(var item in access.ChildValues)
                {
                    if(item.ChildValues[0].Value != "code" &&
                        item.ChildValues[0].Value != "name" &&
                        item.ChildValues[0].Value != "age")
                    {
                        Console.WriteLine("Error! wrong pacient param in command " + cmd);
                        Errors += 1;
                    }
                }
            }
            if (access.Value == "region" || access.Value == "medicine")
            {
                foreach (var item in access.ChildValues)
                {
                    if (item.ChildValues[0].Value != "code" &&
                       item.ChildValues[0].Value != "name")
                    {
                        Console.WriteLine("Error! wrong region/medicine param in command " + cmd);
                        Errors += 1;
                    }
                }
            }
            if (access.Value == "disease")
            {
                foreach (var item in access.ChildValues)
                {
                    if (item.ChildValues[0].Value != "code" &&
                       item.ChildValues[0].Value != "name" &&
                       item.ChildValues[0].Value != "heavy")
                    {
                        Console.WriteLine("Error! wrong disease param in command " + cmd);
                        Errors += 1;
                    }
                }
            }
        } 
    }
}
