using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDb.Services.Tests
{
    internal class Route
    {
        public object Defaults { get; set; }

        public string Name { get; set; }

        public string Template { get; set; }

        public Route(string name, string template, object defaults)
        {
            this.Defaults = defaults;
            this.Name = name;
            this.Template = template;
        }
    }
}
