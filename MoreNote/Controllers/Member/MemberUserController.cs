﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreNote.Logic.Entity;
using MoreNote.Logic.Service;
using MoreNote.Value;

namespace MoreNote.Controllers.Member
{
    [Route("/member/user/{action=Username}")]
    public class MemberUserController : BaseController
    {
        public MemberUserController(AttachService attachService
            , TokenSerivce tokenSerivce
            , NoteFileService noteFileService
            , UserService userService
            , ConfigFileService configFileService
            , IHttpContextAccessor accessor) : base(attachService, tokenSerivce, noteFileService, userService, configFileService, accessor)
        {

        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Username()
        {
         
            User user = GetUserBySession();
            ViewBag.user = user;
            ViewBag.msg = LanguageResource.GetMsg();
            ViewBag.member = LanguageResource.GetMember();
            ViewBag.title = "用户名";
            return View("Views/Member/user/Username.cshtml");
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Email()
        {
         
            User user = GetUserBySession();
            ViewBag.user = user;
            ViewBag.msg = LanguageResource.GetMsg();
            ViewBag.member = LanguageResource.GetMember();
            ViewBag.title = "电子邮箱";
            return View("Views/Member/user/email.cshtml");
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Password()
        {
            User user = GetUserBySession();
            ViewBag.user = user;
            ViewBag.msg = LanguageResource.GetMsg();
            ViewBag.member = LanguageResource.GetMember();
            ViewBag.title = "密码";
            return View("Views/Member/user/password.cshtml");
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin,SuperAdmin")]
        public IActionResult Avatar()
        {
          
            User user = GetUserBySession();
            ViewBag.user = user;
            ViewBag.msg = LanguageResource.GetMsg();
            ViewBag.member = LanguageResource.GetMember();
            ViewBag.title = "头像";
            return View("Views/Member/user/avatar.cshtml");
        }
    }
}