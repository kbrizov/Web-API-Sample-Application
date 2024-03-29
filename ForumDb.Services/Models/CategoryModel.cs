﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ForumDb.Services.Models
{
    [DataContract]
    public class CategoryModel
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="threadTitles")]
        public IEnumerable<string> threadTitles { get; set; }
    }
}