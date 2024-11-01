using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;

namespace UserIntegrationTests
{
    [TestClass]
    public class IntegrationUserTests
    {
        private readonly HttpClient _client;

        public IntegrationUserTests()
        {
            var factory = new MockFactory();
            using (factory.Services.CreateScope())
            {
                // var provider = scope.ServiceProvider;
                // using (var APIContext = provider.GetRequiredService<APIContext>())
                // {
                //     APIContext.Database.EnsureCreatedAsync();
                //
                //     APIContext.Users.AddAsync(new User { Username = "oggiVAdmin", Id = 1 });
                //     APIContext.Users.AddAsync(new User { Username = "oggiVUser", Id = 2 });
                //     APIContext.SaveChangesAsync();
                // }
                _client = factory.CreateClient();
            }
        }

        [TestMethod]
        public async Task GetSkeleton()
        {
            var response = await _client.GetAsync("/users/skeleton");
            var value = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            Assert.IsTrue(value.Contains("This is the User Service Skeleton endpoint."));
        }
        
        // [TestMethod]
        // public async Task SendSkeletonEmail()
        // {
        //     var response = await _client.GetAsync("/users/skeleton/email");
        //
        //     // Poll for the response
        //     string value = null;
        //     for (int i = 0; i < 5; i++) // Try up to 5 times
        //     {
        //         value = await response.Content.ReadAsStringAsync();
        //         if (value.Length > 0)
        //         {
        //             break;
        //         }
        //         await Task.Delay(1000); // Wait for 1 second before trying again
        //     }
        //
        //     response.EnsureSuccessStatusCode();
        //     Assert.IsTrue(value.Contains("Successfully sent email!"));
        // }
    }
}