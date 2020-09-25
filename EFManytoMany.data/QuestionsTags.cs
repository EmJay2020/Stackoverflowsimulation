using System;
using System.Collections.Generic;
using System.Text;

namespace EFManytoMany.data
{
    public class QuestionsTags
    {
        public int QuestionsId { get; set; }
        public int TagId { get; set; }
        public Questions Questions { get; set; }
        public Tag Tag { get; set; }
    }
}
