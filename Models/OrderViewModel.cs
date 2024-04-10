using MRSTWEb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public decimal TotalSumToPay { get; set; }
        public DateTime BuyingTime { get; set; }

        //Relatia cu utilizatorul
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        //Relatia cu produsul cumparat
        public ICollection<Book> Items { get; set; }
    }
}