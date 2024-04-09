using AutoMapper;
using MRSTWEb.BuisnessLogic.BuisnessModels;
using MRSTWEb.BuisnessLogic.DTO;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BuisnessLogic.Services;
using MRSTWEb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRSTWEb.Controllers
{
    public class BuyController : Controller
    {
        private ICartService cartService;
        public BuyController(ICartService _cartService)
        {
            cartService = _cartService;
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