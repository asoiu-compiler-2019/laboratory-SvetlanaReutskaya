using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    public class SyntaxParser
    {
        public List<Tuple<Lexemes, string>> LexemesList;

        public List<Tree> Commands = new List<Tree>();

        public int ErrorsCount { get; set; }

        public void Parse()
        {
            Commands.Clear();
            int count = 0;
            ErrorsCount = 0;

            while (LexemesList.IndexOf(new Tuple<Lexemes, string>(Lexemes.CommandLimiter, ";")) != -1) 
            {
                count++;
                var ind = LexemesList.IndexOf(new Tuple<Lexemes, string>(Lexemes.CommandLimiter, ";"));
                var nextCommand = LexemesList.GetRange(0, ind);
                LexemesList.RemoveRange(0, ind+1);

                Tree command = new Tree();
                command.Value = "Command " + count;

                var mainW = nextCommand.Where(x => x.Item1 == Lexemes.ReservedMainWord).Select(x=>x).ToList();
                if (mainW.Count != 1)
                {
                    Console.WriteLine("Error (Hasn`t command word) in command " + count);
                    ErrorsCount += 1;
                    break;
                }
                int mainWId = nextCommand.IndexOf(mainW[0]);
                var mhelpW = nextCommand.Where(x => x.Item1 == Lexemes.ReservedHelpWord).Select(x => x).ToList();
                int mhelpWId = mhelpW.Count!=0?nextCommand.IndexOf(mhelpW[0]): -1;
                if (mhelpW.Count > 1)
                {
                    Console.WriteLine("Error (Too many command word) in command " + count);
                    ErrorsCount += 1;
                    break;
                }
                if (mainWId != 0 || nextCommand[mainWId + 1].Item1 != Lexemes.Limiter)
                {
                    Console.WriteLine("Error (Hasn`t ; after command word) in command " + count);
                    ErrorsCount += 1;
                    break;
                }

                Tree head = new Tree();
                head.Lexeme = mainW[0].Item1;
                head.Value = mainW[0].Item2;
                head.ParentValue = command;
                command.ChildValues.Add(head);

                var optionsMain = mhelpWId != -1 ? nextCommand.GetRange(mainWId + 2, mhelpWId - mainWId - 2) : nextCommand.GetRange(mainWId + 2, nextCommand.Count - mainWId - 2);

                var acceses = optionsMain.Where(x => x.Item1 == Lexemes.AccessWord).Select(x => x).ToList();
                if (acceses.Count<1 || acceses.Count>3)
                {
                    Console.WriteLine("Error (Too many access word) in command " + count);
                    ErrorsCount += 1;
                    break;
                }
                foreach(var acc in acceses)
                {
                    int acId = optionsMain.IndexOf(acc);
                    if (optionsMain[acId + 1].Item2 != "(")
                    {
                        Console.WriteLine("Error (Hasn`t access () sign) in command " + count);
                        ErrorsCount += 1;
                        break;
                    }
                    var close = optionsMain.GetRange(acId, optionsMain.Count-acId).IndexOf(new Tuple<Lexemes, string>(Lexemes.AccessLimiter, ")"))+ acId;

                    if (close==-1)
                    {
                        Console.WriteLine("Error (Hasn`t access () sign) in command " + count);
                        ErrorsCount += 1;
                        break;
                    }
                    var accDef = optionsMain.GetRange(acId + 2, close - acId - 2);
                    if (accDef.Count<1 
                        || accDef.Where(x => x.Item1 == Lexemes.AccessWord).Select(x => x).Count()>0
                        || accDef.Where(x => x.Item1 == Lexemes.AccessLimiter).Select(x => x).Count()>0)
                    {
                        Console.WriteLine("Error (Has command word in access) in command " + count);
                        ErrorsCount += 1;
                        break;
                    }
                    Tree accessItem = new Tree();
                    accessItem.Lexeme = acc.Item1;
                    accessItem.Value = acc.Item2;
                    accessItem.ParentValue = head;
                    head.ChildValues.Add(accessItem); 

                    var delims = accDef.Where(x => x.Item1 == Lexemes.Delimiter).Select(x => x).ToList();
                    if (delims.Count > 0)
                    {
                        int delNextId = 0;
                        foreach (var delim in delims)
                        {
                            int delId = delNextId==0?accDef.IndexOf(delim): delNextId;
                            delNextId = accDef.GetRange(delId + 1, accDef.Count - delId - 1).FindIndex(x => x.Item1 == Lexemes.Delimiter);
                            if (delNextId != -1) delNextId = delNextId + delId + 1;
                            if (delId<3)
                            {
                                Console.WriteLine("Error (hasn`t expression before &&) in command " + count);
                                ErrorsCount += 1;
                                break;
                            }
                            if ((delNextId!=-1 && (delNextId-delId)!=4) || (delNextId == -1 && accDef.Count- delId!=4))
                            {
                                Console.WriteLine("Error (hasn`t expression after &&) in command " + count);
                                ErrorsCount += 1;
                                break;
                            }
                            if((accDef[delId-3].Item1 == Lexemes.KeyTextWord || accDef[delId - 3].Item1 == Lexemes.KeyDigitWord)
                                && (accDef[delId - 2].Item1 == Lexemes.Comparator)
                                && (accDef[delId - 1].Item1 == Lexemes.KeyTextValue || accDef[delId - 1].Item1 == Lexemes.KeyDigitValue))
                            {
                                Tree defItem = new Tree();
                                defItem.Lexeme = accDef[delId - 2].Item1;
                                defItem.Value = accDef[delId - 2].Item2;
                                defItem.ParentValue = accessItem;
                                accessItem.ChildValues.Add(defItem);

                                Tree leftChild = new Tree();
                                leftChild.Lexeme = accDef[delId - 3].Item1;
                                leftChild.Value = accDef[delId - 3].Item2;
                                leftChild.ParentValue = defItem;
                                defItem.ChildValues.Add(leftChild);

                                Tree rightChild = new Tree();
                                rightChild.Lexeme = accDef[delId - 1].Item1;
                                rightChild.Value = accDef[delId - 1].Item2;
                                rightChild.ParentValue = defItem;
                                defItem.ChildValues.Add(rightChild);

                            } 
                            else
                            {
                                Console.WriteLine("Error (expression is wrong) in command " + count);
                                ErrorsCount += 1;
                                break;
                            }
                            if (delNextId == -1)
                            {
                                if ((accDef[delId + 1].Item1 == Lexemes.KeyTextWord || accDef[delId + 1].Item1 == Lexemes.KeyDigitWord)
                                && (accDef[delId + 2].Item1 == Lexemes.Comparator)
                                && (accDef[delId + 3].Item1 == Lexemes.KeyTextValue || accDef[delId + 3].Item1 == Lexemes.KeyDigitValue))
                            {
                                    Tree defItem = new Tree();
                                    defItem.Lexeme = accDef[delId + 2].Item1;
                                    defItem.Value = accDef[delId + 2].Item2;
                                    defItem.ParentValue = accessItem;
                                    accessItem.ChildValues.Add(defItem);

                                    Tree leftChild = new Tree();
                                    leftChild.Lexeme = accDef[delId + 1].Item1;
                                    leftChild.Value = accDef[delId + 1].Item2;
                                    leftChild.ParentValue = defItem;
                                    defItem.ChildValues.Add(leftChild);

                                    Tree rightChild = new Tree();
                                    rightChild.Lexeme = accDef[delId + 3].Item1;
                                    rightChild.Value = accDef[delId + 3].Item2;
                                    rightChild.ParentValue = defItem;
                                    defItem.ChildValues.Add(rightChild);

                                }
                                else
                                {
                                    Console.WriteLine("Error (expression is wrong) in command " + count);
                                    ErrorsCount += 1;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if(accDef.Count==1 && (accDef[0].Item1 == Lexemes.KeyDigitValue || accDef[0].Item1 == Lexemes.KeyTextValue))
                        {
                            Tree defItem = new Tree();
                            defItem.Lexeme = Lexemes.Comparator;
                            defItem.Value = "==";
                            defItem.ParentValue = accessItem;
                            accessItem.ChildValues.Add(defItem);

                            Tree leftChild = new Tree();
                            leftChild.Lexeme = Lexemes.KeyDigitWord;
                            leftChild.Value = "code";
                            leftChild.ParentValue = defItem;
                            defItem.ChildValues.Add(leftChild);

                            Tree rightChild = new Tree();
                            rightChild.Lexeme = accDef[0].Item1;
                            rightChild.Value = accDef[0].Item2;
                            rightChild.ParentValue = defItem;
                            defItem.ChildValues.Add(rightChild);
                        } else if(accDef.Count==3 && (accDef[0].Item1 == Lexemes.KeyTextWord || accDef[0].Item1 == Lexemes.KeyDigitWord)
                                && (accDef[1].Item1 == Lexemes.Comparator)
                                && (accDef[2].Item1 == Lexemes.KeyTextValue || accDef[2].Item1 == Lexemes.KeyDigitValue)
                            )
                        {
                            Tree defItem = new Tree();
                            defItem.Lexeme = accDef[1].Item1;
                            defItem.Value = accDef[1].Item2;
                            defItem.ParentValue = accessItem;
                            accessItem.ChildValues.Add(defItem);

                            Tree leftChild = new Tree();
                            leftChild.Lexeme = accDef[0].Item1;
                            leftChild.Value = accDef[0].Item2;
                            leftChild.ParentValue = defItem;
                            defItem.ChildValues.Add(leftChild);

                            Tree rightChild = new Tree();
                            rightChild.Lexeme = accDef[2].Item1;
                            rightChild.Value = accDef[2].Item2;
                            rightChild.ParentValue = defItem;
                            defItem.ChildValues.Add(rightChild);
                        }
                    }
                }


                if (mhelpW.Count == 1)
                {
                    Tree part = new Tree();
                    part.Lexeme = mhelpW[0].Item1;
                    part.Value = mhelpW[0].Item2;
                    part.ParentValue = command;
                    command.ChildValues.Add(part);

                    if (nextCommand[mhelpWId + 1].Item1 != Lexemes.Limiter)
                    {
                        Console.WriteLine("Error not found :");
                        ErrorsCount += 1;
                        break;
                    }
                    var optionsHelp = nextCommand.GetRange(mhelpWId + 2, nextCommand.Count - mhelpWId - 2);
                    var accesesHelp = optionsHelp.Where(x => x.Item1 == Lexemes.AccessWord).Select(x => x).ToList();
                    if (accesesHelp.Count < 1 || accesesHelp.Count > 2)
                    {
                        Console.WriteLine("Error acces");
                        ErrorsCount += 1;
                        break;
                    }
                    foreach (var acc in accesesHelp)
                    {
                        int acId = optionsHelp.IndexOf(acc);
                        if (optionsHelp[acId + 1].Item2 != "(")
                        {
                            Console.WriteLine("Error (Hasn`t access () sign) in command " + count);
                            ErrorsCount += 1;
                            break;
                        }
                        var close = optionsHelp.GetRange(acId, optionsHelp.Count - acId).IndexOf(new Tuple<Lexemes, string>(Lexemes.AccessLimiter, ")")) + acId;
                        if (close == -1)
                        {
                            Console.WriteLine("Error (Hasn`t access () sign) in command " + count);
                            ErrorsCount += 1;
                            break;
                        }
                        var accDef = optionsHelp.GetRange(acId + 2, close - acId - 2);
                        if (accDef.Count < 1
                            || accDef.Where(x => x.Item1 == Lexemes.AccessWord).Select(x => x).Count() > 0
                            || accDef.Where(x => x.Item1 == Lexemes.AccessLimiter).Select(x => x).Count() > 0)
                        {
                            Console.WriteLine("Error (Has command word in access) in command " + count);
                            ErrorsCount += 1;
                            break;
                        }
                        Tree accessItem = new Tree();
                        accessItem.Lexeme = acc.Item1;
                        accessItem.Value = acc.Item2;
                        accessItem.ParentValue = part;
                        part.ChildValues.Add(accessItem);

                        var delims = accDef.Where(x => x.Item1 == Lexemes.Delimiter).Select(x => x).ToList();

                        if (delims.Count > 0)
                        {
                            int delNextId = 0;
                            foreach (var delim in delims)
                            {
                                int delId = delNextId == 0 ? accDef.IndexOf(delim) : delNextId;
                                delNextId = accDef.GetRange(delId + 1, accDef.Count - delId - 1).FindIndex(x => x.Item1 == Lexemes.Delimiter) + delId + 1;
                                if (delId < 3)
                                {
                                    Console.WriteLine("Error (hasn`t expression before &&) in command " + count);
                                    ErrorsCount += 1;
                                    break;
                                }
                                if ((delNextId != -1 && (delNextId - delId) != 4) || (delNextId == -1 && accDef.Count - delId != 4))
                                {
                                    Console.WriteLine("Error (hasn`t expression after &&) in command " + count);
                                    ErrorsCount += 1;
                                    break;
                                }
                                if ((accDef[delId - 3].Item1 == Lexemes.KeyTextWord || accDef[delId - 3].Item1 == Lexemes.KeyDigitWord)
                                    && (accDef[delId - 2].Item1 == Lexemes.Comparator)
                                    && (accDef[delId - 1].Item1 == Lexemes.KeyTextValue || accDef[delId - 1].Item1 == Lexemes.KeyDigitValue))
                                {
                                    Tree defItem = new Tree();
                                    defItem.Lexeme = accDef[delId - 2].Item1;
                                    defItem.Value = accDef[delId - 2].Item2;
                                    defItem.ParentValue = accessItem;
                                    accessItem.ChildValues.Add(defItem);

                                    Tree leftChild = new Tree();
                                    leftChild.Lexeme = accDef[delId - 3].Item1;
                                    leftChild.Value = accDef[delId - 3].Item2;
                                    leftChild.ParentValue = defItem;
                                    defItem.ChildValues.Add(leftChild);

                                    Tree rightChild = new Tree();
                                    rightChild.Lexeme = accDef[delId - 1].Item1;
                                    rightChild.Value = accDef[delId - 1].Item2;
                                    rightChild.ParentValue = defItem;
                                    defItem.ChildValues.Add(rightChild);

                                }
                                else
                                {
                                    Console.WriteLine("Error (expression is wrong) in command " + count);
                                    ErrorsCount += 1;
                                    break;
                                }
                                if (delNextId == -1)
                                {
                                    if ((accDef[delId + 1].Item1 == Lexemes.KeyTextWord || accDef[delId + 1].Item1 == Lexemes.KeyDigitWord)
                                    && (accDef[delId + 2].Item1 == Lexemes.Comparator)
                                    && (accDef[delId + 3].Item1 == Lexemes.KeyTextValue || accDef[delId + 3].Item1 == Lexemes.KeyDigitValue))
                                    {
                                        Tree defItem = new Tree();
                                        defItem.Lexeme = accDef[delId + 2].Item1;
                                        defItem.Value = accDef[delId + 2].Item2;
                                        defItem.ParentValue = accessItem;
                                        accessItem.ChildValues.Add(defItem);

                                        Tree leftChild = new Tree();
                                        leftChild.Lexeme = accDef[delId + 1].Item1;
                                        leftChild.Value = accDef[delId + 1].Item2;
                                        leftChild.ParentValue = defItem;
                                        defItem.ChildValues.Add(leftChild);

                                        Tree rightChild = new Tree();
                                        rightChild.Lexeme = accDef[delId + 3].Item1;
                                        rightChild.Value = accDef[delId + 3].Item2;
                                        rightChild.ParentValue = defItem;
                                        defItem.ChildValues.Add(rightChild);

                                    }
                                    else
                                    {
                                        Console.WriteLine("Error (expression is wrong) in command " + count);
                                        ErrorsCount += 1;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (accDef.Count == 1 && (accDef[0].Item1 == Lexemes.KeyDigitValue || accDef[0].Item1 == Lexemes.KeyTextValue))
                            {
                                Tree defItem = new Tree();
                                defItem.Lexeme = Lexemes.Comparator;
                                defItem.Value = "==";
                                defItem.ParentValue = accessItem;
                                accessItem.ChildValues.Add(defItem);

                                Tree leftChild = new Tree();
                                leftChild.Lexeme = Lexemes.KeyDigitWord;
                                leftChild.Value = "code";
                                leftChild.ParentValue = defItem;
                                defItem.ChildValues.Add(leftChild);

                                Tree rightChild = new Tree();
                                rightChild.Lexeme = accDef[0].Item1;
                                rightChild.Value = accDef[0].Item2;
                                rightChild.ParentValue = defItem;
                                defItem.ChildValues.Add(rightChild);
                            }
                            else if (accDef.Count == 3 && (accDef[0].Item1 == Lexemes.KeyTextWord || accDef[0].Item1 == Lexemes.KeyDigitWord)
                                  && (accDef[1].Item1 == Lexemes.Comparator)
                                  && (accDef[2].Item1 == Lexemes.KeyTextValue || accDef[2].Item1 == Lexemes.KeyDigitValue)
                              )
                            {
                                Tree defItem = new Tree();
                                defItem.Lexeme = accDef[1].Item1;
                                defItem.Value = accDef[1].Item2;
                                defItem.ParentValue = accessItem;
                                accessItem.ChildValues.Add(defItem);

                                Tree leftChild = new Tree();
                                leftChild.Lexeme = accDef[0].Item1;
                                leftChild.Value = accDef[0].Item2;
                                leftChild.ParentValue = defItem;
                                defItem.ChildValues.Add(leftChild);

                                Tree rightChild = new Tree();
                                rightChild.Lexeme = accDef[2].Item1;
                                rightChild.Value = accDef[2].Item2;
                                rightChild.ParentValue = defItem;
                                defItem.ChildValues.Add(rightChild);
                            }
                        }

                    }
                }
                Commands.Add(command);
            }
        }

        public void printResult()
        {
            if (ErrorsCount < 1)
            {
                foreach (var cmd in Commands)
                {
                    Console.WriteLine("---------------------");
                    cmd.PrintPretty("", true);
                }
            } else
            {
                Console.WriteLine("---------------------");
                Console.WriteLine("Found " + ErrorsCount + " errors!!!");
            }
        }
    }
}
