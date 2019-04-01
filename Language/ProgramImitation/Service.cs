using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language.ProgramImitation
{
    public class Service
    {
        DataBase db = new DataBase();

        public List<PacientDisease> GetPacientDiseases()
        {
            return db.PacientDiseases.ToList();
        }
        public List<PacientMedicine> GetPacientMedicines()
        {
            return db.PacientMedicines.ToList();
        }

        public string CreatePacient(Pacient a)
        {
            if (db.Pacients.Where(x => x.Code == a.Code).Count() == 0)
            {
                db.Pacients.Add(a);
                return "";
            }
            else return "Can't Add";
        }
        public string DeletePacient(Pacient a)
        {
            if (db.Pacients.Where(x => x.Code == a.Code).Count() != 0)
            {
                db.Pacients.Remove(db.Pacients.Where(x => x.Code == a.Code).FirstOrDefault());
                return "";
            }
            else return "Can't Delete";
        }
        public List<Pacient> GetPacients()
        {
            return db.Pacients;
        }

        public string CreateDisease(Disease a)
        {
            if (db.Diseases.Where(x => x.Code == a.Code).Count() == 0)
            {
                db.Diseases.Add(a);
                return "";
            }
            else return "Can't Add";
        }
        public string DeleteDisease(Disease a)
        {
            if (db.Diseases.Where(x => x.Code == a.Code).Count() != 0)
            {
                db.Diseases.Remove(db.Diseases.Where(x => x.Code == a.Code).FirstOrDefault());
                return "";
            }
            else return "Can't Delete";
        }
        public List<Disease> GetDiseases()
        {
            return db.Diseases;
        }


        public string CreateMedicine(Medicine a)
        {
            if (db.Medicines.Where(x => x.Code == a.Code).Count() == 0)
            {
                db.Medicines.Add(a);
                return "";
            }
            else return "Can't Add";
        }
        public string DeleteMedicine(Medicine a)
        {
            if (db.Medicines.Where(x => x.Code == a.Code).Count() != 0)
            {
                db.Medicines.Remove(db.Medicines.Where(x => x.Code == a.Code).FirstOrDefault());
                return "";
            }
            else return "Can't Delete";
        }
        public List<Medicine> GetMedicines()
        {
            return db.Medicines;
        }

        public string CreateRegion(Region a)
        {
            if (db.Regions.Where(x => x.Code == a.Code).Count() == 0)
            {
                db.Regions.Add(a);
                return "";
            }
            else return "Can't Add";
        }
        public string DeleteRegion(Region a)
        {
            if (db.Regions.Where(x => x.Code == a.Code).Count() != 0)
            {
                db.Regions.Remove(db.Regions.Where(x => x.Code == a.Code).FirstOrDefault());
                return "";
            }
            else return "Can't Delete";
        }
        public List<Region> GetRegions()
        {
            return db.Regions;
        }

        public string SetRegion(Pacient a, Region r)
        {
            if (db.Pacients.Where(x => x.Code == a.Code).Count() != 0 && db.Regions.Where(x => x.Code == r.Code).Count() != 0)
            {
                db.Pacients.Where(x => x.Code == a.Code).First().Region = db.Regions.Where(x => x.Code == r.Code).First();
                return "";
            }
            else return "Can't SetRegion";
        }
        public string RemoveRegion(Pacient a, Region r)
        {
            if (db.Pacients.Where(x => x.Code == a.Code).Count() != 0 && db.Regions.Where(x => x.Code == r.Code).Count() != 0)
            {
                db.Pacients.Where(x => x.Code == a.Code).First().Region = null;
                return "";
            }
            else return "Can't SetRegion";
        }

        public string SetDisease(Pacient a, Disease d)
        {
            if (db.Pacients.Where(x => x.Code == a.Code).Count() != 0 && db.Diseases.Where(x => x.Code == d.Code).Count() != 0)
            {
                PacientDisease pd = new PacientDisease() { Code = db.PacientDiseases.Count()+1, Pacient = db.Pacients.Where(x => x.Code == a.Code).First(), Disease = db.Diseases.Where(x => x.Code == d.Code).First() };
                db.PacientDiseases.Add(pd);
                return "";
            }
            else return "Can't SetDisease";
        }
        public string RemoveDisease(Pacient a, Disease d)
        {
            if (db.Pacients.Where(x => x.Code == a.Code).Count() != 0 && db.Diseases.Where(x => x.Code == d.Code).Count() != 0)
            {
                db.PacientDiseases.Remove(db.PacientDiseases.Where(x => (x.Pacient.Code == a.Code && x.Disease.Code==d.Code)).First());
                return "";
            }
            else return "Can't RemoveDisease";
        }

        public string SetMedicine(PacientDisease pd, Medicine m)
        {
            if (db.PacientDiseases.Where(x => x.Code == pd.Code).Count() != 0 && db.Medicines.Where(x => x.Code == m.Code).Count() != 0)
            {
                PacientMedicine pm = new PacientMedicine() { Code = db.PacientMedicines.Count() + 1, PacientDisease = db.PacientDiseases.Where(x => x.Code == pd.Code).First(), Medicine = db.Medicines.Where(x => x.Code == m.Code).First() };
                db.PacientMedicines.Add(pm);
                return "";
            }
            else return "Can't SetMedicine";
        }
        public string RemoveMedicine(PacientDisease pd, Medicine m)
        {
            if (db.PacientDiseases.Where(x => (x.Pacient == pd.Pacient && x.Disease==pd.Disease)).Count() != 0 && db.Medicines.Where(x => x.Code == m.Code).Count() != 0)
            {
                db.PacientMedicines.Remove(db.PacientMedicines.Where(x => (x.PacientDisease.Code == pd.Code && x.Medicine.Code == m.Code)).First());
                return "";
            }
            else return "Can't RemoveMedicine";
        }
        
    }
}
