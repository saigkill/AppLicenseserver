// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTest.cs" company="marcos software">
// This file may not be redistributed in whole or significant part
// and is subject to the marcos software license.
//
// @author: Sascha Manns, s.manns@marcossoftware.com
// @copyright: 2022, marcos-software, http://www.marcos-software.de
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable SA1027 // TabsMustNotBeUsed
#pragma warning disable SA1200 // UsingDirectivesMuseBePlacedWithinNamespace
#pragma warning disable SA1649 // FileNameShouldMatchFirstName
#pragma warning disable xUnit1013 // Public method should be marked as test

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AppLicenseserver.Domain;
using IdentityModel.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using static JWT.Controllers.TokenController;

/// <summary>
/// Designed by AnaSoft Inc.(c) 2019-2021
/// http://www.anasoft.net/apincore
///
/// NOTE: Tests are not working with InMemory database.
///       Must update database connection in appsettings.json - "AppLicenseserverDB".
///       Initial database and tables will be created and seeded once during tests startup
///
///
/// AUTHENTICATION:
/// This test drives which authentication/authorization mechanism is used.
/// Update appsettings.json to switch between
/// "UseIdentityServer4": false = uses embeded JWT authentication
/// "UseIdentityServer4": true  =  uses IdentityServer 4
/// IMPORTANT: Before run IS4 test must build the solution and run once solution with IdentityServer as startup project
///            After you get the start page for IdentityServer4 you can stop run and run unit tests
/// </summary>

namespace AppLicenseserver.Test
{
	/// <summary>
	/// Constructor for BaseTest.
	/// </summary>
	public class BaseTest
	{
		/// <summary>
		/// The remote service.
		/// </summary>
		public static bool RemoteService = false; // true to use service deployed to remote server

		/// <summary>
		/// The user name.
		/// </summary>
		public static string UserName = "saigkill";

		/// <summary>
		/// The password encoded in User table.
		/// </summary>
		public static string Password = "EvilCorpDev";
	}


	// Build Test web site from

