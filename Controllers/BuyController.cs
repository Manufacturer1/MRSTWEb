using AutoMapper;
using Microsoft.AspNet.Identity;
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
using System.Threading.Tasks;
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

        [Authorize]
        [SessionTimeout]
        public ActionResult SuccessfullOrder()
        {
            string userId = User.Identity.GetUserId();
            var orderDto = orderService.GetOrdersByUserId(userId).LastOrDefault();
            var itemsDto = orderDto.Items.ToList();
            var bookModel = new List<BookViewModel>();  

            foreach(var item in itemsDto)
            {
                var bookDto = cartService.GetPBook(item.BookId);
                var book = new BookViewModel
                {
                    Id = item.BookId,
                    Title = bookDto.Title,
                    Price = bookDto.Price,
                    Author = bookDto.Author,    
                   PathImage = bookDto.PathImage,
                   Quantity = item.Quantity,
                   
                };
                bookModel.Add(book);    
            }
            if(orderDto == null) return HttpNotFound();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO,OrderViewModel>()).CreateMapper();
            var order = mapper.Map<OrderDTO, OrderViewModel>(orderDto);
            ViewBag.CartItems = bookModel;
            Session.Clear();

            return View(order);

        }


        [HttpGet]
        [Authorize]
        public ActionResult Checkout()
        {
           var total = cartService.CalculateTotalPrice();
            var subtotal = total;
            ViewBag.Total = total; ViewBag.Subtotal = subtotal;

            return View();
        }
        [HttpPost]
        [Authorize]
        [SessionTimeout]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Checkout(OrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserService.GetUserById(User.Identity.GetUserId());
               

                decimal totalSumToPay = cartService.CalculateTotalPrice();

                if(totalSumToPay > 0)
                {
                    var orderDto = new OrderDTO
                    {
                        ApplicationUserId = user.Id,
                        Address = model.Address,
                        City = model.City,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Phone = model.Phone,
                        PostCode = model.PostCode,
                        Email = model.Email,
                        TotalSumToPay = totalSumToPay,
                       
                    };
                    orderService.MakeOrder(orderDto);
                    return RedirectToAction("SuccessfullOrder","Buy");
                }
             
            }
            return View(model);
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