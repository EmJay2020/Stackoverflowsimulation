using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EFManytoMany.web.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using EFManytoMany.data;

namespace EFManytoMany.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _connectionString;


        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConStr");
            _logger = logger;
        }
        public IActionResult Home()
        {
            var repo = new UserRepository(_connectionString);
            
            return View(repo.GetQuestions());
        }
        [HttpPost]
        public IActionResult Home(string Name)
        {
            var repo = new UserRepository(_connectionString);
            return View(repo.GetQuestionsForTag(Name));
        }
        public IActionResult Q(int Id)
        {
            var repo = new UserRepository(_connectionString);
            var vm = new QviewModel();
            vm.Question = repo.Question(Id);
            if(User.Identity.Name != null)
            {
                var a = repo.GetByEmail(User.Identity.Name);
                vm.DidLike = a.Likes.Any(i => i.QuestionsId == Id);
                vm.DidAnswer = a.Answers.Any(i => i.QuestionsId == Id);
                vm.IsAuth = true;
            }
            vm.Tags = repo.GetTagsPerQuestion(Id).ToList();
            return View(vm);
        }
        [Authorize]
        public IActionResult Ask()
        {
            return View();
        }
        [HttpPost]
        public IActionResult submit(Questions question, IEnumerable<string> tags)
        {
            var repo = new UserRepository(_connectionString);
            var q = new Questions();
            q = question;
            var a = repo.GetByEmail(User.Identity.Name);
            q.UserId = a.Id;
            repo.AddQuestion(q, tags);
            return Redirect("/home/home");
        }
        [HttpPost]
        public IActionResult Answer(string text, int Id)
        {
            var repo = new UserRepository(_connectionString);
            var a = repo.GetByEmail(User.Identity.Name);
            var answer = new Answers
            {
                QuestionsId = Id,
                UserId = a.Id,
                Text = text
            };
            repo.AddAnswer(answer);
            return Redirect($"/home/home");

            
        }
        [HttpPost]
        public IActionResult Like(int Id)
        {
            var repo = new UserRepository(_connectionString);
            var a = repo.GetByEmail(User.Identity.Name);
            var like = new Likes
            {
                QuestionsId = Id,
                UserId = a.Id
            };
            repo.AddLike(like);
            return Json("");
        }
        public IActionResult GetLikes(int Id)
        {
            var repo = new UserRepository(_connectionString);
            var a = new User();
            var q = repo.Question(Id);
            bool didLike = false;
            bool notIn = true;
            if(User.Identity.Name != null)
            {
                a = repo.GetByEmail(User.Identity.Name);
                didLike = a.Likes.Any(c => c.QuestionsId == Id);
                notIn = false;
            }
            int likeCount = q.Likes.Count();
            var obj = new
            {
                didLike,
                notIn,
                likeCount
            };
            return Json(obj);
            
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
