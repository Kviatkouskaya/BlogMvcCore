using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace BlogMvcCore.TagHelpers
{
    public class GreetingTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            var time = DateTime.Now.Hour;
            var dayTime = time < 12 && time > 6 ? $"morning" :
                              (time < 18 ? "afternoon" : "evening");
            output.Content.SetContent($"Good {dayTime}!");
        }
    }
}
