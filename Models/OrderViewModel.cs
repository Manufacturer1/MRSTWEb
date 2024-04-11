using MRSTWEb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "The First Name is Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "The Last Name is Required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The Address filed is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "The City is Required")]
        public string City { get; set; }
        public string PostCode { get; set; }
        [Required(ErrorMessage = "The Phone Number field is required.")]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(8, ErrorMessage = "The Phone Number exceeds the required length")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "The Email is Required")]
        public string Email { get; set; }
        public decimal TotalSumToPay { get; set; }
        public DateTime BuyingTime { get; set; }

        //Relatia cu utilizatorul
        public string ApplicationUserId { get; set; }

        //Relatia cu produsul cumparat
        public ICollection<OrderItem> Items { get; set; }

        public OrderViewModel()
        {
            Items = new List<OrderItem>();
        }
    }
}