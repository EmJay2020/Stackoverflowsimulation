using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFManytoMany.data;

namespace EFManytoMany.web
{
    public class QviewModel
    {
        public Questions Question { get; set; }
        public bool DidLike { get; set; }
        public List<Tag> Tags { get; set; }
        public bool DidAnswer { get; set; }
        public bool IsAuth { get; set; }
    }
}
