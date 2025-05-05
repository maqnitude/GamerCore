using GamerCore.CustomerSite.Models;
using GamerCore.CustomerSite.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GamerCore.CustomerSite.Tests.TagHelpers
{
    public class PaginationTagHelperTests
    {
        [Fact]
        public void Process_CreatesProperPageLinks_WithMultiplePages()
        {
            // Arrange
            var tagHelper = new PaginationTagHelper
            {
                Pagination = new PaginationMetadata
                {
                    Page = 2,
                    PageSize = 10,
                    TotalItems = 30
                },
                PageUrl = "/Products?page=2&categoryId=1"
            };

            // "N" removes the '-' in the Guid string
            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var output = new TagHelperOutput(
                "pagination",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            tagHelper.Process(context, output);

            // Assert
            Assert.Equal("nav", output.TagName);
            Assert.True(output.Attributes.ContainsName("aria-label"));
            Assert.Equal("Page links", output.Attributes["aria-label"].Value);

            var content = output.Content.GetContent();

            Assert.Contains("<ul class=\"pagination\">", content);

            Assert.Contains("Previous", content);
            Assert.Contains(">1<", content); // Check for page 1 label
            Assert.Contains("active", content); // Check that current page is active
            Assert.Contains(">2<", content); // Check for page 2 label
            Assert.Contains(">3<", content); // Check for page 3 label
            Assert.Contains("Next", content);

            Assert.Contains("href=\"/Products?", content);
            Assert.Contains("page=1", content);
            Assert.Contains("page=2", content);
            Assert.Contains("page=3", content);
            Assert.Contains("categoryId=1", content);
            Assert.Equal(6, content.Split("page-link").Length); // There should be 5 page links (+1 because of split)
        }

        [Fact]
        public void Process_DoesNotCreatePageLink_WithOnePage()
        {
            // Arrange
            var tagHelper = new PaginationTagHelper
            {
                Pagination = new PaginationMetadata
                {
                    Page = 1,
                    PageSize = 10,
                    TotalItems = 5
                },
                PageUrl = "/Products?page=1&categoryId=1"
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var output = new TagHelperOutput(
                "pagination",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            tagHelper.Process(context, output);

            // Assert
            var content = output.Content.GetContent();
            Assert.DoesNotContain("Previous", content);
            Assert.DoesNotContain("Next", content);

            // Only page 1 is shown
            Assert.Contains("active", content);
            Assert.Equal(2, content.Split("page-link").Length);
        }

        [Fact]
        public void Process_DoesNotCreatePreviousPageLink_WhenOnFirstPage()
        {
            // Arrange
            var tagHelper = new PaginationTagHelper
            {
                Pagination = new PaginationMetadata
                {
                    Page = 1,
                    PageSize = 10,
                    TotalItems = 30
                },
                PageUrl = "/Products?page=1&categoryId=1"
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var output = new TagHelperOutput(
                "pagination",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            tagHelper.Process(context, output);

            // Assert
            var content = output.Content.GetContent();
            Assert.DoesNotContain("Previous", content);
            Assert.Contains("Next", content);
        }

        [Fact]
        public void Process_DoesNotCreateNextPageLink_WhenOnLastPage()
        {
            // Arrange
            var tagHelper = new PaginationTagHelper
            {
                Pagination = new PaginationMetadata
                {
                    Page = 3,
                    PageSize = 10,
                    TotalItems = 30
                },
                PageUrl = "/Products?page=3&categoryId=1"
            };

            var context = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var output = new TagHelperOutput(
                "pagination",
                new TagHelperAttributeList(),
                (useCachedResult, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            tagHelper.Process(context, output);

            // Assert
            var content = output.Content.GetContent();
            Assert.Contains("Previous", content);
            Assert.DoesNotContain("Next", content);
        }
    }
}