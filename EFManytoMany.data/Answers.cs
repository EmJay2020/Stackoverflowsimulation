using System;
using System.Collections.Generic;
using System.Text;

namespace EFManytoMany.data
{
    public class Answers
    {
        public string Text { get; set; }
        public int UserId { get; set; }
        public int QuestionsId { get; set; }
       
        public Questions Questions { get; set; }
        public User User { get; set; }

    }
}
