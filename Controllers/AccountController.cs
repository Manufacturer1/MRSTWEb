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
using System.Net;
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
        private IOrderService orderService;
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

        public AccountController(ICartService cartService,IOrderService orderService)
        {
            this.cartService = cartService;
            this.orderService = orderService;
        }

        [Authorize(Roles ="admin")]
        [SessionTimeout]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            if(userId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var result = await userService.DeleteUserByUserId(userId);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }
            return RedirectToAction("OtherUsers");
        }

        [Authorize(Roles ="admin")]
        [SessionTimeout]
        public async Task<ActionResult> OtherUsers()
        {
            var users = await GetAllUsers();
            return View(users); 
        }

        [SessionTimeout]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> AdminDashboard()
        {
            var userAdmin = await GetUserAdmin();
            var adminModel = MapToUserModel(userAdmin);

            return View(adminModel);
        }

        [HttpGet]
        [SessionTimeout]
        [Authorize]
        public ActionResult ChangePassword()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> ChangePassword(PasswordModel model)
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            if (user == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var passwordHasher = new PasswordHasher();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user.Password, model.CurrentPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Success)
            {
                var newPasswordHash = passwordHasher.HashPassword(model.Password);
                user.Password = newPasswordHash;
                OperationDetails operationDetails = await userService.UpdateClient(user);

                if (operationDetails.Succeeded)
                {
                    ViewBag.PasswordChanged = true;
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Error changing password. Please try again.");
                    return View(model);
                }
            }
            else
            {

                ModelState.AddModelError("", "The current password is incorrect.");
                return View(model);
            }


        }


        [Authorize]
        [SessionTimeout]
        public async Task<ActionResult> ClientProfile()
        {
            var userDto = await userService.GetUserById(User.Identity.GetUserId());
            var user = new UserModel { Id = userDto.Id,Email=userDto.Email,Address=userDto.Address,Name=userDto.Name,UserName=userDto.UserName };

            if(user == null) return HttpNotFound();

            return View(user);
        }


        [HttpGet]
        [SessionTimeout]
        public async Task<ActionResult> EditClientProfile()
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            var editModel = new EditModel
            {
                UserName = user.UserName,
                Name = user.Name,
                Address = user.Address,
                Email = user.Email,
            };
            return View(editModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "user, admin")]
        [SessionTimeout]
        public async Task<ActionResult> EditClientProfile(EditModel model)
        {
            var user = await userService.GetUserById(User.Identity.GetUserId());
            if(user == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotFound);

            if (ModelState.IsValid)
            {
                // Update the properties only if the model properties are not empty
                if (!string.IsNullOrEmpty(model.Email))
                {
                    user.Email = model.Email;
                }
                if (!string.IsNullOrEmpty(model.Name))
                {
                    user.Name = model.Name;
                }

                if (!string.IsNullOrEmpty(model.Address))
                {
                    user.Address = model.Address;
                }


                if (!string.IsNullOrEmpty(model.UserName))
                {
                    user.UserName = model.UserName;
                }
                if (!string.IsNullOrEmpty(model.Email))
                {

                }
                OperationDetails operationDetails = await userService.UpdateClient(user);

                if (operationDetails.Succeeded)
                {
                    if (User.IsInRole("admin"))
                    {
                        return RedirectToAction("AdminDashboard");
                    }
                    else
                    {
                        return RedirectToAction("ClientProfile");
                    }
                }
            }
            return View(model);
        }

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
                        Session["UserId"] = "admin";
                        authenticationManager.SignOut();
                        authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);
                        return RedirectToAction("AdminDashboard", "Account");
                    }
                    else if(userRole == "user")
                    {
                        Session["UserId"] = "user";
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
          /*  await SetInitialData();*/
            if (ModelState.IsValid)
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
                if (operationDetalis.Succeeded) return RedirectToAction("Login");
                else ModelState.AddModelError(operationDetalis.Property, operationDetalis.Message);
            }
            return View(model);
        }

        [Authorize(Roles ="admin")]
        public async Task<ActionResult> OrderDetails(string userId)
        {
            var user = await GetUserWithOrders(userId);
            return View(user);
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
        private async Task<UserDTO> GetUserAdmin()
        {
            var userId = User.Identity.GetUserId();
            return await userService.GetUserById(userId);
        }

        private async Task<UserModel> GetUserWithOrders(string userId)
        {
            var userDto = await userService.GetUserById(userId);
            var ordersDto = orderService.GetOrdersByUserId(userId);
            var user = new UserModel();
            if(userDto != null)
            {
               user = MapToUserModel(userDto);
                AddOrdersToUser(user, ordersDto);   

            }
            return user;
        }
        private OrderViewModel MapOrderToOrderViewModel(OrderDTO orderDTO)
        {
            return new OrderViewModel
            {
                Id = orderDTO.Id,
                FirstName = orderDTO.FirstName,
                LastName = orderDTO.LastName,
                Address = orderDTO.Address,
                Phone = orderDTO.Phone,
                PostCode = orderDTO.PostCode,
                BuyingTime = orderDTO.BuyingTime,
                Email = orderDTO.Email,
                City = orderDTO.City,
                ApplicationUserId = orderDTO.ApplicationUserId,
                TotalSumToPay = orderDTO.TotalSumToPay,
                Items = orderDTO.Items,
            };
        }
        private void AddOrdersToUser(UserModel user,IEnumerable<OrderDTO> ordersDto)
        {
            if(ordersDto != null)
            {
                user.Orders = ordersDto.Select(o => MapOrderToOrderViewModel(o)).ToList();
            }
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