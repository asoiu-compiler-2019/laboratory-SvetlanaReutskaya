using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language.ProgramImitation
{
    public class Pacient
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public Region Region { get; set; }
    }

    public class Region
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class Medicine
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }

    public class Disease
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Heavy { get; set; }
    }

    public class PacientDisease
    {
        public int Code { get; set; }
        public Pacient Pacient { get; set; }
        public Disease Disease { get; set; }
    }
    public class PacientMedicine
    {
        public int Code { get; set; }
        public PacientDisease PacientDisease { get; set; }
        public Medicine Medicine { get; set; }
    }
    public class DataBase
    {
        public List<Pacient> Pacients = new List<Pacient>();
        public List<Region> Regions = new List<Region>();
        public List<Disease> Diseases = new List<Disease>();
        public List<Medicine> Medicines = new List<Medicine>();
        public List<PacientDisease> PacientDiseases = new List<PacientDisease>();
        public List<PacientMedicine> PacientMedicines = new List<PacientMedicine>();
    }

}