	/// <summary>
	/// Setup testing web host factory.
	/// </summary>
	/// <typeparam name="TEntryPoint">TEntryPoint object.</typeparam>
	public class TestingRESTApiFactory<TEntryPoint> : WebApplicationFactory<Program>
		where TEntryPoint : Program
	{
		// got an idea from https://code-maze.com/aspnet-core-integration-testing/

		/// <summary>
		/// Gives a fixture an opportunity to configure the application before it gets built.
		/// </summary>
		/// <param name="builder">The <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> for the application.</param>
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json").Build();

			// overwrite if azure db test
			if (BaseTest.RemoteService)
			{
				configuration = new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings-remote.json").Build();
			}

			builder.UseConfiguration(configuration);
		}
	}

	/// <summary>
	/// Initialize Http client for testing for all test classes.
	/// </summary>
	public class HttpClientFixture : IDisposable
	{
		/// <summary>
		/// Gets the client.
		/// </summary>
		/// <value>
		/// The client.
		/// </value>
		public HttpClient Client { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpClientFixture"/> class.
		/// </summary>
		public HttpClientFixture()
		{
			this.Client = new TestingRESTApiFactory<Program>().CreateClient();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Client.Dispose();
		}
	}

	/// <summary>
	/// HttpClientCollection.
	/// </summary>
	/// <seealso cref="Xunit.ICollectionFixture&lt;AppLicenseserver.Test.HttpClientFixture&gt;" />
	[CollectionDefinition("HttpClient collection")]
	public class HttpClientCollection : ICollectionFixture<HttpClientFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}

	/// <summary>
	/// Token class.
	/// </summary>
	public class Token
	{
		public string token;
	}

	/// <summary>
	/// TokenTest class.
	/// </summary>
	/// <seealso cref="AppLicenseserver.Test.BaseTest" />
	[Collection("HttpClient collection")]
	public class TokenTest : BaseTest
	{
		static HttpClient Client;
		public HttpClientFixture fixture;

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenTest"/> class.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		public TokenTest(HttpClientFixture fixture)
		{
			this.fixture = fixture;
			Client = fixture.Client;
		}

		/// <summary>
		/// Gets or sets the token value.
		/// </summary>
		/// <value>
		/// The token value.
		/// </value>
		public static string TokenValue { get; set; }


		/// <summary>
		/// This test drives which authentication/authorization mechanism is used.
		/// Update appsettings.json to switch between
		/// "UseIdentityServer4": false = uses embeded JWT authentication
		/// "UseIdentityServer4": true  =  uses IdentityServer 4.
		/// </summary>
		/// <returns>Token.</returns>
		[Fact]
		public async Task token_test()
		{
			await token_get(null);
		}

		/// <summary>
		/// Tokens the get.
		/// </summary>
		/// <param name="client">The client.</param>
		public static async Task token_get(HttpClient client)
		{
			if (client == null)
			{
				client = Client;
			}

			if (!string.IsNullOrEmpty(TokenValue))
			{
				return;
			}

			// read from tests settings
			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json").Build();

			// JWT or IS4 authentication
			if (configuration["Authentication:UseIdentityServer4"] == "False")
			{
				// JWT
				LoginModel login = new LoginModel { Username = UserName, Password = Password };
				var response = await client.PostAsync(
					"/api/token",
					new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));
				response.EnsureSuccessStatusCode();
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				var jsonString = await response.Content.ReadAsStringAsync();
				var token = JsonConvert.DeserializeObject<Token>(jsonString);
				TokenValue = token.token;
			}
			else
			{
				// IS4
				var is4ip = configuration["Authentication:IdentityServer4IP"];
				var is4token = is4ip + "/connect/token";

				//// request the token from the Auth server for type ClientCredentials
				// var tokenClient1 = new TokenClient(discoveryClient.TokenEndpoint, "clientCred", "secret");
				// var response1 = await tokenClient1.RequestClientCredentialsAsync("AppLicenseserver");
				// var resp1 = response1.Json;


				// BAD client test
				var response = LoginUsingIdentityServer(
					is4token,
					"AppLicenseserver-BAD",
					"secret",
					"read-write",
					"my@email.com",
					"mysecretpassword123");

				var response_json = response.AccessToken;
				if (response.IsError)
				{
					Console.WriteLine(response.Error);
					Console.WriteLine(response.ErrorDescription);
				}

				Assert.True(response.IsError);
				Assert.Equal("invalid_client", response.Error);
				Assert.Equal(HttpStatusCode.BadRequest, response.HttpStatusCode);

				// BAD grant test
				response = LoginUsingIdentityServer(
					is4token,
					"AppLicenseserverClient",
					"secret",
					"read-write",
					"my@email.com",
					"mysecretpassword123-BAD");

				response_json = response.AccessToken;
				if (response.IsError)
				{
					Console.WriteLine(response.Error);
					Console.WriteLine(response.ErrorDescription);
				}

				Assert.True(response.IsError);
				Assert.Equal("invalid_grant", response.Error);
				Assert.Equal(HttpStatusCode.BadRequest, response.HttpStatusCode);

				// GOOD TEST----------------
				// use your own user list (from database) to get a token for API user
				response = LoginUsingIdentityServer(
					is4token,
					"AppLicenseserverClient",
					"secret",
					"read-write",
					"my@email.com",
					"mysecretpassword123");

				response_json = response.AccessToken;
				if (response.IsError)
				{
					Console.WriteLine(response.Error);
					Console.WriteLine(response.ErrorDescription);
				}

				Assert.False(response.IsError);
				Assert.Equal(HttpStatusCode.OK, response.HttpStatusCode);
				var jsonString = response.AccessToken;
				var token = new Token();
				token.token = jsonString;
				TokenValue = token.token;
			}
		}

		/// <summary>
		/// Logins the using identity server.
		/// </summary>
		/// <param name="is4ip">The is4ip.</param>
		/// <param name="clientId">The client identifier.</param>
		/// <param name="clientSecret">The client secret.</param>
		/// <param name="scope">The scope.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <returns>Respnse Token.</returns>
		public static TokenResponse LoginUsingIdentityServer(string is4ip, string clientId, string clientSecret, string scope, string username, string password)
		{
			var client = new HttpClient();
			PasswordTokenRequest tokenRequest = new PasswordTokenRequest()
			{
				Address = is4ip,
				ClientId = clientId,
				ClientSecret = clientSecret,
				UserName = username,
				Password = password,
				Scope = scope,
			};

			return client.RequestPasswordTokenAsync(tokenRequest).Result;
		}
	}

	// Account tests

	/// <summary>
	/// Account API Integration tests.
	/// </summary>
	[Collection("HttpClient collection")]
	public class AccountTest : BaseTest
	{
		/// <summary>
		/// The fixture.
		/// </summary>
		public HttpClientFixture fixture;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountTest"/> class.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		public AccountTest(HttpClientFixture fixture)
		{
			this.fixture = fixture;
			var client = fixture.Client;
		}

		// Account tests.

		/// <summary>
		/// Accounts the getall.
		/// </summary>
		/// <returns>Task with all accounts.</returns>
		[Fact]
		public async Task account_getall()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/account/getall");
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var accounts =
				(ICollection<AccountViewModel>)JsonConvert
					.DeserializeObject<IEnumerable<AccountViewModel>>(jsonString);
			Assert.True(accounts.Count > 0);

			// clean
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Accounts the add update delete.
		/// </summary>
		/// <returns>Task with accounts for add, update or delete.</returns>
		[Fact]
		public async Task account_add_update_delete()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			// insert
			AccountViewModel vmentity = new AccountViewModel
			{
				Name = "Account 1",
				Email = "apincore@anasoft.net",
				Description = "desc",
				IsTrial = false,
				IsActive = true,
				SetActive = DateTime.Now,
			};
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.PostAsync("/api/account/create/newaccount", new StringContent(
				JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var lastAddedId = await response.Content.ReadAsStringAsync();
			Assert.True(int.Parse(lastAddedId) > 1);
			int id = 0;
			int.TryParse(lastAddedId, out id);

			// get inserted
			var util = new Utility();
			vmentity = await util.GetAccount(client, id);

			// update test
			vmentity.Description = "desc updated";
			response = await client.PutAsync(
				"/api/account/update/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

			// confirm update
			response = await client.GetAsync("/api/account/get/byid/" + id.ToString());
			response.EnsureSuccessStatusCode();
			var jsonString = await response.Content.ReadAsStringAsync();
			var oj = JObject.Parse(jsonString);
			var desc = oj["description"].ToString();
			Assert.Equal(desc, vmentity.Description);

			// another update with same account - concurrency
			vmentity.Description = "desc updated 2";
			response = await client.PutAsync(
				"/api/account/update/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);

			// delete test
			response = await client.DeleteAsync("/api/account/delete/" + id.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		/// <summary>
		/// Accounts the getbyid.
		/// </summary>
		/// <returns>Task with information about removed account.</returns>
		[Fact]
		public async Task account_getbyid()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/account/get/byid/" + accountid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var account = JsonConvert.DeserializeObject<AccountViewModel>(jsonString);
			Assert.True(account.Name == "Account");

			// clean
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Gets active account by name.
		/// </summary>
		/// <returns>Task with removed Accounts.</returns>
		[Fact]
		public async Task account_getactivebyname()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);

			// get by id
			var response = await client.GetAsync("/api/account/get/byid/" + accountid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var account = JsonConvert.DeserializeObject<AccountViewModel>(jsonString);

			response = await client.GetAsync("/api/account/get/activebyname/" + account.Name);
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var accounts =
				(ICollection<AccountViewModel>)JsonConvert
					.DeserializeObject<IEnumerable<AccountViewModel>>(jsonString);
			Assert.True(accounts.Count > 0);

			// clean
			await util.removeAccount(client, accountid);
		}

		// Account async tests

		/// <summary>
		/// Test for getall accounts with async.
		/// </summary>
		/// <returns>Task with removed Accounts.</returns>
		[Fact]
		public async Task account_getallasync()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/accountasync/getall");
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var accounts =
				(ICollection<AccountViewModel>)JsonConvert
					.DeserializeObject<IEnumerable<AccountViewModel>>(jsonString);
			Assert.True(accounts.Count > 0);

			// clean
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Task for testing add, update and delete a account.
		/// </summary>
		/// <returns>Task with crud.</returns>
		[Fact]
		public async Task account_add_update_delete_async()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			// insert
			AccountViewModel vmentity = new AccountViewModel
			{
				Name = "Account 1",
				Email = "apincore@anasoft.net",
				Description = "desc",
				IsTrial = false,
				IsActive = true,
				SetActive = DateTime.Now,
			};
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.PostAsync("/api/accountasync/create/newaccount", new StringContent(
				JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var lastAddedId = await response.Content.ReadAsStringAsync();
			Assert.True(int.Parse(lastAddedId) > 1);
			int id = 0;
			int.TryParse(lastAddedId, out id);

			// get inserted
			var util = new Utility();
			vmentity = await util.GetAccount(client, id);

			// update test
			vmentity.Description = "desc updated";
			response = await client.PutAsync(
				"/api/accountasync/update/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

			// confirm update
			response = await client.GetAsync("/api/accountasync/get/" + id.ToString());
			response.EnsureSuccessStatusCode();
			var jsonString = await response.Content.ReadAsStringAsync();
			var oj = JObject.Parse(jsonString);
			var desc = oj["description"].ToString();
			Assert.Equal(desc, vmentity.Description);

			// another update with same account - concurrency
			vmentity.Description = "desc updated 2";
			response = await client.PutAsync(
				"/api/accountasync/update/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);

			// delete test
			response = await client.DeleteAsync("/api/accountasync/delete/" + id.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		/// <summary>
		/// Task for getting a account by id.
		/// </summary>
		/// <returns>Task with removed Account.</returns>
		[Fact]
		public async Task account_getbyidasync()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/accountasync/get/byid/" + accountid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var account = JsonConvert.DeserializeObject<AccountViewModel>(jsonString);
			Assert.True(account.Name == "Account");

			// clean
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Task for getting active accounts by name.
		/// </summary>
		/// <returns>Task with removed account.</returns>
		[Fact]
		public async Task account_getactivebynameasync()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);

			// get by id
			var response = await client.GetAsync("/api/accountasync/get/byid/" + accountid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var account = JsonConvert.DeserializeObject<AccountViewModel>(jsonString);

			response = await client.GetAsync("/api/accountasync/get/activebyname/" + account.Name);
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var accounts =
				(ICollection<AccountViewModel>)JsonConvert
					.DeserializeObject<IEnumerable<AccountViewModel>>(jsonString);
			Assert.True(accounts.Count > 0);

			// clean
			await util.removeAccount(client, accountid);
		}
	}

	// User tests

	/// <summary>
	/// Collection of Users tests.
	/// </summary>
	[Collection("HttpClient collection")]
	public class UserTest : BaseTest
	{
		/// <summary>
		/// The fixture for HttpClient.
		/// </summary>
		public HttpClientFixture fixture;

		/// <summary>
		/// Initializes a new instance of the <see cref="UserTest"/> class.
		/// </summary>
		/// <param name="fixture">HttpClientFixture.</param>
		public UserTest(HttpClientFixture fixture)
		{
			this.fixture = fixture;
			var client = fixture.Client;
		}

		/// <summary>
		/// Gets or sets Property what contains the last added user.
		/// </summary>
		public static string LastAddedUser { get; set; }

		// User tests

		/// <summary>
		/// Task what getting all Users.
		/// </summary>
		/// <returns>Task with removed user and removed account.</returns>
		[Fact]
		public async Task user_getall()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/user/getall");
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var users =
				(ICollection<UserViewModel>)JsonConvert.DeserializeObject<IEnumerable<UserViewModel>>(jsonString);
			Assert.True(users.Count > 0);

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Task for crud user.
		/// </summary>
		/// <returns>Task for create, update and delete user.</returns>
		[Fact]
		public async Task user_add_update_delete()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			// insert
			UserViewModel vmentity = new UserViewModel
			{
				FirstName = "User 1",
				LastName = "LastName",
				Email = "apincore@anasoft.net",
				Description = "desc",
				IsAdminRole = true,
				IsActive = true,
				Password = " ",
				AccountId = 1,
			};

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.PostAsync("/api/user/create/newuser", new StringContent(
				JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var lastAddedId = await response.Content.ReadAsStringAsync();
			Assert.True(int.Parse(lastAddedId) > 1);
			int id = 0;
			int.TryParse(lastAddedId, out id);

			// get inserted
			var util = new Utility();
			vmentity = await util.GetUser(client, id);

			// update test
			vmentity.Description = "desc updated";
			response = await client.PutAsync(
				"/api/user/update/byid/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

			// confirm update
			response = await client.GetAsync("/api/user/get/byid/" + id.ToString());
			response.EnsureSuccessStatusCode();
			var jsonString = await response.Content.ReadAsStringAsync();
			var oj = JObject.Parse(jsonString);
			var desc = oj["description"].ToString();
			Assert.Equal(desc, vmentity.Description);

			// another update with same user - concurrency
			vmentity.Description = "desc updated 2";
			response = await client.PutAsync(
				"/api/user/update/byid/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);

			// delete test
			response = await client.DeleteAsync("/api/user/delete/" + id.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		/// <summary>
		/// Task for getting a user by id.
		/// </summary>
		/// <returns>Task for getting a user by its id.</returns>
		[Fact]
		public async Task user_getbyid()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/user/get/byid/" + userid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);
			Assert.True(user.FirstName == "FirstName");

			// lazy-loading test
			response = await client.GetAsync("/api/account/get/byid/" + accountid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var account = JsonConvert.DeserializeObject<AccountViewModel>(jsonString);
			Assert.True(account.Users.Count == 1);

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Task for getting a active user by lastname.
		/// </summary>
		/// <returns>Task who gets a active user by lastname.</returns>
		[Fact]
		public async Task user_getactivebylastname()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);

			// get by id
			var response = await client.GetAsync("/api/user/get/byid/" + userid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);

			response = await client.GetAsync("/api/user/getactive/bylastname/" + user.LastName);
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var users =
				(ICollection<UserViewModel>)JsonConvert.DeserializeObject<IEnumerable<UserViewModel>>(jsonString);
			Assert.True(users.Count > 0);

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Getting a user by its name via stored procedure.
		/// </summary>
		/// <returns>Task for getting a user by its name.</returns>
		[Fact]
		public async Task user_getbyname_sp()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);

			// get by id
			var response = await client.GetAsync("/api/user/get/byid/" + userid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);

			response = await client.GetAsync("/api/user/get/byname/" + user.FirstName + "/" + user.LastName);
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var users =
				(ICollection<UserViewModel>)JsonConvert.DeserializeObject<IEnumerable<UserViewModel>>(jsonString);
			Assert.True(users.Count > 0);

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		//[Fact]
		//public async Task user_updateemailbyusername_sp()
		//{
		//    var client = fixture.Client;
		//    if (String.IsNullOrEmpty(TokenTest.TokenValue)) await TokenTest.token_get(client);
		//    //
		//    var util = new Utility();
		//    var accountid = await util.addAccount(client);
		//    var userid = await util.addUser(client, accountid);
		//    //
		//    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
		//    //get by id           
		//    var response = await client.GetAsync("/api/user/get/" + userid.ToString());
		//    response.EnsureSuccessStatusCode();
		//    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//    var jsonString = await response.Content.ReadAsStringAsync();
		//    var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);
		//    //
		//    string updatedEmail = "newemail@com.com";
		//    response = await client.GetAsync("/api/user/update/" + user.UserName + "/" + updatedEmail);
		//    response.EnsureSuccessStatusCode();
		//    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//    var records = await response.Content.ReadAsStringAsync();
		//    Assert.True(records != "0");
		//    //get by id to confirm update           
		//    response = await client.GetAsync("/api/user/get/" + userid.ToString());
		//    response.EnsureSuccessStatusCode();
		//    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//    jsonString = await response.Content.ReadAsStringAsync();
		//    user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);
		//    Assert.True(user.Email == updatedEmail);
		//    //clean
		//    await util.removeUser(client, userid);
		//    await util.removeAccount(client, accountid);
		//}

		//[Fact]
		//public async Task user_check_deleted()
		//{
		//    var client = fixture.Client;
		//    if (String.IsNullOrEmpty(TokenTest.TokenValue)) await TokenTest.token_get(client);
		//    //
		//    var util = new Utility();
		//    var accountid = await util.addAccount(client);
		//    var userid = await util.addUser(client, accountid);
		//    //
		//    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
		//}

		// User async tests

		/// <summary>
		/// Get all users async.
		/// </summary>
		/// <returns>Task for getting all users async.</returns>
		[Fact]
		public async Task user_getallasync()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/userasync/getall");
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var users =
				(ICollection<UserViewModel>)JsonConvert.DeserializeObject<IEnumerable<UserViewModel>>(jsonString);
			Assert.True(users.Count > 0);

			// lazy-loading test
			response = await client.GetAsync("/api/accountasync/get/byid/" + accountid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var account = JsonConvert.DeserializeObject<AccountViewModel>(jsonString);
			Assert.True(account.Users.Count == 1);

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Add, update and delete a user async.
		/// </summary>
		/// <returns>Task for crud users.</returns>
		[Fact]
		public async Task user_add_update_delete_async()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			// insert
			UserViewModel vmentity = new UserViewModel
			{
				FirstName = "User 1",
				LastName = "LastName",
				Email = "apincore@anasoft.net",
				Description = "desc",
				IsAdminRole = true,
				IsActive = true,
				Password = " ",
				AccountId = 1,
			};

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.PostAsync("/api/userasync/create/newuser", new StringContent(
				JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var lastAddedId = await response.Content.ReadAsStringAsync();
			Assert.True(int.Parse(lastAddedId) > 1);
			int id = 0;
			int.TryParse(lastAddedId, out id);

			// get inserted
			var util = new Utility();
			vmentity = await util.GetUser(client, id);

			// update test
			vmentity.Description = "desc updated";
			response = await client.PutAsync(
				"/api/userasync/update/byid/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

			// confirm update
			response = await client.GetAsync("/api/userasync/get/byid/" + id.ToString());
			response.EnsureSuccessStatusCode();
			var jsonString = await response.Content.ReadAsStringAsync();
			var oj = JObject.Parse(jsonString);
			var desc = oj["description"].ToString();
			Assert.Equal(desc, vmentity.Description);

			// another update with same user - concurrency
			vmentity.Description = "desc updated 2";
			response = await client.PutAsync(
				"/api/userasync/update/byid/" + id.ToString(),
				new StringContent(JsonConvert.SerializeObject(vmentity), Encoding.UTF8, "application/json"));
			Assert.Equal(HttpStatusCode.PreconditionFailed, response.StatusCode);

			// delete test
			response = await client.DeleteAsync("/api/userasync/delete/" + id.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
		}

		/// <summary>
		/// Get a user by its id.
		/// </summary>
		/// <returns>Task for getting a user by its id.</returns>
		[Fact]
		public async Task user_getbyidasync()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/userasync/get/byid/" + userid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);
			Assert.True(user.FirstName == "FirstName");

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Getting a active user by its lastname.
		/// </summary>
		/// <returns>Task for getting a active user by its lastname.</returns>
		[Fact]
		public async Task user_getactivebylastnameasync()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);

			// get by id
			var response = await client.GetAsync("/api/userasync/get/byid/" + userid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);

			response = await client.GetAsync("/api/userasync/getactive/bylastname/" + user.LastName);
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var users =
				(ICollection<UserViewModel>)JsonConvert.DeserializeObject<IEnumerable<UserViewModel>>(jsonString);
			Assert.True(users.Count > 0);

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		/// <summary>
		/// Getting a user by its name.
		/// </summary>
		/// <returns>Task for get a user by its name.</returns>
		[Fact]
		public async Task user_getbynameasync_sp()
		{
			var client = this.fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var util = new Utility();
			var accountid = await util.addAccount(client);
			var userid = await util.addUser(client, accountid);

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);

			// get by id
			var response = await client.GetAsync("/api/userasync/get/byid/" + userid.ToString());
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);

			response = await client.GetAsync("/api/userasync/get/byname/" + user.FirstName + "/" + user.LastName);
			response.EnsureSuccessStatusCode();
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			jsonString = await response.Content.ReadAsStringAsync();
			var users =
				(ICollection<UserViewModel>)JsonConvert.DeserializeObject<IEnumerable<UserViewModel>>(jsonString);
			Assert.True(users.Count > 0);

			// clean
			await util.removeUser(client, userid);
			await util.removeAccount(client, accountid);
		}

		//[Fact]
		//public async Task user_updateemailbyusernameasync_sp()
		//{
		//    var client = fixture.Client;
		//    if (String.IsNullOrEmpty(TokenTest.TokenValue)) await TokenTest.token_get(client);
		//    //
		//    var util = new Utility();
		//    var accountid = await util.addAccount(client);
		//    var userid = await util.addUser(client, accountid);
		//    //
		//    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
		//    //get by id           
		//    var response = await client.GetAsync("/api/userasync/get/" + userid.ToString());
		//    response.EnsureSuccessStatusCode();
		//    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//    var jsonString = await response.Content.ReadAsStringAsync();
		//    var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);
		//    //
		//    string updatedEmail = "newemail@com.com";
		//    response = await client.PutAsync("/api/userasync/update/" + user.UserName + "/" + updatedEmail);
		//    response.EnsureSuccessStatusCode();
		//    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//    var records = await response.Content.ReadAsStringAsync();
		//    Assert.True(records != "0");
		//    //get by id to confirm update           
		//    response = await client.GetAsync("/api/userasync/get/" + userid.ToString());
		//    response.EnsureSuccessStatusCode();
		//    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
		//    jsonString = await response.Content.ReadAsStringAsync();
		//    user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);
		//    Assert.True(user.Email == updatedEmail);
		//    //clean
		//    await util.removeUser(client, userid);
		//    await util.removeAccount(client, accountid);
		//}



	}


	// Shared test

	/// <summary>
	/// Utility class for some shared informatio.
	/// </summary>
	public class Utility
	{
		/// <summary>
		/// Adding a account.
		/// </summary>
		/// <param name="client">The predefined HttpClient.</param>
		/// <returns>Count of last added accounts.</returns>
		public async Task<int> addAccount(HttpClient client)
		{
			AccountViewModel account = new AccountViewModel
			{
				Name = "Account",
				Email = "apincore@anasoft.net",
				Description = "desc" + new Random().Next().ToString(),
				IsTrial = false,
				IsActive = true,
				SetActive = DateTime.Now,
			};

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.PostAsync("/api/accountasync/create/newaccount/", new StringContent(
				JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json"));
			var jsonString = await response.Content.ReadAsStringAsync();
			int lastAdded = 0;
			int.TryParse(jsonString, out lastAdded);
			return lastAdded;
		}

		/// <summary>
		/// Gets a account by id.
		/// </summary>
		/// <param name="client">The predefined HttpClient.</param>
		/// <param name="id">The id of that account, what to get.</param>
		/// <returns>A AccountViewModel what contains the account by given id.</returns>
		public async Task<AccountViewModel> GetAccount(HttpClient client, int id)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/accountasync/get/byid/" + id.ToString());
			response.EnsureSuccessStatusCode();
			var jsonString = await response.Content.ReadAsStringAsync();
			var account = JsonConvert.DeserializeObject<AccountViewModel>(jsonString);
			return account;
		}

		/// <summary>
		/// Delete a Account by id.
		/// </summary>
		/// <param name="client">The predefined HttpClient.</param>
		/// <param name="id">The id of that account, what to remove.</param>
		/// <returns>Task for removing account by id.</returns>
		public async Task removeAccount(HttpClient client, int id)
		{
			await client.DeleteAsync("/api/account/delete/" + id.ToString());
		}

		/// <summary>
		/// Add user with Account id.
		/// </summary>
		/// <param name="client">The predefined HttpClient.</param>
		/// <param name="accountId">The Account id for that user.</param>
		/// <returns>Count of last added Users.</returns>
		public async Task<int> addUser(HttpClient client, int accountId)
		{
			UserViewModel user = new UserViewModel
			{
				FirstName = "FirstName",
				LastName = "LastName",
				UserName = "username",
				Email = "email",
				Description = "desc" + new Random().Next().ToString(),
				IsAdminRole = true,
				IsActive = true,
				Password = " ",
				AccountId = accountId,
			};

			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.PostAsync("/api/userasync/create/newuser", new StringContent(
				JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));
			var jsonString = await response.Content.ReadAsStringAsync();
			int lastAdded = 0;
			int.TryParse(jsonString, out lastAdded);
			return lastAdded;
		}

		/// <summary>
		/// Get user by id.
		/// </summary>
		/// <param name="client">The predifined HttpClient.</param>
		/// <param name="id">The User id.</param>
		/// <returns>A UserViewModel with chosen user by id.</returns>
		public async Task<UserViewModel> GetUser(HttpClient client, int id)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenTest.TokenValue);
			var response = await client.GetAsync("/api/userasync/get/byid/" + id.ToString());
			response.EnsureSuccessStatusCode();
			var jsonString = await response.Content.ReadAsStringAsync();
			var user = JsonConvert.DeserializeObject<UserViewModel>(jsonString);
			return user;
		}

		/// <summary>
		/// Remove User by its id.
		/// </summary>
		/// <param name="client">The predefined HttpClient.</param>
		/// <param name="id">The id of user to remove.</param>
		/// <returns>Task for removing a user.</returns>
		public async Task removeUser(HttpClient client, int id)
		{
			await client.DeleteAsync("/api/user/delete/" + id.ToString());
		}
	}


	// async Load tests

	/// <summary>
	/// Load Test Class.
	/// </summary>
	[Collection("HttpClient collection")]
	public class ZLoadTest : BaseTest
	{
		/// <summary>
		/// Fixture of HttpClient.
		/// </summary>
		public HttpClientFixture fixture;

		/// <summary>
		/// Initializes a new instance of the <see cref="ZLoadTest"/> class.
		/// </summary>
		/// <param name="fixture">Given HttpClientFixture.</param>
		public ZLoadTest(HttpClientFixture fixture)
		{
			this.fixture = fixture;
			var client = fixture.Client;
		}


		/// <summary>
		/// Load test
		/// --local service: BaseTest.RemoteService = false
		/// --remote service: BaseTest.RemoteService = true.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task LoadTest()
		{
			int loopmax = 10;
			var client = fixture.Client;
			if (string.IsNullOrEmpty(TokenTest.TokenValue))
			{
				await TokenTest.token_get(client);
			}

			var accountId = 0;
			var userId = 0;
			var util = new Utility();
			int i = 1;
			while (i < loopmax)
			{
				accountId = await util.addAccount(client);
				userId = await util.addUser(client, accountId);
				await util.GetAccount(client, accountId);
				await util.GetUser(client, userId);
				await util.removeUser(client, userId);
				await util.removeAccount(client, accountId);
				i++;
			}

			Assert.True(i == loopmax);
		}

		/// <summary>
		///  
		/// DDoSAttack prevention test 
		/// /api/info flagged for DDoS test with API.controller attribute  
		/// </summary>
		/// <returns></returns>
		//[Fact]
		//public async Task DDoSAttack_Test()
		//{
		//    int attackCount = 0;
		//    var client = fixture.Client;

		//    var configuration = new ConfigurationBuilder()
		//    .SetBasePath(Directory.GetCurrentDirectory())
		//    .AddJsonFile("appsettings.json").Build();


		//    if (configuration["IntegrationTests"] == "False" || configuration["DDosAttackProtection:Enabled"] == "False")  //skip
		//    {
		//        Assert.True(true, "Skip it when the configuration settings turn off DDoS tests");
		//        return;
		//    }

		//    ////read from tests settings
		//    //var configuration = new ConfigurationBuilder()
		//    //    .SetBasePath(Directory.GetCurrentDirectory())
		//    //    .AddJsonFile("appsettings.json").Build();
		//    //string maxHitsPerOrigin = configuration["DDosAttackProtection:MaxHitsPerOrigin"];
		//    //string maxHitsPerOriginIntervalMs = configuration["DDosAttackProtection:MaxHitsPerOriginIntervalMs"];
		//    //string releaseIntervalMs = configuration["DDosAttackProtection:ReleaseIntervalMs"];

		//    //SetAppSettingValue("DDosAttackProtection:MaxHitsPerOrigin", "50");
		//    //SetAppSettingValue("DDosAttackProtection:MaxHitsPerOriginIntervalMs", "1000");
		//    //SetAppSettingValue("DDosAttackProtection:ReleaseIntervalMs", "1000");

		//    //LoginModel login = new LoginModel { Username = UserName, Password = Password };
		//    //StringContent login_string = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

		//    HttpResponseMessage response = null;
		//    attackCount = 5000;
		//    int i = 1;
		//    while (i < attackCount)
		//    {
		//        //response = await client.PostAsync("/api/token", login_string);
		//        response = await client.GetAsync("/api/info");
		//        i++;
		//    }
		//    //forbidden
		//    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

		//    //blocked ip should be expired after 2s
		//    System.Threading.Thread.Sleep(2000);
		//    attackCount = 10;
		//    i = 1;
		//    while (i < attackCount)
		//    {
		//        //response = await client.PostAsync("/api/token", login_string);
		//        response = await client.GetAsync("/api/info");
		//        i++;
		//    }
		//    //forbidden
		//    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		//    ////set back config settings
		//    //SetAppSettingValue("DDosAttackProtection:MaxHitsPerOrigin", maxHitsPerOrigin);
		//    //SetAppSettingValue("DDosAttackProtection:MaxHitsPerOriginIntervalMs", maxHitsPerOriginIntervalMs);
		//    //SetAppSettingValue("DDosAttackProtection:ReleaseIntervalMs", releaseIntervalMs);

		//}


		//update json file
		//public static void SetAppSettingValue(string key, string value, string appSettingsJsonFilePath = null)
		//{
		//    if (appSettingsJsonFilePath == null)
		//    {
		//        appSettingsJsonFilePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "appsettings.json");
		//    }

		//    var json = System.IO.File.ReadAllText(appSettingsJsonFilePath);
		//    dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

		//    jsonObj[key] = value;

		//    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

		//    System.IO.File.WriteAllText(appSettingsJsonFilePath, output);
		//}

		//}
	}
}