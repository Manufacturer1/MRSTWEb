using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MRSTWEb.BuisnessLogic.DTO;
using MRSTWEb.BuisnessLogic.Infrastructure;
using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BuisnessLogic.Services;
using MRSTWEb.Filters;
using MRSTWEb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace MRSTWEb.Controllers
{
    public class AccountController : Controller
    {
        private ICartService cartService;
        private IUserService userService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>(); 
            }
        }
        private IAuthenticationManager authenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public AccountController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        [Authorize(Roles ="user")]
        [SessionTimeout]
        public async Task<ActionResult> ClientProfile()
        {
            var userDto = await userService.GetUserById(User.Identity.GetUserId());
            var user = new UserModel { Id = userDto.Id,Email=userDto.Email,Address=userDto.Address,Name=userDto.Name,UserName=userDto.UserName };

            if(user == null) return HttpNotFound();

            return View(user);
        }


        [HttpGet]
        [Authorize(Roles ="user")]
        [SessionTimeout]
     /*   public async Task<ActionResult> EditClientProfile()
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            var editModel = 
        }*/
  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                UserDTO userDTO = new UserDTO { UserName = model.UserName, Password = model.Password };
                ClaimsIdentity claim = await userService.Authenticate(userDTO);
                if(claim == null)
                {
                    ModelState.AddModelError("", "Incorect login or password.");
                }
                else
                {
                    var userRole = claim.FindFirst(ClaimTypes.Role)?.Value;
                    if (userRole == "admin")
                    {
                        authenticationManager.SignOut();
                        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        authenticationManager.SignOut();
                        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
                        return RedirectToAction("ClientProfile","Account");
                    }
                }
            }
            return View(model);
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            authenticationManager.SignOut();
            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            /*await SetInitialData();*/
            if(ModelState.IsValid)
            {
                UserDTO userDTO = new UserDTO
                {
                    Email = model.Email,
                    Name = model.Name,
                    Address = model.Address,
                    UserName = model.UserName,
                    Password = model.Password,
                    Role = "user",
                   
                };
                OperationDetails operationDetalis = await userService.Create(userDTO);
                if (operationDetalis.Succeeded) return RedirectToAction("Index");
                else ModelState.AddModelError(operationDetalis.Property, operationDetalis.Message);
            }
            return View(model);
        }


        #region Helpers
        private async Task SetInitialData()
        {
            UserDTO adminUser = GetAdminInfo();
            await userService.SetInitialData(adminUser,new List<string> { "user","admin"});

        }
        private UserModel MapToUserModel(UserDTO user)
        {
            return new UserModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Address = user.Address,
                Id = user.Id,
                Name = user.Name,
            };
        }

        private async Task<IEnumerable<UserModel>> GetAllUsers()
        {
            var usersDto = await userService.GetAllUsers();
            if(usersDto == null) return Enumerable.Empty<UserModel>();   
            var users = new List<UserModel>();
            foreach(var userDto in usersDto)
            {
                var userModel = MapToUserModel(userDto);
                users.Add(userModel);
            }
            return users;
        }
        private UserDTO GetAdminInfo()
        {
            return new UserDTO
            {
                Email = "MRSUTWEB@mail.com",
                UserName = "Admin",
                Password = "Admin123",
                Name= "Application Admin",
                Address = "Chisinau,str.Studentilor",
                Role = "admin",
            };
        }
        protected override void Dispose(bool disposing)
        {
            userService.Dispose();
            cartService.Dispose();
            base.Dispose(disposing);
        }
        #endregion
    }
}