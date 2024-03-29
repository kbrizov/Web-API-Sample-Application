﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ForumDb.Services.Models
{
    [DataContract]
    public class PostCreateModel
    {
        [DataMember(Name="content")]
        public string Content { get; set; }

        [DataMember(Name="postDate")]
        public DateTime PostDate { get; set; }

        [DataMember(Name="threadId")]
        public int ThreadId { get; set; }
    }
}