﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreNote.Common.ExtensionMethods;
using MoreNote.Common.ModelBinder;
using MoreNote.Common.Utils;
using MoreNote.Framework.Controllers;
using MoreNote.Logic.Entity;
using MoreNote.Logic.Model;
using MoreNote.Logic.Models.Entity.Leanote;
using MoreNote.Logic.Service;
using MoreNote.Logic.Service.Logging;
using MoreNote.Logic.Service.MyRepository;
using MoreNote.Models.Entity.Leanote;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MoreNote.Controllers
{
    public class BlogController : BaseController
    {
        private AccessService accessService;
        private BlogService blogService;
        private ConfigService configService;
        private TagService tagService;
        private NotebookService notebookService;
        private NoteService noteService;
        private NoteRepositoryService noteRepository;
        public BlogController(AttachService attachService
            , TokenSerivce tokenSerivce
            , NoteFileService noteFileService
            , UserService userService
            , ConfigFileService configFileService
            , IHttpContextAccessor accessor
            , AccessService accessService
            , ConfigService configService
            , TagService tagService
            , NoteService noteService
            , NotebookService notebookService
            , BlogService blogService
            , NoteRepositoryService noteRepository
            ) : 
            base(attachService, tokenSerivce, noteFileService, userService, configFileService, accessor)
        {
            this.accessService = accessService;
            this.blogService = blogService;
            this.configService = configService;
            this.tagService = tagService;
            this.notebookService = notebookService;
            this.noteService = noteService;
            this.noteRepository = noteRepository;
        }

        private IActionResult render(string templateName, string themePath)
        {
            var isPreview = false;
            if (ViewBag.isPreview == null)
            {
                var themePath2 = ViewBag.themePath;
                if (themePath2 == null)
                {
                    return E404();
                }
                isPreview = true;
                themePath = themePath2;
                //setPreviewUrl();
                // 因为common的themeInfo是从UserBlog.ThemeId来取的, 所以这里要fugai下
                ViewBag.themeInfo = ViewBag.themeInfoPreview;
            }
            //todo:RenderTemplateStr
            return null;
        }

 

        private IActionResult E404()
        {
            ViewBag.title = "404";
            return NoFound();
        }

        private async Task InsertLogAsync(string url)
        {
            var headers = Request.Headers;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in headers)
            {
                stringBuilder.Append(item.Key + "---" + item.Value + "\r\n");
            }
            string RealIP = headers["X-Forwarded-For"].ToString().Split(",")[0];

            AccessRecords accessRecords = new AccessRecords()
            {
                Id = idGenerator.NextId(),
                IP = RealIP,
                X_Real_IP = headers["X-Real-IP"],
                X_Forwarded_For = headers["X-Forwarded-For"],
                Referrer = headers["Referer"],
                RequestHeader = stringBuilder.ToString(),
                AccessTime = DateTime.Now,
                UnixTime = UnixTimeUtil.GetTimeStampInLong(),
                TimeInterval = -1,
                URL = url
            };
            await accessService.InsertAccessAsync(accessRecords).ConfigureAwait(false);
        }


        private User ActionInitBlogUser(NotesRepository repository)
        {
            
            User blogUser = userService.GetUserByUserId(repository.OwnerId);
            ViewBag.blogUser = blogUser;
            if (blogUser == null)
            {
                return null;
            }
            ViewBag.CateArray = blogService.GetCateArrayForBlog(blogUser.Id,repository.Id);
            return blogUser;
        }

        /// <summary>
        /// 博客归档
        /// </summary>
        /// <param name="repositoryId">仓库id</param>
        /// <param name="archiveHex"></param>
        /// <returns></returns>
        [Route("Blog/{repositoryId?}/Archive/{archiveHex?}")]
        [HttpGet]
        public IActionResult Archive(string repositoryId, string archiveHex)
        {
            var repository = this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            User blogUser = ActionInitBlogUser(repository);
            if (blogUser == null)
            {
                return Content("查无此人");
            }
            Dictionary<string, string> blog = new Dictionary<string, string>();
            var list = blogService.GetNotes(blogUser.Id, repository.Id);

            IOrderedEnumerable<IGrouping<int, Note>> queryArchiveList =
                                from note in list
                                group note by note.PublicTime.Year into newGroup
                                orderby newGroup.Key descending
                                select newGroup;
            ViewBag.queryArchiveList = queryArchiveList;
            ViewBag.blogUser = blogUser;
            blog.Add("Title", "标题");
            ViewBag.blog = blog;
            return View();
        }

        /// <summary>
        /// 分类 /cate/xxxxxxxx?notebookId=1212
        /// </summary>
        /// <param name="blogUserName"></param>
        /// <param name="cateHex"></param>
        /// <param name="page"></param>
        /// <returns></returns>

        [Route("Blog/{repositoryId}/Cate/{cateHex}/")]
        [HttpGet]
        public IActionResult Cate(string repositoryId, string cateHex, int page)
        {
            long? notebookId = cateHex.ToLongByHex();
            var repository = this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            User blogUser = ActionInitBlogUser(repository);
            if (blogUser == null)
            {
                return Content("无此仓库或此仓库已经失效");
            }
            if (page < 1)
            {
                //页码
                page = 1;
            }
            ViewBag.page = page;
            Notebook notebook = notebookService.GetNotebookById(notebookId,repository.Id);
            ViewBag.notebook = notebook;

            ViewBag.postCount = blogService.CountTheNumberForBlogsOfNoteBookId(blogUser.Id, notebookId,repository.Id);
            NoteAndContent[] noteAndContent = noteService.GetNoteAndContentForBlogOfNoteBookId(page, notebookId, blogUser.Id,repository.Id);
            SetAccessPassword(noteAndContent);
            ViewBag.noteAndContent = noteAndContent;

            if (blogUser == null)
            {
                return Content("查无此人");
            }
            ViewBag.CateArray = blogService.GetCateArrayForBlog(blogUser.Id,repository.Id);
            Dictionary<string, string> blog = new Dictionary<string, string>();
            blog.Add("Title", $"分类-{notebook.Title}");
            blog.Add("keywords", "关键字");
            ViewBag.blog = blog;
            return View();
        }

     
        [Route("Blog/{repositoryId}/Index/{page?}")]
        [HttpGet]
        public IActionResult Index(string repositoryId, int page)
        {
            if (page < 1)
            {
                //页码
                page = 1;
            }
            ViewBag.page = page;
            var repository=this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            User blogUser =userService.GetUserByUserId(repository.OwnerId);
            if (string.IsNullOrEmpty(repositoryId)|| repository==null|| repository.IsDelete || !repository.IsBlog)
            {
                Response.StatusCode=404;
                return Content("仓库无法访问");
            }
            else
            {
                //blogUser = userService.GetUserByUserId(blogUserIdHex.ToLongByHex());
            }

            ViewBag.blogUser = blogUser;
            ViewBag.repository = repository;
            if (!blogUser.Verified)
            {
                return Content("仓库拥有者用户未实名认证");
            }
            ViewBag.postCount = blogService.CountTheNumberForBlogs(blogUser.Id);
            NoteAndContent[] noteAndContent = noteService.GetNoteAndContentForBlog(page, blogUser.Id, repository.Id);
            SetAccessPassword(noteAndContent);

            ViewBag.noteAndContent = noteAndContent;
            ViewBag.CateArray = blogService.GetCateArrayForBlog(blogUser.Id, repository.Id);

            Dictionary<string, string> blog = new Dictionary<string, string>();
            blog.Add("Title", "moreote云笔记");
            blog.Add("keywords", "搜索");
            ViewBag.blog = blog;

            BlogCommon(blogUser);

            return View();
        }



        //[Route("Blog/{repository}/Post/{noteIdHex}/1")]
        //[HttpGet]
        //public IActionResult Post1(string noteIdHex)
        //{
        //    long? noteId = noteIdHex.ToLongByHex();
        //    Note note = noteService.GetNoteById(noteId);
        //    User user = userService.GetUserByUserId(note.UserId);
        //    return Redirect($"/Blog/Post/{user.Username}/{noteIdHex}");
        //}

      
        [Route("Blog/{repositoryId}/Post/{noteIdHex}/")]
        [HttpGet]
        public async Task<IActionResult> Post(string repositoryId, string noteIdHex)
        {
            var repository = this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            //添加访问日志
            await InsertLogAsync($"Blog/Post/{repository.Name}/{noteIdHex}/").ConfigureAwait(false);

            User blogUser = ActionInitBlogUser(repository);
            if (blogUser == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content("查无此人");
            }
            long? noteId = noteIdHex.ToLongByHex();
            if (noteId == 0)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content("输入参数无效");
            }

            Dictionary<string, string> blog = new Dictionary<string, string>();

            NoteAndContent noteAndContent = noteService.GetNoteAndContent(noteId);

            //访问校验
            if (noteAndContent == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content("未经授权的访问");
            }

            if (noteAndContent.note.IsDeleted || noteAndContent.note.IsTrash)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Content("这篇文章已经被删除");
            }

            if (!noteAndContent.note.IsBlog || !repository.IsBlog)
            {
                Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Content("这篇文章已经被取消分享");
            }

            if (!blogUser.Verified)
            {
                return Content("用户未实名认证");
            }

            if (!string.IsNullOrEmpty(noteAndContent.note.AccessPassword))
            {
                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    Response.StatusCode = 401;
                    Response.Headers.Add("WWW-Authenticate", $"Basic realm='{config.APPConfig.SiteUrl}/Blog/Post/{repositoryId}/{noteIdHex}'");
                    return Content("Missing Authorization Header");
                }
                else
                {
                    var authorization = Request.Headers["Authorization"].ToString().Replace("Basic", "");
                    var basic = Base64Util.UnBase64String(authorization);
                    var sp = basic.Split(":");
                    var user = sp[0];
                    var password = sp[1];
                    if (!noteService.VerifyAccessPassword(noteAndContent.note.UserId, noteId, password, noteAndContent.note.AccessPassword))
                    {
                        Response.StatusCode = 401;
                        Response.Headers.Add("WWW-Authenticate", $"Basic realm='{config.APPConfig.SiteUrl}/Blog/Post/{repositoryId}/{noteIdHex}'");
                        return Content("Missing Authorization Header");
                    }
                    else
                    {
                    }
                }
            }

            noteService.AddReadNum(noteId);

            UserBlog userBlog = blogService.GetUserBlog(blogUser.Id);
            BlogCommon(blogUser.Id, userBlog, blogUser);
            ViewBag.noteAndContent = noteAndContent;
            blog.Add("Title", noteAndContent.note.Title);
            blog.Add("NoteTitle", noteAndContent.note.Title);
            blog.Add("keywords", "关键字");
            ViewBag.blog = blog;
            return View();
        }

        [Route("Blog/{repositoryId}/Tags/")]
        [HttpGet]
        public IActionResult Tags(string repositoryId)
        {
            var repository = this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            User blogUser = ActionInitBlogUser(repository);
            if (blogUser == null)
            {
                return Content("查无此人");
            }
            Dictionary<string, string> blog = new Dictionary<string, string>();
            string[] tags = tagService.GetBlogTags(blogUser.Id,repository.Id);
            ViewBag.tags = tags;
            ViewBag.blogUser = blogUser;
            blog.Add("Title", "标签云");
            blog.Add("keywords", "关键字");
            ViewBag.blog = blog;

            BlogCommon(blogUser);
            return View();
        }

        [Route("Blog/{repositoryId}/Search/{keywords?}/")]
        [HttpGet]
        public IActionResult Search(string repositoryId, string keywords, int page)
        {
            var repository = this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            if (page < 1)
            {
                //页码
                page = 1;
            }
            ViewBag.page = page;
            User blogUser = null;
            blogUser = userService.GetUserByUserId(repository.OwnerId);
            ViewBag.blogUser = blogUser;

            ViewBag.postCount = blogService.CountTheNumberForSearch(blogUser.Id,repository.Id, keywords);
            NoteAndContent[] noteAndContent = noteService.SearchNoteAndContentForBlog(page, blogUser.Id,repository.Id, keywords);
            SetAccessPassword(noteAndContent);
            ViewBag.noteAndContent = noteAndContent;
            ViewBag.CateArray = blogService.GetCateArrayForBlog(blogUser.Id,repository.Id);
            ViewBag.keywords = keywords;

            Dictionary<string, string> blog = new Dictionary<string, string>();
            blog.Add("Title", "moreote云笔记");
            blog.Add("keywords", "搜索");
            ViewBag.blog = blog;

            BlogCommon(blogUser);
            return View();
        }
        private void SetAccessPassword(NoteAndContent[] noteAndContent)
        {
            var languageResource=this.GetLanguageResource();
            var msg=languageResource.GetMsg();
            var needAccessPassword=msg["needAccessPassword"];

              foreach (var item in noteAndContent)
            {
                if (!string.IsNullOrEmpty(item.note.AccessPassword))
                {
                    item.noteContent.Abstract = needAccessPassword;
                }
            }

        }

        [Route("Blog/{repositoryId}/Single/{SingleIdHex}/")]
        [HttpGet]
        public IActionResult Single(string repositoryId, string SingleIdHex)
        {

            var repository = this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            User blogUser = ActionInitBlogUser(repository);
            if (blogUser == null)
            {
                return Content("查无此人");
            }
            Dictionary<string, string> blog = new Dictionary<string, string>();
            blog.Add("Title", "标题");
            blog.Add("keywords", "关键字");
            ViewBag.blog = blog;
            BlogCommon(blogUser);
            return View();
        }

        [Route("Blog/{repositoryId}/Tags_Posts/{tag}/")]
        [HttpGet]
        public IActionResult Tags_Posts(string repositoryId, string tag, int page)
        {
            var repository = this.noteRepository.GetNotesRepository(repositoryId.ToLongByHex());
            ViewBag.repository = repository;
            if (page < 1)
            {
                //页码
                page = 1;
            }
            ViewBag.page = page;

            User blogUser = ActionInitBlogUser(repository);
            if (blogUser == null)
            {
                return Content("查无此人");
            }
            ViewBag.blogUser = blogUser;

            ViewBag.postCount = blogService.CountTheNumberForBlogTags(blogUser.Id, repository.Id, tag);
            NoteAndContent[] noteAndContent = noteService.GetNoteAndContentByTag(page, blogUser.Id, repository.Id, tag);
           
            SetAccessPassword(noteAndContent);
            ViewBag.noteAndContent = noteAndContent;
            ViewBag.CateArray = blogService.GetCateArrayForBlog(blogUser.Id);

            Dictionary<string, string> blog = new Dictionary<string, string>();
            blog.Add("Title", "标签检索");
            blog.Add("keywords", "搜索");
            ViewBag.blog = blog;
            BlogCommon(blogUser);
            return View();
        }

        [Route("Blog/NoFound")]
        [HttpGet]
        public IActionResult NoFound()
        {
            Dictionary<string, string> blog = new Dictionary<string, string>();
            blog.Add("Title", "标题");
            blog.Add("keywords", "关键字");
            ViewBag.blog = blog;
            return View();
        }

        public IActionResult IncReadNum(string noteId)
        {
            ResponseMessage re = new ResponseMessage();
            long? noteNum = noteId.ToLongByHex();
            re.Ok = blogService.IncReadNum(noteNum);

            return Json(re, MyJsonConvert.GetLeanoteOptions());
        }
         [Route("Blog/getLikesAndComments")]
        [HttpGet]
        public IActionResult GetLikesAndComments([ModelBinder(typeof(Hex2LongModelBinder))] long? noteId, string callback)
        {
            long? userId = GetUserIdBySession();
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();

            // 我也点过?
            var isILikeIt = false;
            if (userId != null)
            {
                isILikeIt = blogService.IsILikeIt(noteId, userId);
                var userAndBlog = userService.GetUserAndBlog(userId);
                result.Add("visitUserInfo", userAndBlog);
            }

            // 点赞用户列表
            bool hasMoreLikedUser = false;
            var likedUsers = blogService.ListLikedUsers(noteId, false, out hasMoreLikedUser);
            // 评论
            var page = this.GetPage();
            blogService.ListComments(userId, noteId, page, 15, out Page pageInfo, out BlogCommentPublic[] comments, out Dictionary<string, UserAndBlog> commentUserInfo);

            result.Add("isILikeIt", isILikeIt);
            result.Add("likedUsers", likedUsers);
            result.Add("hasMoreLikedUser", hasMoreLikedUser);
            result.Add("pageInfo", pageInfo);
            result.Add("comments", comments);
            result.Add("commentUserInfo", commentUserInfo);

            ResponseMessage re = new ResponseMessage()
            {
                Ok = true,
                Item = result
            };

            string json = JsonSerializer.Serialize(re, MyJsonConvert.GetSimpleOptions());
            string jsonpCallback = $"jsonpCallback({json});";
            return new JavaScriptResult(jsonpCallback);
        }

        public UserBlog BlogCommon(User userInfo)
        {
            UserBlog userBlog = blogService.GetUserBlog(userInfo.Id);
            return BlogCommon(userInfo.Id, userBlog, userInfo);
        }
        public UserBlog BlogCommon(long? userId, UserBlog userBlog, User userInfo)
        {
            if (userInfo.Id == 0)
            {
                userInfo = userService.GetUserByUserId(userId);
                if (userInfo.Id == 0)
                {
                    return null;
                }
            }

            //	c.ViewArgs["userInfo"] = userInfo

            // 最新笔记
            // 语言, url地址
            // 得到博客设置信息

            SetBlog(userBlog, userInfo);

            //	c.ViewArgs["userBlog"] = userBlog
            // 分类导航

            // 单页导航
            SetUrl(userBlog, userInfo);
            // 当前分类Id, 全设为""
            // 得到主题信息
            // var recentBlogs = BlogService.ListBlogs(userId, "", 1, 5, userBlog.SortField, userBlog.IsAsc);

            return null;
        }
        public void SetBlog(UserBlog userBlog, User userInfo)
        {
            BlogInfo blogInfo = blogService.GetBlogInfo(userBlog, userInfo);
            ViewBag.blogInfo = blogInfo;
        }

        // 各种地址设置
        public void SetUrl(UserBlog userBlog, User user)
        {
            // 主页 http://leanote.com/blog/life or http://blog.leanote.com/life or http:// xxxx.leanote.com or aa.com
            // host := c.Request.Request.Host
            // var staticUrl = configService.GetUserUrl(strings.Split(host, ":")[0])
            // staticUrl == host, 为保证同源!!! 只有host, http://leanote.com, http://blog/leanote.com
            // life.leanote.com, lealife.com
            var siteUrl = configService.GetSiteUrl();
            var blogUrls = blogService.GetBlogUrls(userBlog, user);
            // 分类
            // 搜索
            // 查看
            ViewBag.siteUrl = siteUrl;
            ViewBag.indexUrl = blogUrls.IndexUrl;
            ViewBag.cateUrl = blogUrls.CateUrl;
            ViewBag.postUrl = blogUrls.PostUrl;
            ViewBag.searchUrl = blogUrls.SearchUrl;
            ViewBag.singleUrl = blogUrls.SingleUrl;
            ViewBag.archiveUrl = blogUrls.ArchiveUrl;
            ViewBag.archivesUrl = blogUrls.ArchiveUrl;
            ViewBag.tagsUrl = blogUrls.TagsUrl;
            ViewBag.tagPostsUrl = blogUrls.TagPostsUrl;
            ViewBag.tagUrl = blogUrls.TagsUrl;
            // themeBaseUrl 本theme的路径url, 可以加载js, css, images之类的
            ViewBag.themeBaseUrl = "/" + userBlog.ThemePath;

            // 其它static js
            ViewBag.jQueryUrl = "/js/jquery-1.9.0.min.js";

            ViewBag.prettifyJsUrl = "/js/google-code-prettify/prettify.js";

            ViewBag.prettifyCssUrl = "/js/google-code-prettify/prettify.css";

            ViewBag.blogCommonJsUrl = "/public/blog/js/common.js";

            ViewBag.shareCommentCssUrl = "/public/blog/css/share_comment.css";

            ViewBag.shareCommentJsUrl = "/public/blog/js/share_comment.js";

            ViewBag.fontAwesomeUrl = "/css/font-awesome-4.2.0/css/font-awesome.css";

            ViewBag.bootstrapCssUrl = "/css/bootstrap.css";

            ViewBag.bootstrapJsUrl = "/js/bootstrap-min.js";
        }
    }
}