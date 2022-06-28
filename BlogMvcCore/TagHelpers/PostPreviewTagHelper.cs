using BlogMvcCore.DomainModel;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using System;

namespace BlogMvcCore.TagHelpers
{
    public class PostPreviewTagHelper : TagHelper
    {
        public PostDomain UserPost { get; set; }
        private const int maxPostLength = 500;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "postpreview";
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"{UserPost.Date.Day}.{UserPost.Date.Month}.{UserPost.Date.Year}</br>");
            stringBuilder.Append($"{UserPost.Author.FirstName} {UserPost.Author.SecondName} </br>");
            stringBuilder.Append($"{UserPost.Title}<br>");
            if (UserPost.Text.Length > maxPostLength)
            {
                var postText = UserPost.Text.Substring(0, maxPostLength);
                stringBuilder.Append($"{postText}<br>");
            }
            else
            {
                stringBuilder.Append($"{UserPost.Text}<br>");
            }
            output.Content.SetHtmlContent(stringBuilder.ToString());
        }
    }
}
