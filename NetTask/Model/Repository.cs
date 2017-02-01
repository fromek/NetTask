using NetTask.Model.Interfaces;

namespace NetTask.Model
{
    public class Repository : IRepository
    {
        public string name { get; set; }
        public string id { get; set; }
        public string full_name { get; set; }

    }
}
