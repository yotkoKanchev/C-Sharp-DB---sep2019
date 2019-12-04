namespace PetClinic.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Procedure
    {
        public int Id { get; set; }

        public int AnimalId { get; set; }
        public Animal Animal { get; set; }

        public int VetId { get; set; }
        public Vet Vet { get; set; }

        public ICollection<ProcedureAnimalAid> ProcedureAnimalAids { get; set; } = new HashSet<ProcedureAnimalAid>();

        [NotMapped] //getter only
        public decimal Cost => this.ProcedureAnimalAids.Sum(paa => paa.Procedure.Cost);

        public DateTime DateTime { get; set; }
    }
}
