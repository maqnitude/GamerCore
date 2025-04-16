using GamerCore.CustomerSite.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GamerCore.CustomerSite.TagHelpers
{
    [HtmlTargetElement("pagination")]
    public class PaginationTagHelper : TagHelper
    {
        public PaginationMetadata Pagination { get; set; } = null!;
        public string PageUrl { get; set; } = null!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.Attributes.SetAttribute("aria-label", "Page navigation");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            // Previous page link
            if (Pagination.Page > 1)
            {
                ul.InnerHtml.AppendHtml(CreatePageLink(Pagination.Page - 1, "Previous"));
            }

            // Page numbners
            for (int i = 1; i <= Pagination.TotalPages; i++)
            {
                ul.InnerHtml.AppendHtml(CreatePageLink(i, i.ToString(), i == Pagination.Page));
            }

            // Next page link
            if (Pagination.Page < Pagination.TotalPages)
            {
                ul.InnerHtml.AppendHtml(CreatePageLink(Pagination.Page + 1, "Next"));
            }

            output.Content.AppendHtml(ul);
        }

        private TagBuilder CreatePageLink(int page, string text, bool isActive = false)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");

            if (isActive)
            {
                li.AddCssClass("active");
            }

            var a = new TagBuilder("a");
            a.AddCssClass("page-link");
            a.Attributes["href"] = $"{PageUrl}?page={page}";
            a.InnerHtml.Append(text);

            li.InnerHtml.AppendHtml(a);

            return li;
        }
    }
}