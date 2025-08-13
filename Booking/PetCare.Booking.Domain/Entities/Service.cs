using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Booking.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }

        public Guid CaregiverId { get; set; }

        // Navegación inversa
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }


}
