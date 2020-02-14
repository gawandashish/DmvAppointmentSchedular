using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DmvAppointmentScheduler
{
    class Program
    {
        public static Random random = new Random();
        public static List<Appointment> appointmentList = new List<Appointment>();
        static void Main(string[] args)
        {
            try
            {
                CustomerList customers = ReadCustomerData();
                TellerList tellers = ReadTellerData();
                Calculation(customers, tellers);
                OutputTotalLengthToConsole();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Appointment Schedular -" + ex.Message);
                Console.Read();
            }

        }
        private static CustomerList ReadCustomerData()
        {
            try
            {
                string fileName = "CustomerData.json";
                string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
                string jsonString = File.ReadAllText(path);
                CustomerList customerData = JsonConvert.DeserializeObject<CustomerList>(jsonString);
                return customerData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ReadCustomerData -" + ex.Message);
            }
            return null;

        }
        private static TellerList ReadTellerData()
        {
            try
            {
                string fileName = "TellerData.json";
                string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
                string jsonString = File.ReadAllText(path);
                TellerList tellerData = JsonConvert.DeserializeObject<TellerList>(jsonString);
                return tellerData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ReadTellerData -" + ex.Message);
            }
            return null;

        }
        async static void Calculation(CustomerList customers, TellerList tellers)
        {
            try
            {

                //  foreach (Customer customer in customers.Customer)

                int i = 0;
                while (i <= customers.Customer.Count - 1)
                {
                    Parallel.ForEach(tellers.Teller, (teller, state) =>
                    {
                        if (teller.isFree)
                        {
                            teller.isFree = false;
                            Task<double> task= ProcessTeller(teller, customers.Customer[i]);
                            var appointment = new Appointment(customers.Customer[i], teller, task.Result);
                            appointmentList.Add(appointment);                            
                            i = i + 1;
                            state.Break();
                        }
                    }
                    );
                    //foreach (Teller teller in tellers.Teller)
                    //{
                    //    if (teller.isFree)
                    //    {
                    //        teller.isFree = false;
                    //        var appointment = new Appointment(customers.Customer[i], teller);
                    //        appointmentList.Add(appointment);
                    //        ProcessTeller(teller, customers.Customer[i]);
                    //        i = i + 1;
                    //        break;
                    //    }
                    //}

                    //var appointment = new Appointment(customer, tellers.Teller[0]);
                    //appointmentList.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Calculation -" + ex.Message);
            }
        }
        async static Task<double> ProcessTeller(Teller teller, Customer customer)
        {
            double duration = 0;
            try
            {
                
                if (customer.type == teller.specialtyType)
                {
                    duration = Math.Ceiling(Convert.ToDouble(customer.duration) * Convert.ToDouble(teller.multiplier));
                }
                else
                {
                    duration = Convert.ToDouble(customer.duration);
                }
                await Task.Run(() => { System.Threading.Thread.Sleep(Convert.ToInt32(duration) * 1000); });
                teller.isFree = true;
                return duration;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ProcessTeller -" + ex.Message);
            }
            return duration;
        }
        static void OutputTotalLengthToConsole()
        {
            try
            {
                var tellerAppointments =
                    from appointment in appointmentList
                    group appointment by appointment.teller into tellerGroup
                    select new
                    {
                        teller = tellerGroup.Key,
                        totalDuration = tellerGroup.Sum(x => x.duration),
                    };
                var max = tellerAppointments.OrderBy(i => i.totalDuration).LastOrDefault();
                Console.WriteLine("Teller " + max.teller.id + " will work for " + max.totalDuration + " minutes!");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in OutputTotalLengthToConsole -" + ex.Message);
            }
        }

    }
}
