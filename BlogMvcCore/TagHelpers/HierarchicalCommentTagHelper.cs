using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using BlogMvcCore.DomainModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BlogMvcCore.TagHelpers
{
    public class HierarchicalCommentTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        public List<CommentWithLevel> CommentList { get; set; }

        public HierarchicalCommentTagHelper(IHtmlGenerator htmlGenerator) => _htmlGenerator = htmlGenerator;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "hierarchicalcomment";
            output.TagMode = TagMode.StartTagAndEndTag;
            var htmlBuilder = new HtmlContentBuilder();
            foreach (var item in CommentList)
            {
                htmlBuilder.AppendHtmlLine($@"<div style=""margin - left: @(({item.Level} + 1) * 20)px"">");

                var deleteLink = _htmlGenerator.GenerateActionLink(ViewContext, "Delete", "DeleteComment", "Comment", null, null, null,
                                                                   new { postID = item.Comment.Post.ID, commentID = item.Comment.ID }, null);

                htmlBuilder.AppendHtml(deleteLink);
                htmlBuilder.AppendHtml(" ");

                var editLink = _htmlGenerator.GenerateActionLink(ViewContext, "Edit", "EditComment", "Comment", null, null, null, null,
                                                                   new { commentID = item.Comment.ID, text = item.Comment.Text, postID = item.Comment.Post.ID });
                htmlBuilder.AppendHtml(editLink);
                htmlBuilder.AppendHtml("<br>");

                htmlBuilder.AppendHtml($"{item.Comment.Author} {item.Comment.Date.Day}/{item.Comment.Date.Month}/{item.Comment.Date.Year}</br>");
                htmlBuilder.AppendHtml($"{item.Comment.Text}</br>");

                htmlBuilder.AppendHtml(GenerateAddCommentForm(item.Comment.Post.ID, item.Comment.ID));
                htmlBuilder.AppendHtml("<br>");
                htmlBuilder.AppendHtml(@"<hr class=""hr1"">");
                htmlBuilder.AppendHtmlLine("</div>");
            }
            output.Content.SetHtmlContent(htmlBuilder);
        }

        private TagBuilder GenerateAddCommentForm(long postID, long commentID)
        {
            var addCommentForm = _htmlGenerator.GenerateForm(
                ViewContext,
                actionName: "AddComment",
                controllerName: "Comment",
                fragment: null,
                routeValues: new { HtmlString.NewLine, postID, commentID },
                method: "post",
                htmlAttributes: null);
            return addCommentForm;
        }
    }
}
