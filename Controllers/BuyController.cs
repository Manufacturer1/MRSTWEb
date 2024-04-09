using AutoMapper;
using MRSTWEb.BuisnessLogic.DTO;
using MRSTWEb.BuisnessLogic.Interfaces;
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

            return View();
        }

    }
}