using System;
using System.Collections.Generic;
using System.Text;

namespace DmvAppointmentScheduler
{
    public class Appointment
    {
        public Customer customer { get; set; }
        public Teller teller { get; set; }
        public double duration { get; set; }
        public Appointment(Customer customer, Teller teller, double duration)
        {
            this.customer = customer;
            this.teller = teller;
            this.duration = duration;
            //if (customer.type == teller.specialtyType)
            //{
            //    this.duration = Math.Ceiling(Convert.ToDouble(customer.duration) * Convert.ToDouble(teller.multiplier));
            //}
            //else
            //{
            //    this.duration = Convert.ToDouble(customer.duration);
            //}
        }
    }
}
