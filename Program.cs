using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

[TestFixture]
public class SauceDemoTests
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IPage _page = null!;
    private IBrowserContext _context = null!;

    [SetUp]
    public async Task SetUp()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
        _page = await _context.NewPageAsync();
    }

    [Test]
    public async Task CompletePurchaseFlow()
    {
        try
        {
            // 1. Navigate to the Sauce Labs Sample Application
            await _page.GotoAsync("https://www.saucedemo.com/");
            
            // 2. Enter valid credentials to log in
            await _page.FillAsync("#user-name", "standard_user");
            await _page.FillAsync("#password", "secret_sauce");
            await _page.ClickAsync("#login-button");
        

            // 3. Verify login and redirection to the products page
            await _page.WaitForSelectorAsync(".inventory_list");
            Assert.IsTrue(await _page.Locator(".inventory_list").IsVisibleAsync(), "Products page not displayed after login.");

            // 4. Select a T-shirt
            await _page.ClickAsync("text=Sauce Labs Bolt T-Shirt");

            // 5. Verify T-shirt details page
            await _page.WaitForSelectorAsync(".inventory_details_desc_container");
            Assert.IsTrue(await _page.Locator(".inventory_details_desc_container").IsVisibleAsync(), "T-shirt details page not displayed.");

            // 6. Click "Add to Cart"
            await _page.ClickAsync("button:has-text('Add to cart')");

            // 7. Verify item added to cart
            var cartBadge = await _page.Locator(".shopping_cart_badge").InnerTextAsync();
            Assert.AreEqual("1", cartBadge, "Cart does not show the correct item count.");

            // 8. Navigate to the cart
            await _page.ClickAsync(".shopping_cart_link");

            // 9. Verify cart page displayed
            await _page.WaitForSelectorAsync(".cart_item");
            Assert.IsTrue(await _page.Locator(".cart_item").IsVisibleAsync(), "Cart page not displayed.");

            // 10. Review items in the cart
            var itemName = await _page.Locator(".inventory_item_name").InnerTextAsync();
            var itemPrice = await _page.Locator(".inventory_item_price").InnerTextAsync();
            Assert.IsNotEmpty(itemName, "Item name is missing.");
            Assert.IsNotEmpty(itemPrice, "Item price is missing.");

            // 11. Click "Checkout"
            await _page.ClickAsync("button:has-text('Checkout')");

            // 12. Verify checkout information page
            await _page.WaitForSelectorAsync(".checkout_info");
            Assert.IsTrue(await _page.Locator(".checkout_info").IsVisibleAsync(), "Checkout information page not displayed.");

            // 13. Enter checkout information
            await _page.FillAsync("#first-name", "John");
            await _page.FillAsync("#last-name", "Doe");
            await _page.FillAsync("#postal-code", "12345");
            await _page.ClickAsync("#continue");

            // 14. Verify order summary page
            await _page.WaitForSelectorAsync(".summary_info");
            Assert.IsTrue(await _page.Locator(".summary_info").IsVisibleAsync(), "Order summary page not displayed.");

            // 15. Click "Finish"
            await _page.ClickAsync("button:has-text('Finish')");

            // 16. Verify order confirmation page
            await _page.WaitForSelectorAsync(".complete-header");
            Assert.IsTrue(await _page.Locator(".complete-header").IsVisibleAsync(), "Order confirmation page not displayed.");

            // 17. Logout
            await _page.ClickAsync("#react-burger-menu-btn");
            await _page.ClickAsync("#logout_sidebar_link");

            // 18. Verify logout
            await _page.WaitForSelectorAsync("#login-button");
            Assert.IsTrue(await _page.Locator("#login-button").IsVisibleAsync(), "Logout unsuccessful.");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed: {ex.Message}");
        }
    }

    [TearDown]
    public async Task TearDown()
    {
        if (_browser != null)
        {
            await _browser.CloseAsync();
        }
    }
}


