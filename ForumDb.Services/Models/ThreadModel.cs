using ForumDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace ForumDb.Services.Models
{
    [DataContract]
    public class ThreadModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="dateCreated")]
        public DateTime DateCreated { get; set; }

        [DataMember(Name="content")]
        public string Content { get; set; }

        [DataMember(Name="createdBy")]
        public string CreatedBy { get; set; }

        [DataMember(Name="categories")]
        public IEnumerable<string> Categories { get; set; }

        [DataMember(Name = "posts")]
        public IEnumerable<PostModel> Posts { get; set; }

        public static ThreadModel CreateFromThreadEntity(Thread entity)
        {
            var postEntities = entity.Posts.ToList();
            var currentThreadPostModels = new List<PostModel>();

            foreach (var postEntity in postEntities)
            {
                currentThreadPostModels.Add(PostModel.CreateFromPostEntity(postEntity));
            }

            var threadModel = new ThreadModel()
            {
                Id = entity.Id,
                Title = entity.Title,
                DateCreated = entity.DateCreated,
                Content = entity.Content,
                CreatedBy = entity.Creator.Nickname,
                Categories = from cat in entity.Categories
                             select cat.Name,
                Posts = currentThreadPostModels
            };

            return threadModel;
        }
    }
}