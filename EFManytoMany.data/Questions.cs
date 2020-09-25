using System;
using System.Collections.Generic;
using System.Text;

namespace EFManytoMany.data
{
    public class Questions
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Answers> Answers { get; set; }
        public List<Likes> Likes { get; set; }
        public List<QuestionsTags> QuestionsTags { get; set; }
    }
}
