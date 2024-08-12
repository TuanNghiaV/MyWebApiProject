using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortenerApi.Controllers;
using UrlShortenerApi.Models;
using UrlShortenerApi.Services;
using MyWebApiProject.Models;

namespace UrlShortenerApi.Tests
{
	[TestFixture]
	public class UrlShortenerControllerTests
	{
		private Mock<UrlShortenerService> _mockService;
		private UrlShortenerController _controller;

		[SetUp]
		public void Setup()
		{
			_mockService = new Mock<UrlShortenerService>(null);
			_controller = new UrlShortenerController(_mockService.Object);
		}

		[Test]
		public async Task ShortenUrl_ValidUrl_ReturnsOkResult()
		{
			// Arrange
			var request = new UrlShortenRequest { OriginalUrl = "https://www.example.com" };
			_mockService.Setup(s => s.ShortenUrlAsync(request.OriginalUrl, null))
						.ReturnsAsync("short123");

			// Act
			var result = await _controller.ShortenUrl(request) as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			var response = result.Value as dynamic;
			Assert.IsNotNull(response);
			Assert.AreEqual("URL shortened successfully!", response.message);
			Assert.IsTrue(response.shortUrl.ToString().Contains("short123"));
		}

		[Test]
		public async Task ShortenUrl_InvalidOriginalUrl_ReturnsBadRequest()
		{
			// Arrange
			var request = new UrlShortenRequest { OriginalUrl = "invalid-url" };

			// Act
			var result = await _controller.ShortenUrl(request) as BadRequestObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

			var response = result.Value as dynamic;
			Assert.IsNotNull(response);
			Assert.AreEqual("Original URL is not a valid URL.", response.message);
		}

		[Test]
		public async Task ShortenUrl_CustomShortUrlInUse_ReturnsBadRequest()
		{
			// Arrange
			var request = new UrlShortenRequest { OriginalUrl = "https://www.example.com", CustomShortUrl = "custom123" };
			_mockService.Setup(s => s.ShortenUrlAsync(request.OriginalUrl, request.CustomShortUrl))
						.ThrowsAsync(new ArgumentException("The custom short URL is already in use. Please choose a different one."));

			// Act
			var result = await _controller.ShortenUrl(request) as BadRequestObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

			var response = result.Value as dynamic;
			Assert.IsNotNull(response);
			Assert.AreEqual("The custom short URL is already in use. Please choose a different one.", response.message);
		}

		[Test]
		public async Task RedirectToOriginalUrl_ValidShortUrl_ReturnsRedirectResult()
		{
			// Arrange
			var shortUrl = "short123";
			var originalUrl = "https://www.example.com";
			_mockService.Setup(s => s.GetOriginalUrlAsync(shortUrl))
						.ReturnsAsync(originalUrl);

			// Act
			var result = await _controller.RedirectToOriginalUrl(shortUrl) as RedirectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(originalUrl, result.Url);
		}

		[Test]
		public async Task RedirectToOriginalUrl_InvalidShortUrl_ReturnsNotFoundResult()
		{
			// Arrange
			var shortUrl = "invalid123";
			_mockService.Setup(s => s.GetOriginalUrlAsync(shortUrl))
						.ReturnsAsync((string)null);

			// Act
			var result = await _controller.RedirectToOriginalUrl(shortUrl) as NotFoundResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
		}

		[Test]
		public async Task GetAllUrls_ReturnsOkResult()
		{
			// Arrange
			var urlMappings = new List<UrlMapping>
			{
				new UrlMapping { OriginalUrl = "https://www.example1.com", ShortUrl = "short1" },
				new UrlMapping { OriginalUrl = "https://www.example2.com", ShortUrl = "short2" }
			};

			_mockService.Setup(s => s.GetAllUrlsAsync())
						.ReturnsAsync(urlMappings);

			// Act
			var result = await _controller.GetAllUrls() as OkObjectResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

			var response = result.Value as List<UrlMapping>;
			Assert.IsNotNull(response);
			Assert.AreEqual(2, response.Count);
		}

		[Test]
		public async Task DeleteUrl_ExistingShortUrl_ReturnsNoContent()
		{
			// Arrange
			var shortUrl = "short123";
			_mockService.Setup(s => s.DeleteUrlAsync(shortUrl))
						.ReturnsAsync(true);

			// Act
			var result = await _controller.DeleteUrl(shortUrl) as NoContentResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
		}

		[Test]
		public async Task DeleteUrl_NonExistingShortUrl_ReturnsNotFound()
		{
			// Arrange
			var shortUrl = "nonexistent123";
			_mockService.Setup(s => s.DeleteUrlAsync(shortUrl))
						.ReturnsAsync(false);

			// Act
			var result = await _controller.DeleteUrl(shortUrl) as NotFoundResult;

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
		}
	}
}
