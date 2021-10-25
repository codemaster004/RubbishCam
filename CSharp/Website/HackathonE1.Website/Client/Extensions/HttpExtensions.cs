using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HackathonE1.Website.Client.Extensions
{
	public static class HttpExtensions
	{
		public static Task<HttpResponseMessage> GetAsync( this HttpClient http, string requestUri, BrowserRequestCredentials credentials )
		{
			return http.GetAsync( ToUri( requestUri ), credentials );
		}
		public static async Task<HttpResponseMessage> GetAsync( this HttpClient http, Uri requestUri, BrowserRequestCredentials credentials )
		{
			using HttpRequestMessage msg = new( HttpMethod.Get, requestUri );
			_ = msg.SetBrowserRequestCredentials( credentials );

			return await http.SendAsync( msg );
		}

		public static Task<HttpResponseMessage> DeleteAsync( this HttpClient http, string requestUri, BrowserRequestCredentials credentials )
		{
			return http.DeleteAsync( ToUri( requestUri ), credentials );
		}
		public static async Task<HttpResponseMessage> DeleteAsync( this HttpClient http, Uri requestUri, BrowserRequestCredentials credentials )
		{
			using HttpRequestMessage msg = new( HttpMethod.Delete, requestUri );
			_ = msg.SetBrowserRequestCredentials( credentials );

			return await http.SendAsync( msg );
		}

		public static Task<HttpResponseMessage> PostAsync( this HttpClient http, string requestUri, HttpContent content, BrowserRequestCredentials credentials )
		{
			return http.PostAsync( ToUri( requestUri ), content, credentials );
		}
		public static async Task<HttpResponseMessage> PostAsync( this HttpClient http, Uri requestUri, HttpContent content, BrowserRequestCredentials credentials )
		{
			using HttpRequestMessage msg = new( HttpMethod.Post, requestUri );
			msg.Content = content;
			_ = msg.SetBrowserRequestCredentials( credentials );

			return await http.SendAsync( msg );
		}


		public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>( this HttpClient http, string requestUri, TValue value, BrowserRequestCredentials credentials )
		{
			return http.PostAsJsonAsync( ToUri( requestUri ), value, credentials );
		}
		public static async Task<HttpResponseMessage> PostAsJsonAsync<TValue>( this HttpClient http, Uri requestUri, TValue value, BrowserRequestCredentials credentials )
		{
			using HttpRequestMessage msg = new( HttpMethod.Post, requestUri );
			msg.Content = JsonContent.Create( value );
			_ = msg.SetBrowserRequestCredentials( credentials );

			return await http.SendAsync( msg );
		}

		public static Task<T> GetFromJsonAsync<T>( this HttpClient http, string requestUri, BrowserRequestCredentials credentials )
		{
			return http.GetFromJsonAsync<T>( ToUri( requestUri ), credentials );
		}
		public static async Task<T> GetFromJsonAsync<T>( this HttpClient http, Uri requestUri, BrowserRequestCredentials credentials )
		{
			var resp = await http.GetAsync( requestUri, credentials );
			return await resp.Content.ReadFromJsonAsync<T>();
		}


		private static Uri ToUri( string path )
		{
			return new Uri( path, UriKind.RelativeOrAbsolute );
		}

	}
}
