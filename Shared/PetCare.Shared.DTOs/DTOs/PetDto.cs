using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Shared.DTOs
{
    public class PetDto
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime BirthDate { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }

    }
}
