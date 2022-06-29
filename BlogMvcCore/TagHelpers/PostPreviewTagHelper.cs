using BlogMvcCore.DomainModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Text;

namespace BlogMvcCore.TagHelpers
{
    public class PostPreviewTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        public List<PostDomain> PostList { get; set; }
        private const int maxPostLength = 500;
        public PostPreviewTagHelper(IHtmlGenerator htmlGenerator) => _htmlGenerator = htmlGenerator;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "postpreview";
            output.TagMode = TagMode.StartTagAndEndTag;
            var htmlBuilder = new HtmlContentBuilder();
            foreach (var item in PostList)
            {
                htmlBuilder.AppendHtml($"{item.Date.Day}.{item.Date.Month}.{item.Date.Year}</br>");
                htmlBuilder.AppendHtml($"{item.Author.FirstName} {item.Author.SecondName} </br>");
                htmlBuilder.AppendHtml($"{item.Title}<br>");
                if (item.Text.Length > maxPostLength)
                {
                    var postText = item.Text.Substring(0, maxPostLength);
                    htmlBuilder.AppendHtml($"{postText}<br>");
                }
                else
                {
                    htmlBuilder.AppendHtml($"{item.Text}<br>");
                }
                htmlBuilder.AppendHtml(GenerateViewMoreLink(item.ID));
                htmlBuilder.AppendHtml("<hr class=hr1>");
            }
            output.Content.SetHtmlContent(htmlBuilder);
        }
        private TagBuilder GenerateViewMoreLink(long postID)
        {
            var actionLink = _htmlGenerator.GenerateActionLink(
                ViewContext,
                linkText: "view more",
                actionName: "ViewPostAndComments",
                controllerName: "Post",
                fragment: null,
                hostname: null,
                htmlAttributes: null,
                protocol: null,
                routeValues: new { postID }
                );
            return actionLink;
        }
    }
}
