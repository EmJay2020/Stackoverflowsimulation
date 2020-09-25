using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EFManytoMany.data
{
   public class UserRepository
    {
        private string _connectionString;
        public UserRepository(string cs)
        {
            _connectionString = cs;
        }
        public IEnumerable<Questions> GetQuestions()
        {
            using(var context = new UserContext(_connectionString))
            {
                return context.Questions.Include(l => l.Likes)          
                    .ToList();
            }
        }
        public Questions Question(int id)
        {
            using(var context = new UserContext(_connectionString))
            {
                return context.Questions.Include(c => c.Likes)
                    .Include(a => a.Answers)
                    .FirstOrDefault(i => i.Id == id);
            }
        }
        public IEnumerable<Questions> GetQuestionsForTag(string name)
        {
            using(var context = new UserContext(_connectionString))
            {
                return context.Questions.Include(l => l.Likes).Include(q => q.QuestionsTags)
                    .ThenInclude(qt => qt.Tag)
                    .Where(c => c.QuestionsTags.Any(t => t.Tag.Name == name))
                    .ToList();
            }
        }
        public IEnumerable<Tag> GetTagsPerQuestion(int id)
        {
            using(var context = new UserContext(_connectionString))
            {
                return context.Tags.Include(c => c.QuestionsTags)
                    .ThenInclude(q => q.Questions)
                    .Where(t => t.QuestionsTags.Any(e => e.Questions.Id == id))
                    .ToList();
            }
        }
        private Tag GetTag(string name)
        {
            using(var context = new UserContext(_connectionString))
            {
                return context.Tags.FirstOrDefault(t => t.Name == name);
            }
        }
        private int AddTag(string name)
        {
            using(var context = new UserContext(_connectionString))
            {
                var tag = new Tag { Name = name };
                context.Tags.Add(tag);
                context.SaveChanges();
                return tag.Id;
            }
        }
        public void AddQuestion(Questions question, IEnumerable<string> tags)
        {
            using(var context = new UserContext(_connectionString))
            {
                context.Questions.Add(question);
                foreach(var tag in tags)
                {
                    Tag t = GetTag(tag);
                    int tagId;
                    if (t == null)
                    {
                        tagId = AddTag(tag);
                    }
                    else
                    {
                        tagId = t.Id;
                    }
                    context.QuestionsTags.Add(new QuestionsTags
                    {
                        QuestionsId = question.Id,
                        TagId = tagId
                    });
                }
                context.SaveChanges();
            }
        }
        public void AddAnswer(Answers answer)
        {
            using(var context = new UserContext(_connectionString))
            {
                context.Answers.Add(answer);
                context.SaveChanges();
            }
        }
        public void SignUp(string email, string password)
        {
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            var u = new User
            {
                Email = email,
                Password = hash
            };
            using(var context = new UserContext(_connectionString))
            {
                context.Users.Add(u);
                context.SaveChanges();
            }
        }
        public void AddLike(Likes like)
        {
            using(var context = new UserContext(_connectionString))
            {
                context.Likes.Add(like);
                context.SaveChanges();
            }
        }
        public User GetByEmail(string email)
        {
            using(var context = new UserContext(_connectionString))
            {
                return context.Users.Include(c => c.Likes) 
                    .Include(a => a.Answers)
                    .FirstOrDefault(u => u.Email == email);
            }
        }
        public User Login(string email, string password)
        {
            var u = GetByEmail(email);
            if(u == null)
            {
                return null;
            }
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, u.Password);
            if (isValidPassword)
            {
                return u;
            }
            return null;
        }
        public bool IsEmailAvailable(string email)
        {
            using(var context =  new UserContext(_connectionString))
            {
                bool isUsed = context.Users.Any(u => u.Email == email);
                return !isUsed;
            }
        }
        
    }
}
