using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UrlShortenerApi.Services;
using UrlShortenerApi.Models;
using MyWebApiProject.Models;

namespace UrlShortenerApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class UrlShortenerController : ControllerBase
	{
		private readonly UrlShortenerService _service;

		public UrlShortenerController(UrlShortenerService service)
		{
			_service = service;
		}

		[HttpPost("shorten")]
		public async Task<IActionResult> ShortenUrl([FromBody] UrlShortenRequest request)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(request.OriginalUrl))
				{
					return BadRequest(new { message = "Original URL cannot be empty." });
				}

				if (!Uri.IsWellFormedUriString(request.OriginalUrl, UriKind.Absolute))
				{
					return BadRequest(new { message = "Original URL is not a valid URL." });
				}

				var shortUrl = await _service.ShortenUrlAsync(request.OriginalUrl, request.CustomShortUrl);
				return Ok(new { message = "URL shortened successfully!", shortUrl = $"{Request.Scheme}://{Request.Host}/{shortUrl}" });
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				// Log the exception (could use a logging framework)
				Console.WriteLine($"An error occurred: {ex.Message}");
				return StatusCode(500, new { message = "An unexpected error occurred." });
			}
		}

		[HttpGet("{shortUrl}")]
		public async Task<IActionResult> RedirectToOriginalUrl(string shortUrl)
		{
			var originalUrl = await _service.GetOriginalUrlAsync(shortUrl);
			if (string.IsNullOrEmpty(originalUrl))
			{
				// Log the issue
				Console.WriteLine($"Original URL not found for short URL: {shortUrl}");
				return NotFound();
			}

			if (!Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
			{
				// Log the issue
				Console.WriteLine($"Invalid original URL: {originalUrl}");
				return BadRequest("Invalid original URL");
			}

			// Log the successful redirect
			Console.WriteLine($"Redirecting short URL: {shortUrl} to original URL: {originalUrl}");
			return RedirectPermanent(originalUrl);
		}

		[HttpGet("all")]
		public async Task<IActionResult> GetAllUrls()
		{
			var urlMappings = await _service.GetAllUrlsAsync();
			return Ok(urlMappings);
		}

		[HttpDelete("{shortUrl}")]
		public async Task<IActionResult> DeleteUrl(string shortUrl)
		{
			var success = await _service.DeleteUrlAsync(shortUrl);
			if (success)
			{
				return NoContent(); // Return 204 No Content status if deletion is successful
			}
			else
			{
				return NotFound(); // Return 404 Not Found if the short URL was not found
			}
		}
	}
}
