using MRSTWEb.BuisnessLogic.Interfaces;
using MRSTWEb.BuisnessLogic.Services;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MRSTWEb.Util
{
    public class CartModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ICartService>().To<CartService>();
        }
    }
}