using System.ComponentModel;
using System.Runtime.Serialization.Formatters;
using minproject.Models.question;

namespace minproject.ViewModels.editquestion
{
    public class editquestion
    {
        //public List<question> questionlist{get;set;}
        [DisplayName("題目")]
        public question question { get; set; }
        [DisplayName("題目圖片")]
        public IFormFile? Image { get; set; }
    }
}