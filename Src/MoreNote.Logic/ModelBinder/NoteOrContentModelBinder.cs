﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using MoreNote.Common.ExtensionMethods;
using MoreNote.Logic.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MoreNote.Common.ModelBinder
{
    public class NoteOrContentModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            NoteOrContent noteOrContent = new NoteOrContent()
            {
                NotebookId = bindingContext.ValueProvider.GetValue("NotebookId").FirstOrDefault().ToLongByHex(),
                NoteId = bindingContext.ValueProvider.GetValue("NoteId").FirstOrDefault().ToLongByHex(),
                UserId = bindingContext.ValueProvider.GetValue("UserId").FirstOrDefault().ToLongByHex(),
                Title = bindingContext.ValueProvider.GetValue("Title").FirstOrDefault(),
                Desc = bindingContext.ValueProvider.GetValue("Desc").FirstOrDefault(),
                Src = bindingContext.ValueProvider.GetValue("Src").FirstOrDefault(),
                ImgSrc = bindingContext.ValueProvider.GetValue("ImgSrc").FirstOrDefault(),
                Tags = bindingContext.ValueProvider.GetValue("Tags").FirstOrDefault(),
                Content = bindingContext.ValueProvider.GetValue("Content").FirstOrDefault(),
                Abstract = bindingContext.ValueProvider.GetValue("Abstract").FirstOrDefault(),
                IsNew = bindingContext.ValueProvider.GetValue("IsNew").FirstOrDefault().ToBool(),
                IsMarkdown = bindingContext.ValueProvider.GetValue("IsMarkdown").FirstOrDefault().ToBool(),
                FromUserId = bindingContext.ValueProvider.GetValue("FromUserId").FirstOrDefault().ToLongByHex(),
                IsBlog = bindingContext.ValueProvider.GetValue("IsBlog").FirstOrDefault().ToBool()
            };
            var IsNew= bindingContext.ValueProvider.GetValue("IsNew").FirstOrDefault();

            bindingContext.Result = ModelBindingResult.Success(noteOrContent);

            return Task.CompletedTask;
        }
    }
}
