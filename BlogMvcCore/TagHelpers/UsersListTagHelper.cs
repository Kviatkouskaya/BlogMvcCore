using BlogMvcCore.DomainModel;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogMvcCore.TagHelpers
{
    public class UsersListTagHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;
        [ViewContext]
        public ViewContext ViewContext { get; set; }
        public List<UserDomain> userList { get; set; }
        public UsersListTagHelper(IHtmlGenerator htmlGenerator) => _htmlGenerator = htmlGenerator;
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "users";
            output.TagMode = TagMode.StartTagAndEndTag;
            var htmlBuilder = new HtmlContentBuilder();
            foreach (var user in userList)
            {
                var htmlLink = _htmlGenerator.GenerateActionLink(
                    ViewContext,
                    $"{user.FirstName} {user.SecondName}",
                    "VisitUserPage",
                    "User",
                    null, null, null,
                    routeValues: new { user.Login }, null);
                htmlBuilder.AppendHtml(htmlLink);
                htmlBuilder.AppendHtml("</br>");
            }
            output.Content.SetHtmlContent(htmlBuilder);
        }
    }
}
