using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using RedisSample.Models;

namespace RedisSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var viewModel = CreateNewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(UserInfoViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = CreateNewModel();
            }
            else if (viewModel.EditAnswers)
            {
                viewModel = CreateNewModel();
                viewModel.EditAnswers = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(viewModel.FavoriteColor))
                    HttpContext.Session.SetString("FavoriteColor", viewModel.FavoriteColor);
                if (!string.IsNullOrEmpty(viewModel.FavoritePet))
                    HttpContext.Session.SetString("FavoritePet", viewModel.FavoritePet);
                if (!string.IsNullOrEmpty(viewModel.UserName))
                    HttpContext.Session.SetString("Username", viewModel.UserName);
            }

            viewModel.Instance = Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX");

            return View(viewModel);
        }

        private UserInfoViewModel CreateNewModel()
        {
            var viewModel = new UserInfoViewModel()
            {
                FavoriteColor = HttpContext.Session.GetString("FavoriteColor"),
                FavoritePet = HttpContext.Session.GetString("FavoritePet"),
                Instance = Environment.GetEnvironmentVariable("CF_INSTANCE_INDEX"),
                UserName = HttpContext.Session.GetString("Username")
            };

            return viewModel;
        }
    }
}
