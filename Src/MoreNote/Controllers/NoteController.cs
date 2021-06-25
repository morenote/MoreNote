﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using MoreNote.Common.ExtensionMethods;
using MoreNote.Common.ModelBinder;
using MoreNote.Common.Utils;
using MoreNote.Framework.Controllers;
using MoreNote.Logic.Entity;
using MoreNote.Logic.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MoreNote.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin,User")]
    public class NoteController : BaseController
    {
        private NotebookService notebookService;
        private NoteContentService noteContentService;
        private NoteService noteService;
        private ShareService shareService;
        private TagService tagService;
        IWebHostEnvironment env;
        private TrashService trashService;
        public NoteController(AttachService attachService
            , TokenSerivce tokenSerivce
            , NoteFileService noteFileService
            , UserService userService
            ,ShareService shareService
            , ConfigFileService configFileService
            ,IWebHostEnvironment env
            ,TagService tagService
            ,TrashService trashService
            , IHttpContextAccessor accessor, NotebookService notebookService, NoteService noteService, NoteContentService noteContentService) : base(attachService, tokenSerivce, noteFileService, userService, configFileService, accessor)
        {
            this.notebookService = notebookService;
            this.noteService = noteService;
            this.noteContentService = noteContentService;
            this.env=env;
            this.tagService=tagService;
            this.trashService=trashService;
        }

        [Route("Note/{action=Editor}/{noteIdHex?}/")]
        [Authorize(Roles = "Admin,SuperAdmin,User")]
        public IActionResult Editor(string noteIdHex)
        {
            this.SetLocale();//设置区域化信息
            //todo:存在一个问题，可能导出用户敏感信息
            var userInfo = this.GetUserAndBlogUrl();//得到用户信息+博客主页
            //判断用户ID是否已经登录
            var user = this.GetUserBySession();
            var userId=user.UserId;
    
            Notebook[] noteBoooks = notebookService.GetNoteBookTree(user.UserId);

            //是否已经开放注册功能
            ViewBag.openRegister = configFileService.WebConfig.SecurityConfig.OpenRegister;
            // 已登录了, 那么得到所有信息
            var notebooks=notebookService.GetNotebooks(userId);
            
           
           //获得共享的笔记


            // 还需要按时间排序(DESC)得到notes
            List<Note> notes=new List<Note>();
            NoteContent noteContent=null;

            if (!notebooks.IsNullOrNothing())
            {
                // noteId是否存在
		        // 是否传入了正确的noteId
                var   hasRightNoteId = false;

                long? noteId=noteIdHex.ToLongByHex();
                if (noteIdHex!=null)
                {
                    //说明ID本身是有效的
                    var note=noteService.GetNoteById(noteId);
                    if (note!=null)
                    {
                        var noteOwner=note.UserId;
                        noteContent=noteContentService.GetNoteContent(noteId,noteOwner,false);
                       
                        hasRightNoteId=true;
                        ViewBag.curNoteId=noteId;
                        ViewBag.curNotebookId=note.NotebookId.ToHex24();

                        // 打开的是共享的笔记, 那么判断是否是共享给我的默认笔记
                        if (noteOwner!=GetUserIdBySession())
                        {
                            if (shareService.HasReadPerm(noteOwner,GetUserIdBySession(),noteId))
                            {
                                ViewBag.curSharedNoteNotebookId=note.NotebookId.ToHex24();
                                ViewBag.curSharedUserId=noteOwner;
                            }
                            else
                            {
                                hasRightNoteId = false;
                            }

                        }
                        else
                        {
                            
                             notes=  noteService.ListNotes(this.GetUserIdBySession(),note.NotebookId,false,GetPage(),50, defaultSortField, false,false);
                            // 如果指定了某笔记, 则该笔记放在首位
         
                             notes.Insert(0, note);

                        }



                    }
                    //获得最近的笔记
                    int count2 = 0;
                    var latestNotes = noteService.ListNotes(userId, null, false, GetPage(), 50, defaultSortField, false, false);


                }
                // 没有传入笔记
                // 那么得到最新笔记
                if (!hasRightNoteId)
                {
                    notes=noteService.ListNotes(userId,null,false,GetPage(),50,defaultSortField,false,false);
                    if (notes.Any())
                    {
                        noteContent=noteContentService.GetValidNoteContent(notes[0].NoteId,userId);
                        ViewBag.curNoteId=notes[0].NoteId;

                    }

                }

            }

            ViewBag.isAdmin=user.Username.Equals(config.SecurityConfig.AdminUsername);
            ViewBag.IsDevelopment = config.APPConfig.Dev;


            ViewBag.userInfo = userInfo;
            ViewBag.notebooks= notebooks;
            //ViewBag.shareNotebooks= shareNotebooks;


            ViewBag.notes= notes;
            ViewBag.noteContentJson= noteContent;
            ViewBag.noteContent = noteContent.Content;

            ViewBag.tags=tagService.GetTags(userId);
            ViewBag.config=config;

            Dictionary<string, string> js = new Dictionary<string, string>();
         

            string json = JsonSerializer.Serialize(noteBoooks, MyJsonConvert.GetOptions());
            //json  = System.IO.File.ReadAllText(@"E:\Project\JSON\notebook\GetNotebooks.json");
            //js.Add("notebooks", json);
            //ViewBag.notebooks = JsonSerializer.Serialize(notebooks, MyJsonConvert.GetOptions());
            SetLocale();
            ViewBag.js = js;
           
            //页面的值
            ViewBag.isAdmin=configFileService.WebConfig.SecurityConfig.AdminUsername.Equals(user.Username);
            
            ViewBag.userInfo=user;
           
            
            ViewBag.OpenRegister=config.SecurityConfig.OpenRegister;

            return View();


        }

        public IActionResult GetNoteContent(string noteId)
        {
            long? noteNumber = noteId.ToLongByHex();
            long? userNumber = GetUserIdBySession();
            NoteContent noteContent = noteContentService.GetValidNoteContent(noteId.ToLongByHex(), GetUserIdBySession());
            ApiRe falseRe = new ApiRe()
            {
                Ok = false,
                Msg = "GetNoteContent_is_error"
            };
            if (noteContent == null )
            {
                return Json(falseRe, MyJsonConvert.GetOptions());

            }
            return Json(noteContent, MyJsonConvert.GetOptions());
        }

        public JsonResult ListNotes(string notebookId)
        {
            Note[] notes = noteService.ListNotes(GetUserIdBySession(), notebookId.ToLongByHex(),false,false);
            //string json = JsonSerializer.Serialize(notes, MyJsonConvert.GetOptions());
            return Json(notes, MyJsonConvert.GetOptions());
        }
        /// <summary>
        /// 设置/取消Blog; 置顶
        /// </summary>
        /// <param name="noteIds">笔记的名称</param>
        /// <param name="isBlog">是否设置成博客</param>
        /// <param name="isTop">是否置顶</param>
        /// <returns></returns>
        public JsonResult SetNote2Blog(string[] noteIds,bool isBlog,bool isTop)
        {
            foreach (var item in noteIds)
            {
                noteService.ToBlog(GetUserIdBySession(),item.ToLongByHex(),isBlog,isTop);
            }
            return Json(true);
        }
        // 这里不能用json, 要用post
        public JsonResult UpdateNoteOrContent([ModelBinder(BinderType = typeof(NoteOrContentModelBinder))] NoteOrContent noteOrContent)
        {
            var userid= GetUserIdBySession();
            var old=noteService.GetNoteById(noteOrContent.NoteId);
            // 新添加note
            if (noteOrContent.IsNew.IsValidTrue()&& old==null)
            {
                var userId = GetUserIdBySession();
                // 为共享新建?
                if (noteOrContent.FromUserId != null)
                {
                    userId=noteOrContent.FromUserId;
                }
             
                //todo:IsBlog.Value 缺陷 空指针异常
                var note =new Note()
                {
                    UserId=userId,
                    NoteId=noteOrContent.NoteId,
                    NotebookId=noteOrContent.NotebookId,
                    Title=noteOrContent.Title,
                    Src=noteOrContent.Src,
                    Tags=noteOrContent.Tags.ToTagsArray(),
                    Desc=noteOrContent.Desc,
                    ImgSrc=noteOrContent.ImgSrc,
                    IsBlog=noteOrContent.IsBlog.GetValueOrDefault(),
                    IsMarkdown =noteOrContent.IsMarkdown.GetValueOrDefault()
                };
                var noteContent=new NoteContent()
                {
                    UserId=userId,
                    IsBlog=note.IsBlog,
                    Content=noteOrContent.Content,
                    Abstract=noteOrContent.Abstract
                };
                note=noteService.AddNoteAndContentForController(note,noteContent, userid);
                return Json(note,MyJsonConvert.GetOptions());
            }
            var noteUpdate=new Note();
            var needUpdateNote=false;
            if (noteOrContent.Desc.IsValid())
            {
                needUpdateNote=true;
                noteUpdate.Desc=noteOrContent.Desc;
            }
            if (noteOrContent.ImgSrc.IsValid())
            {
                needUpdateNote = true;
                noteUpdate.ImgSrc = noteOrContent.ImgSrc;
            }
            if (noteOrContent.Tags.IsValid())
            {
                needUpdateNote = true;
                noteUpdate.Tags = noteOrContent.Tags.ToTagsArray();
            }
          
            // web端不控制
            if (needUpdateNote)
            {
                noteService.UpdateNote(userid, noteOrContent.NotebookId, noteUpdate, -1);
            }
            if (noteOrContent.Content.IsValid())
            {
                noteContentService.UpdateNoteContent(userid,noteOrContent.NoteId, noteOrContent.Content, noteOrContent.Abstract,needUpdateNote,-1,DateTime.Now);
            }
            return Json(true);
        }

        public JsonResult DeleteNote(string[] noteIds,bool isShared)
        {
            if (!isShared)
            {
                foreach (var item in noteIds)
                {
                    trashService.DeleteNote(item.ToLongByHex(),GetUserIdBySession());
                }
                return Json(true);
            }
      
            foreach (var item in noteIds)
            {
                trashService.DeleteSharedNote(item.ToLongByHex(),GetUserIdBySession());

            }
            return Json(true);
        }
    }
}