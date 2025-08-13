using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetCare.Booking.Application.Common
{
    public static class ReservationStatusCodes
    {
        public const int Pending = 1;
        public const int Accepted = 2;
        public const int Canceled = 3;
        public const int Finished = 4;
    }

}
