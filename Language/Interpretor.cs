
using Language.ProgramImitation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Language
{
    public class Interpretor
    {
        Service Program = new Service();

        LexicalParser LexParser = new LexicalParser();
        SyntaxParser SynParser = new SyntaxParser();
        SemanticParser SemParser = new SemanticParser();

        public void Run(string path)
        {
            LexParser.Parse(path);
            
            SynParser.LexemesList = LexParser.LexemesList;
            SynParser.Parse();

            if (SynParser.ErrorsCount == 0)
            {
                SemParser.Commands = SynParser.Commands;
                SemParser.Analise();

                if (SemParser.Errors == 0)
                {
                    CreateCode(SemParser.Commands);
                }
            }
        }

        void CreateCode(List<Tree> commands)
        {
            foreach(var c in commands)
            {
                if (c.ChildValues[0].Value == "create") CreateFunction(c);
                if (c.ChildValues[0].Value == "delete") DeleteFunction(c);

                if (c.ChildValues[0].Value == "bind") BindFunction(c);
                if (c.ChildValues[0].Value == "unbind") UnbindFunction(c);

                if (c.ChildValues[0].Value == "getall") CheckFunction(c);
            } 
        }

        void CreateFunction(Tree c)
        {
            string result="Nothing happened";
            if (c.ChildValues[0].ChildValues[0].Value == "pacient")
            {
                Pacient a = new Pacient();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                foreach(var p in pars)
                {
                    if (p.ChildValues[0].Value == "code") a.Code = Convert.ToInt32(p.ChildValues[1].Value);
                    if (p.ChildValues[0].Value == "name") a.Name = p.ChildValues[1].Value;
                    if (p.ChildValues[0].Value == "age") a.Age = Convert.ToInt32(p.ChildValues[1].Value);
                }

                result = Program.CreatePacient(a);
            }
            if (c.ChildValues[0].ChildValues[0].Value == "medicine")
            {
                Medicine a = new Medicine();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                foreach (var p in pars)
                {
                    if (p.ChildValues[0].Value == "code") a.Code = Convert.ToInt32(p.ChildValues[1].Value);
                    if (p.ChildValues[0].Value == "name") a.Name = p.ChildValues[1].Value;
                }
                result = Program.CreateMedicine(a);
            }
            if (c.ChildValues[0].ChildValues[0].Value == "disease")
            {
                Disease a = new Disease();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                foreach (var p in pars)
                {
                    if (p.ChildValues[0].Value == "code") a.Code = Convert.ToInt32(p.ChildValues[1].Value);
                    if (p.ChildValues[0].Value == "name") a.Name = p.ChildValues[1].Value;
                    if (p.ChildValues[0].Value == "heavy") a.Heavy = p.ChildValues[1].Value;
                }
                result = Program.CreateDisease(a);
            }
            if (c.ChildValues[0].ChildValues[0].Value == "region")
            {
                Region a = new Region();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                foreach (var p in pars)
                {
                    if (p.ChildValues[0].Value == "code") a.Code = Convert.ToInt32(p.ChildValues[1].Value);
                    if (p.ChildValues[0].Value == "name") a.Name = p.ChildValues[1].Value;
                }
                result = Program.CreateRegion(a);
            }

            if (result != "") Console.WriteLine(result);
            else Console.WriteLine("Added sucsessfully");
        }
        void DeleteFunction(Tree c)
        {
            string result = "Nothing happened";
            if (c.ChildValues[0].ChildValues[0].Value == "pacient")
            {
                Pacient a = new Pacient();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                if (pars.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(pars.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                    result = Program.DeletePacient(a);
                } else
                {
                    var pacients = Program.GetPacients();
                    foreach (var p in pars)
                    {
                        if (p.ChildValues[0].Value == "name") pacients = pacients.Where(x => x.Name == p.ChildValues[1].Value).ToList();
                        if (p.ChildValues[0].Value == "age") pacients = pacients.Where(x => x.Age == Convert.ToInt32(p.ChildValues[1].Value)).ToList();
                    }
                    foreach(var p in pacients)
                    {
                        result = Program.DeletePacient(p);
                    }
                }
            }
            if (c.ChildValues[0].ChildValues[0].Value == "medicine")
            {
                Medicine a = new Medicine();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                if (pars.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(pars.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                    result = Program.DeleteMedicine(a);
                }
                else
                {
                    var medicines = Program.GetMedicines();
                    medicines = medicines.Where(x => x.Name == pars[0].ChildValues[1].Value).ToList();
                    foreach (var p in medicines)
                    {
                        result = Program.DeleteMedicine(p);
                    }
                }
            }
            if (c.ChildValues[0].ChildValues[0].Value == "disease")
            {
                Disease a = new Disease();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                if (pars.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(pars.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                    result = Program.DeleteDisease(a);
                }
                else
                {
                    var diseases = Program.GetDiseases();
                    foreach (var p in pars)
                    {
                        if (p.ChildValues[0].Value == "name") diseases = diseases.Where(x => x.Name == p.ChildValues[1].Value).ToList();
                        if (p.ChildValues[0].Value == "heavy") diseases = diseases.Where(x => x.Heavy == p.ChildValues[1].Value).ToList();
                    }
                    foreach (var p in diseases)
                    {
                        result = Program.DeleteDisease(p);
                    }
                }
            }
            if (c.ChildValues[0].ChildValues[0].Value == "region")
            {
                Region a = new Region();
                var pars = c.ChildValues[0].ChildValues[0].ChildValues;
                if (pars.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(pars.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                    result = Program.DeleteRegion(a);
                }
                else
                {
                    var regions = Program.GetRegions();
                    regions = regions.Where(x => x.Name == pars[0].ChildValues[1].Value).ToList();
                    foreach (var p in regions)
                    {
                        result = Program.DeleteRegion(p);
                    }
                }
            }

            if (result != "") Console.WriteLine(result);
            else Console.WriteLine("deleted sucsessfully");
        }
        void BindFunction(Tree c)
        {
            string result = "Nothing happened";
            if (c.ChildValues[0].ChildValues[0].Value == "region")
            {
                Region r = new Region();
                var parsR = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsR.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    r.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                Pacient a = new Pacient();
                var parsP = c.ChildValues[1].ChildValues[0].ChildValues;
                if (parsP.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(parsP.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);

                    result = Program.SetRegion(a, r);
                }
                else
                {
                    var pacients = Program.GetPacients();
                    foreach (var p in parsP)
                    {
                        if (p.ChildValues[0].Value == "name")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                        }
                        if (p.ChildValues[0].Value == "age")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Age != 0 && x.Age == Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Age == 0 || (x.Age != 0 && x.Age != Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Age != 0 && Math.Abs(x.Age - Convert.ToInt32(p.ChildValues[1].Value)) < 3)).ToList();
                        }
                    }
                    foreach (var p in pacients)
                    {
                        result = Program.SetRegion(p, r);
                    }
                }
            }
            if (c.ChildValues[0].ChildValues[0].Value == "disease")
            {
                Disease r = new Disease();
                var parsR = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsR.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    r.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                Pacient a = new Pacient();
                var parsP = c.ChildValues[1].ChildValues[0].ChildValues;
                if (parsP.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(parsP.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);

                    result = Program.SetDisease(a, r);
                }
                else
                {
                    var pacients = Program.GetPacients();
                    foreach (var p in parsP)
                    {
                        if (p.ChildValues[0].Value == "name")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                        }
                        if (p.ChildValues[0].Value == "age")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Age != 0 && x.Age == Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Age == 0 || (x.Age != 0 && x.Age != Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Age != 0 && Math.Abs(x.Age - Convert.ToInt32(p.ChildValues[1].Value)) < 3)).ToList();
                        }
                    }
                    foreach (var p in pacients)
                    {
                        result = Program.SetDisease(p, r);
                    }
                }
            }
            if (c.ChildValues[0].ChildValues[0].Value == "medicine")
            {
                Medicine r = new Medicine();
                var parsR = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsR.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    r.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }

                Pacient a = new Pacient();
                List<Pacient> pacients = null;

                var parsP = c.ChildValues[1].ChildValues[0].ChildValues;
                if (parsP.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(parsP.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                else
                {
                    pacients = Program.GetPacients();
                    foreach (var p in parsP)
                    {
                        if (p.ChildValues[0].Value == "name")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                        }
                        if (p.ChildValues[0].Value == "age")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Age != 0 && x.Age == Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Age == 0 || (x.Age != 0 && x.Age != Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Age != 0 && Math.Abs(x.Age - Convert.ToInt32(p.ChildValues[1].Value)) < 3)).ToList();
                        }
                    }
                }

                Disease d = new Disease();
                var parsM = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsM.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    d.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                if (pacients == null) {
                    pacients = new List<Pacient>();
                    pacients.Add(a);
                }
                var bindes = Program.GetPacientDiseases();

                foreach(var p in pacients)
                {
                    var pd = bindes.Where(x => x.Pacient.Code == p.Code && x.Disease.Code == d.Code).FirstOrDefault();
                    if (pd != null) result = Program.SetMedicine(pd, r);
                }
            }
            if (result != "") Console.WriteLine(result);
            else Console.WriteLine("binded sucsessfully");
        }
        void UnbindFunction(Tree c)
        {
            string result = "Nothing happened";
            if (c.ChildValues[0].ChildValues[0].Value == "region")
            {
                Region r = new Region();
                var parsR = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsR.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    r.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                Pacient a = new Pacient();
                var parsP = c.ChildValues[1].ChildValues[0].ChildValues;
                if (parsP.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(parsP.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);

                    result = Program.RemoveRegion(a, r);
                }
                else
                {
                    var pacients = Program.GetPacients();
                    foreach (var p in parsP)
                    {
                        if (p.ChildValues[0].Value == "name")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                        }
                        if (p.ChildValues[0].Value == "age")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Age != 0 && x.Age == Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Age == 0 || (x.Age != 0 && x.Age != Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Age != 0 && Math.Abs(x.Age - Convert.ToInt32(p.ChildValues[1].Value)) < 3)).ToList();
                        }
                    }
                    foreach (var p in pacients)
                    {
                        result = Program.RemoveRegion(p, r);
                    }
                }
            }
            if (c.ChildValues[0].ChildValues[0].Value == "disease")
            {
                Disease r = new Disease();
                var parsR = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsR.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    r.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                Pacient a = new Pacient();
                var parsP = c.ChildValues[1].ChildValues[0].ChildValues;
                if (parsP.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(parsP.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);

                    result = Program.RemoveDisease(a, r);
                }
                else
                {
                    var pacients = Program.GetPacients();
                    foreach (var p in parsP)
                    {
                        if (p.ChildValues[0].Value == "name")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                        }
                        if (p.ChildValues[0].Value == "age")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Age != 0 && x.Age == Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Age == 0 || (x.Age != 0 && x.Age != Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Age != 0 && Math.Abs(x.Age - Convert.ToInt32(p.ChildValues[1].Value)) < 3)).ToList();
                        }
                    }
                    foreach (var p in pacients)
                    {
                        result = Program.RemoveDisease(p, r);
                    }
                }
            }
            if (c.ChildValues[0].ChildValues[0].Value == "medicine")
            {
                Medicine r = new Medicine();
                var parsR = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsR.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    r.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }

                Pacient a = new Pacient();
                List<Pacient> pacients = null;

                var parsP = c.ChildValues[1].ChildValues[0].ChildValues;
                if (parsP.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(parsP.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                else
                {
                    pacients = Program.GetPacients();
                    foreach (var p in parsP)
                    {
                        if (p.ChildValues[0].Value == "name")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                        }
                        if (p.ChildValues[0].Value == "age")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Age != 0 && x.Age == Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Age == 0 || (x.Age != 0 && x.Age != Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Age != 0 && Math.Abs(x.Age - Convert.ToInt32(p.ChildValues[1].Value)) < 3)).ToList();
                        }
                    }
                }

                Disease d = new Disease();
                var parsM = c.ChildValues[0].ChildValues[0].ChildValues;
                if (parsM.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    d.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                if (pacients == null)
                {
                    pacients = new List<Pacient>();
                    pacients.Add(a);
                }
                var bindes = Program.GetPacientDiseases();

                foreach (var p in pacients)
                {
                    var pd = bindes.Where(x => x.Pacient.Code == p.Code && x.Disease.Code == d.Code).FirstOrDefault();
                    if (pd != null) result = Program.RemoveMedicine(pd, r);
                }
            }
            if (result != "") Console.WriteLine(result);
            else Console.WriteLine("unbinded sucsessfully");
        }

        void CheckFunction(Tree c)
        {
            string result = "false";


            Pacient a = new Pacient();
            List<Pacient> pacients = null;

            Region r = new Region();
            List<Region> regions = null;

            Disease d = new Disease();
            List<Disease> deseases = null;

            Medicine m = new Medicine();
            List<Medicine> medicines = null;

            if (c.ChildValues[1].ChildValues[0].Value == "pacient")
            {
                var parsP = c.ChildValues[1].ChildValues[0].ChildValues;
                if (parsP.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                {
                    a.Code = Convert.ToInt32(parsP.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                }
                else
                {
                    pacients = Program.GetPacients();
                    foreach (var p in parsP)
                    {
                        if (p.ChildValues[0].Value == "name")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                        }
                        if (p.ChildValues[0].Value == "age")
                        {
                            if (p.Value == "==") pacients = pacients.Where(x => (x.Age != 0 && x.Age == Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "!=") pacients = pacients.Where(x => x.Age == 0 || (x.Age != 0 && x.Age != Convert.ToInt32(p.ChildValues[1].Value))).ToList();
                            if (p.Value == "~~") pacients = pacients.Where(x => (x.Age != 0 && Math.Abs(x.Age - Convert.ToInt32(p.ChildValues[1].Value)) < 3)).ToList();
                        }
                    }
                }
                if (pacients == null)
                {
                    pacients = new List<Pacient>();
                    pacients.Add(a);
                }
            }
            else pacients = Program.GetPacients();

            var parse = c.ChildValues[0].ChildValues;
            foreach(var param in parse)
            {
                if (param.Value == "region")
                {
                    var parsR = param.ChildValues;

                    if (parsR.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                    {
                        r.Code = Convert.ToInt32(parsR.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                    }
                    else
                    {
                        regions = Program.GetRegions();
                        foreach (var p in parsR)
                        {
                            if (p.ChildValues[0].Value == "name")
                            {
                                if (p.Value == "==") regions = regions.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                                if (p.Value == "!=") regions = regions.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                                if (p.Value == "~~") regions = regions.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                            }
                        }
                    }
                    if (regions == null)
                    {
                        regions = new List<Region>();
                        regions.Add(Program.GetRegions().Where(x=>x.Code==r.Code).FirstOrDefault());
                    }

                }
                if (param.Value == "disease")
                {
                    var parsD = param.ChildValues;

                    if (parsD.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                    {
                        d.Code = Convert.ToInt32(parsD.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                    }
                    else
                    {
                        deseases = Program.GetDiseases();
                        foreach (var p in parsD)
                        {
                            if (p.ChildValues[0].Value == "name")
                            {
                                if (p.Value == "==") deseases = deseases.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                                if (p.Value == "!=") deseases = deseases.Where(x => x.Name == null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                                if (p.Value == "~~") deseases = deseases.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                            }
                            if (p.ChildValues[0].Value == "heavy")
                            {
                                if (p.Value == "==") deseases = deseases.Where(x => (x.Heavy != null && x.Heavy == p.ChildValues[1].Value)).ToList();
                                if (p.Value == "!=") deseases = deseases.Where(x => x.Heavy == null || (x.Heavy != null && x.Heavy != p.ChildValues[1].Value)).ToList();
                                if (p.Value == "~~") deseases = deseases.Where(x => (x.Heavy != null && x.Heavy.Contains(p.ChildValues[1].Value))).ToList();

                            }
                        }
                    }
                    if (deseases == null)
                    {
                        deseases = new List<Disease>();
                        deseases.Add(Program.GetDiseases().Where(x => x.Code == d.Code).FirstOrDefault());
                    }
                }
                if (param.Value == "medicine")
                {
                    var parsM = param.ChildValues;

                    if (parsM.Where(x => x.ChildValues[0].Value == "code").Count() == 1)
                    {
                        m.Code = Convert.ToInt32(parsM.Where(x => x.ChildValues[0].Value == "code").FirstOrDefault().ChildValues[1].Value);
                    }
                    else
                    {
                        medicines = Program.GetMedicines();
                        foreach (var p in parsM)
                        {
                            if (p.ChildValues[0].Value == "name")
                            {
                                if (p.Value == "==") medicines = medicines.Where(x => (x.Name != null && x.Name == p.ChildValues[1].Value)).ToList();
                                if (p.Value == "!=") medicines = medicines.Where(x => x.Name==null || (x.Name != null && x.Name != p.ChildValues[1].Value)).ToList();
                                if (p.Value == "~~") medicines = medicines.Where(x => (x.Name != null && x.Name.Contains(p.ChildValues[1].Value))).ToList();

                            }
                        }
                    }
                    if (medicines == null)
                    {
                        medicines = new List<Medicine>();
                        medicines.Add(Program.GetMedicines().Where(x => x.Code == m.Code).FirstOrDefault());
                    }
                }
            }

            if (regions!=null && regions.Count != 0) pacients = pacients.Where(x => regions.Contains(x.Region)).ToList();
            if (deseases != null && deseases.Count!=0){
                var pd = Program.GetPacientDiseases().Where(x => deseases.Contains(x.Disease)).Select(x=>x.Pacient).ToList();
                pacients = pacients.Where(x => pd.Contains(x)).ToList();
            }
            if (medicines != null && medicines.Count != 0)
            {
                var pm = Program.GetPacientMedicines().Where(x => medicines.Contains(x.Medicine)).Select(x => x.PacientDisease.Pacient).ToList();
                pacients = pacients.Where(x => pm.Contains(x)).ToList();
            }

            if (c.ChildValues[1].ChildValues.Count == 2)
            {
                if (Convert.ToInt32(c.ChildValues[1].ChildValues[1].ChildValues[0].ChildValues[1].Value) == pacients.Count) result = "true";
            }
            else if (c.ChildValues[1].ChildValues[0].Value == "count")
            {
                if (Convert.ToInt32(c.ChildValues[1].ChildValues[0].ChildValues[0].ChildValues[1].Value) == pacients.Count) result = "true";
            }
            else if(pacients.Count>0) result = "true";

            Console.WriteLine(result);
        }
    }
}