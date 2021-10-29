using Data.Common;

namespace Data.Entities
{
    public class TodoItem : BaseModel
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
    }
}