using System.Collections.Generic;
using System.Linq;

namespace Tour_MVC.Models
{
    public class Paginated<T>: List<T>
    {
        public int TotalRecords { get; set; }

        public Paginated(List<T> source,int pageIndex,int pageSize)
        {
            TotalRecords = source.Count;
            var items = source.Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList();
            this.AddRange(items);
        }
    }
}
