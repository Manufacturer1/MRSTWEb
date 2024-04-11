using AutoMapper;
using Microsoft.AspNet.Identity.Owin;
using MRSTWEb.BuisnessLogic.BuisnessModels;
using MRSTWEb.BuisnessLogic.DTO;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BuisnessLogic.Services;
using MRSTWEb.Filters;
using MRSTWEb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRSTWEb.Controllers
{
    [SessionTimeout]
    public class BuyController : Controller
    {
        private ICartService cartService;
        private IOrderService orderService;
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }
        public BuyController(ICartService _cartService, IOrderService orderService)
        {
            cartService = _cartService;
            this.orderService = orderService;
        }

        [HttpGet]
        public ActionResult Checkout()
        {
           var total = cartService.CalculateTotalPrice();
            var subtotal = total;
            ViewBag.Total = total; ViewBag.Subtotal = subtotal;

            return View();
        }

        [HttpGet]
        public ActionResult Cart() {

            var booksDTO = cartService.GetBooks();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<BookDTO, BookViewModel>()).CreateMapper();
            var books = mapper.Map<IEnumerable<BookDTO>, List<BookViewModel>>(booksDTO);
            ViewBag.Books = books; 
  
            var cart = cartService.GetCart();

            var book = new Item
            {
                Book = new BookDTO(),
                Quantity = 0,
            };
            ViewBag.Cart = cart;    

            return View(book);
        }
        [HttpPost]
        public ActionResult AddToCart(int BookId)
        {
            cartService.AddToCart(BookId);
            var cart = cartService.GetCart();   
            return PartialView("_addToCartForm",cart);
        }
        [HttpPost]
        public ActionResult RemoveFromTheCart(int BookId)
        {
            cartService.RemoveFromTheCart(BookId);
            var cart = cartService.GetCart();

            return PartialView("_addToCartForm",cart);
        }

        
    
        protected override void Dispose(bool disposing)
        {
            cartService.Dispose();
            base.Dispose(disposing);
        }
    }
}