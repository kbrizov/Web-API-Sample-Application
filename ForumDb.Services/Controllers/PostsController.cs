using ForumDb.Models;
using ForumDb.Repositories;
using ForumDb.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ForumDb.Services.Controllers
{
    public class PostsController : BaseApiController
    {
        private readonly IRepository<Post> postRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Thread> threadRepository;
        private readonly IRepository<Vote> voteRepository;
        private readonly IRepository<Comment> commentRepository;

        public PostsController(
            IRepository<Post> postRepository,
            IRepository<User> userRepository,
            IRepository<Thread> threadRepository,
            IRepository<Vote> voteRepository,
            IRepository<Comment> commentRepository)
        {
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.threadRepository = threadRepository;
            this.voteRepository = voteRepository;
            this.commentRepository = commentRepository;
        }

        [HttpGet]
        public IQueryable<PostModel> GetAll(string sessionKey)
        {
            var user = this.userRepository.GetAll()
                .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException("No logged user or invalid sessionKey");
            }

            var postEntities = this.postRepository.GetAll();
            var postModels = new List<PostModel>();
            foreach (var entity in postEntities)
            {
                postModels.Add(PostModel.CreateFromPostEntity(entity));
            }

            return postModels.OrderByDescending(pst => pst.PostDate).AsQueryable<PostModel>();
        }

        [HttpGet]
        public IQueryable<PostModel> GetPage([FromUri]int page, [FromUri]int count, string sessionKey)
        {
            var models = this.GetAll(sessionKey)
                .Skip(page * count)
                .Take(count);

            return models;
        }

        [HttpPost]
        [ActionName("create")]
        public HttpResponseMessage CreatePost([FromBody]PostCreateModel model, string sessionKey)
        {
            var response = this.PerformOperationAndHandleExceptions(() =>
                {
                    var user = this.userRepository.GetAll()
                    .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

                    if (user == null)
                    {
                        throw new InvalidOperationException("No logged user or invalid sessionKey");
                    }

                    var threadEntity = this.threadRepository.GetAll()
                        .Where(thr => thr.Id == model.ThreadId).FirstOrDefault();

                    if (threadEntity == null)
                    {
                        throw new InvalidOperationException("No thread with such id.");
                    }

                    var post = new Post()
                    {
                        Content = model.Content,
                        DateCreated = model.PostDate,
                        User = user,
                        Thread = threadEntity
                    };

                    this.postRepository.Add(post);

                    var responseMessage = this.Request.CreateResponse(
                        HttpStatusCode.Created,
                        new
                        {
                            id = post.Id,
                            postedBy = user.Nickname
                        });

                    return responseMessage;
                });

            return response;
        }

        [HttpPost]
        [ActionName("vote")]
        public HttpResponseMessage VoteForPost([FromBody]VoteModel voteModel, [FromUri]int postId, string sessionKey)
        {
            var response = this.PerformOperationAndHandleExceptions(() =>
                {
                    var user = this.userRepository.GetAll()
                        .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

                    if (user == null)
                    {
                        throw new InvalidOperationException("No logged user or invalid sessionKey");
                    }

                    var post = this.postRepository.GetById(postId);
                    if (post == null)
                    {
                        throw new ArgumentException(string.Format("No post with id = {0}", postId));
                    }

                    if (voteModel.Value < 0 || voteModel.Value > 5)
                    {
                        throw new ArgumentException("The vote value cannot be less than 0 and larger than 5");
                    }

                    var vote = new Vote()
                    {
                        Value = voteModel.Value,
                        User = user,
                        Post = post
                    };

                    post.Votes.Add(vote);
                    this.voteRepository.Add(vote);

                    var responseMessage = this.Request.CreateResponse(HttpStatusCode.OK,
                        new
                        {
                            id = vote.Id,
                            votedBy = user.Nickname,
                            value = vote.Value
                        });

                    return responseMessage;
                });

            return response;
        }

        [HttpPost]
        [ActionName("comment")]
        public HttpResponseMessage CommentForPost([FromBody]CommentModel commentModel, [FromUri]int postId, string sessionKey)
        {
            var response = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.userRepository.GetAll()
                    .Where(usr => usr.SessionKey == sessionKey).FirstOrDefault();

                if (user == null)
                {
                    throw new InvalidOperationException("No logged user or invalid sessionKey");
                }

                var post = this.postRepository.GetById(postId);
                if (post == null)
                {
                    throw new ArgumentException(string.Format("No post with id = {0}", postId));
                }

                if (commentModel.Content == null)
                {
                    throw new ArgumentException("The comment content cannot be null.");
                }

                Comment comment = new Comment()
                {
                    Content = commentModel.Content,
                    DateCreated = commentModel.CommentDate,
                    Post = post,
                    User = user
                };

                user.Comments.Add(comment);
                this.commentRepository.Add(comment);

                var responseMessage = this.Request.CreateResponse(
                    HttpStatusCode.Created, 
                    new 
                    { 
                        id = comment.Id,
                        commentedBy = user.Nickname,
                        content = comment.Content
                    });

                return responseMessage;
            });

            return response;
        }
    }
}
