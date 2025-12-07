using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDbConsoleDemo
{
    public class TodoItem
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Status { get; set; }   // e.g. "New", "InProgress", "Done"
        public string Category { get; set; } // Used as partition key
    }

}
