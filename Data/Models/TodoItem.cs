using Data.Common;

namespace Data.Models
{
    public class TodoItem : BaseModel
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}