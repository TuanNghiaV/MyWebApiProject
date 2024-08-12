using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrlShortenerApi.Data;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Services
{
	public class UrlShortenerService
	{
		private readonly UrlShortenerContext _context;

		public UrlShortenerService(UrlShortenerContext context)
		{
			_context = context;
		}

		public async Task<string> ShortenUrlAsync(string originalUrl, string customShortUrl = null)
		{
			if (string.IsNullOrEmpty(originalUrl) || !Uri.IsWellFormedUriString(originalUrl, UriKind.Absolute))
			{
				throw new ArgumentException("Original URL is not valid.");
			}

			string shortUrl;

			// Check if the original URL already exists
			var existingMapping = await _context.UrlMappings.Find(Builders<UrlMapping>.Filter.Eq("OriginalUrl", originalUrl)).FirstOrDefaultAsync();

			if (existingMapping != null)
			{
				// Check if the provided custom short URL matches the existing one
				if (customShortUrl != null && existingMapping.ShortUrl.Equals(customShortUrl, StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException("This link already exists.");
				}

				// Remove the old mapping
				await _context.UrlMappings.DeleteOneAsync(Builders<UrlMapping>.Filter.Eq("OriginalUrl", originalUrl));
			}

			if (!string.IsNullOrEmpty(customShortUrl))
			{
				shortUrl = customShortUrl;

				// Check if the custom short URL already exists
				var existingCustomMapping = await _context.UrlMappings.Find(Builders<UrlMapping>.Filter.Eq("ShortUrl", shortUrl)).FirstOrDefaultAsync();
				if (existingCustomMapping != null)
				{
					throw new ArgumentException("The custom short URL is already in use. Please choose a different one.");
				}
			}
			else
			{
				// Generate a random short URL if no custom short URL is provided
				do
				{
					shortUrl = Guid.NewGuid().ToString().Substring(0, 6);
				}
				while (await _context.UrlMappings.Find(Builders<UrlMapping>.Filter.Eq("ShortUrl", shortUrl)).AnyAsync());
			}

			var urlMapping = new UrlMapping
			{
				ShortUrl = shortUrl,
				OriginalUrl = originalUrl
			};

			await _context.UrlMappings.InsertOneAsync(urlMapping);

			return shortUrl;
		}

		public async Task<string> GetOriginalUrlAsync(string shortUrl)
		{
			var filter = Builders<UrlMapping>.Filter.Eq("ShortUrl", shortUrl);
			var urlMapping = await _context.UrlMappings.Find(filter).FirstOrDefaultAsync();

			return urlMapping?.OriginalUrl;
		}

		public async Task<List<UrlMapping>> GetAllUrlsAsync()
		{
			return await _context.UrlMappings.Find(_ => true).ToListAsync();
		}

		public async Task<bool> DeleteUrlAsync(string shortUrl)
		{
			var filter = Builders<UrlMapping>.Filter.Eq("ShortUrl", shortUrl);
			var result = await _context.UrlMappings.DeleteOneAsync(filter);
			return result.DeletedCount > 0;
		}
	}
}
