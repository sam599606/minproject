using System.ComponentModel;
using System.Runtime.Serialization.Formatters;
using minproject.Models.lesson;
using minproject.Models.question;

namespace minproject.ViewModels.search
{
    public class search
    {
        public int? Type { get; set; }
        public int? Year { get; set; }

    }
}