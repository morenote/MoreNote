﻿/**
 * copy from https://github.com/ldqk/Masuit.MyBlogs
 * MIT License
 * */

using Masuit.MyBlogs.Core.Models.ViewModel;
using Masuit.Tools;
using Masuit.Tools.AspNetCore.ResumeFileResults.Extensions;
using Masuit.Tools.Files;
using Masuit.Tools.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoreNote.Common.Utils;
using MoreNote.Framework.Controllers;
using MoreNote.Logic.Service;
using MoreNote.Logic.Service.Logging;

using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masuit.MyBlogs.Core.Controllers
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    /// 
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Route("Dashboard/RemoteFile/[action]")]
    public class RemoteFileController : BaseController
    {
        private AccessService accessService;
        private BlogService blogService;
        private ConfigService configService;
        private TagService tagService;
        private NotebookService notebookService;
        private NoteService noteService;

        private string PathRoot = "/";

        public RemoteFileController(AttachService attachService
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
            , IWebHostEnvironment webHostEnvironment
            ,ISevenZipCompressor sevenZipCompressor
            , BlogService blogService
            ) :
            base(attachService, tokenSerivce, noteFileService, userService, configFileService, accessor)
        {
            this.accessService = accessService;
            this.blogService = blogService;
            this.configService = configService;
            this.tagService = tagService;
            this.notebookService = notebookService;
            this.noteService = noteService;
            this.HostEnvironment = webHostEnvironment;
            this.SevenZipCompressor=sevenZipCompressor;

            if (RuntimeEnvironment.IsWindows)
            {
                PathRoot = @"C:\";
            }
        }

        public IWebHostEnvironment HostEnvironment { get; set; }

        /// <summary>
        ///
        /// </summary>
        public ISevenZipCompressor SevenZipCompressor { get; set; }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult GetFiles(string path)
        {
            var files = Directory.GetFiles(HostEnvironment.WebRootPath + path).OrderByDescending(s => s).Select(s => new
            {
                filename = Path.GetFileName(s),
                path = s
            }).ToList();
            return ResultData(files);
        }

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public ActionResult Read(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                string text = new FileInfo(filename).ShareReadWrite().ReadAllText(Encoding.UTF8);
                return ResultData(text);
            }
            return ResultData(null, false, "文件不存在！");
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public ActionResult Save(string filename, string content)
        {
            try
            {
                new FileInfo(filename).ShareReadWrite().WriteAllText(content, Encoding.UTF8);
                return ResultData(null, message: "保存成功");
            }
            catch (IOException e)
            {
                LogManager.Error(GetType(), e);
                return ResultData(null, false, "保存失败");
            }
        }

        /// <summary>
        /// 上传文件 最大512MB
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(536870912L)]
        public async Task<ActionResult> Upload(string destination)
        {
            try
            {
             
                foreach (var httpfile in Request.Form.Files)
                {
                    string path = Path.Combine(destination, httpfile.FileName);
                    await using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
                    await httpfile.CopyToAsync(fs);
                }
                return Json(new
                {
                    result = new List<object>()
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 操作文件
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Handle([FromBody] FileRequest req)
        {
            var list = new List<object>();
            var root = PathRoot;

            switch (req.Action)
            {
                case "list":
                    var path = Path.Combine(root, req.Path.TrimStart('\\', '/'));
                    var dirs = Directory.GetDirectories(path);
                    var files = Directory.GetFiles(path);
                    list.AddRange(dirs.Select(s => new DirectoryInfo(s)).Select(dirinfo => new FileList
                    {
                        date = dirinfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        name = dirinfo.Name,
                        size = 0,
                        type = "dir"
                    }).Union(files.Select(s => new FileInfo(s)).Select(info => new FileList
                    {
                        name = info.Name,
                        size = info.Length,
                        date = info.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        type = "file"
                    })));
                    break;

                case "remove":
                    req.Items.ForEach(s =>
                    {
                        s = Path.Combine(root, s.TrimStart('\\', '/'));
                        try
                        {
                            Policy.Handle<IOException>().WaitAndRetry(5, i => TimeSpan.FromSeconds(1)).Execute(() => System.IO.File.Delete(s));
                        }
                        catch
                        {
                            Policy.Handle<IOException>().WaitAndRetry(5, i => TimeSpan.FromSeconds(1)).Execute(() => Directory.Delete(s, true));
                        }
                    });
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;

                case "rename":
                case "move":
                    string newpath;
                    if (!string.IsNullOrEmpty(req.Item))
                    {
                        newpath = Path.Combine(root, req.NewItemPath?.TrimStart('\\', '/'));
                        path = Path.Combine(root, req.Item.TrimStart('\\', '/'));
                        try
                        {
                            System.IO.File.Move(path, newpath);
                        }
                        catch
                        {
                            Directory.Move(path, newpath);
                        }
                    }
                    else
                    {
                        newpath = Path.Combine(root, req.NewPath.TrimStart('\\', '/'));
                        req.Items.ForEach(s =>
                        {
                            try
                            {
                                System.IO.File.Move(Path.Combine(root, s.TrimStart('\\', '/')), Path.Combine(newpath, Path.GetFileName(s)));
                            }
                            catch
                            {
                                Directory.Move(Path.Combine(root, s.TrimStart('\\', '/')), Path.Combine(newpath, Path.GetFileName(s)));
                            }
                        });
                    }
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;

                case "copy":
                    if (!string.IsNullOrEmpty(req.Item))
                    {
                        System.IO.File.Copy(Path.Combine(root, req.Item.TrimStart('\\', '/')), Path.Combine(root, req.NewItemPath.TrimStart('\\', '/')));
                    }
                    else
                    {
                        newpath = Path.Combine(root, req.NewPath.TrimStart('\\', '/'));
                        req.Items.ForEach(s => System.IO.File.Copy(Path.Combine(root, s.TrimStart('\\', '/')), !string.IsNullOrEmpty(req.SingleFilename) ? Path.Combine(newpath, req.SingleFilename) : Path.Combine(newpath, Path.GetFileName(s))));
                    }
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;

                case "edit":
                    new FileInfo(Path.Combine(root, req.Item.TrimStart('\\', '/'))).ShareReadWrite().WriteAllText(req.Content, Encoding.UTF8);
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;

                case "getContent":
                    return Json(new
                    {
                        result = new FileInfo(Path.Combine(root, req.Item.TrimStart('\\', '/'))).ShareReadWrite().ReadAllText(Encoding.UTF8)
                    });

                case "createFolder":
                    list.Add(new
                    {
                        success = Directory.CreateDirectory(Path.Combine(root, req.NewPath.TrimStart('\\', '/'))).Exists.ToString()
                    });
                    break;

                case "changePermissions":
                    //todo:文件权限修改
                    break;

                case "compress":
                    var filename = Path.Combine(Path.Combine(root, req.Destination.TrimStart('\\', '/')), Path.GetFileNameWithoutExtension(req.CompressedFilename) + ".zip");
                    SevenZipCompressor.Zip(req.Items.Select(s => Path.Combine(root, s.TrimStart('\\', '/'))), filename);
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;

                case "extract":
                    var folder = Path.Combine(Path.Combine(root, req.Destination.TrimStart('\\', '/')), req.FolderName.Trim('/', '\\'));
                    var zip = Path.Combine(root, req.Item.TrimStart('\\', '/'));
                    SevenZipCompressor.Decompress(zip, folder);
                    list.Add(new
                    {
                        success = "true"
                    });
                    break;
            }
            return Json(new
            {
                result = list
            });
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="items"></param>
        /// <param name="toFilename"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Handle(string path, string[] items, string toFilename)
        {
            //path = path?.TrimStart('\\', '/') ?? "";
            //var root = CommonHelper.SystemSettings["PathRoot"].TrimStart('\\', '/');
            //var file = Path.Combine(HostEnvironment.ContentRootPath, PathRoot, path);
            switch (Request.Query["action"])
            {
                case "download":
                    if (System.IO.File.Exists(path))
                    {
                        return this.ResumePhysicalFile(path, Path.GetFileName(path));
                    }
                    //MEMI contentType
                    var memi = GetMemi(path);

                    return File(path, memi);

                case "downloadMultiple":
                    var buffer = SevenZipCompressor.ZipStream(items).ToArray();
                    return this.ResumeFile(buffer, Path.GetFileName(toFilename));
            }

            throw new Exception("文件未找到");
        }
    }
}