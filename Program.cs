using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

class Program
{
    public static async Task Main(string[] args)
    {
        // Initialize Playwright
        using var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
        var page = await context.NewPageAsync();

        try
        {
            // 1. Navigate to the Sauce Labs Sample Application
            await page.GotoAsync("https://www.saucedemo.com/");
            
            // 2. Enter valid credentials to log in
            await page.FillAsync("#user-name", "standard_user");
            await page.FillAsync("#password", "secret_sauce");
            await page.ClickAsync("#login-button");

            // 3. Verify login and redirection to the products page
            await page.WaitForSelectorAsync(".inventory_list");
            Console.WriteLine("Login successful. Redirected to Products page.");

            // 4. Select a T-shirt
            await page.ClickAsync("text=Sauce Labs Bolt T-Shirt");

            // 5. Verify T-shirt details page
            await page.WaitForSelectorAsync(".inventory_details_desc_container");
            Console.WriteLine("T-shirt details page displayed.");

            // 6. Click "Add to Cart"
            await page.ClickAsync("button:has-text('Add to cart')");

            // 7. Verify item added to cart
            var cartBadge = await page.Locator(".shopping_cart_badge").InnerTextAsync();
            Console.WriteLine($"Cart updated: {cartBadge} item(s).");

            // 8. Navigate to the cart
            await page.ClickAsync(".shopping_cart_link");

            // 9. Verify cart page displayed
            await page.WaitForSelectorAsync(".cart_item");
            Console.WriteLine("Cart page displayed.");

            // 10. Review items in the cart
            var itemName = await page.Locator(".inventory_item_name").InnerTextAsync();
            var itemPrice = await page.Locator(".inventory_item_price").InnerTextAsync();
            Console.WriteLine($"Cart contains: {itemName} priced at {itemPrice}.");

            // 11. Click "Checkout"
            await page.ClickAsync("button:has-text('Checkout')");

            // 12. Verify checkout information page
            await page.WaitForSelectorAsync(".checkout_info");
            Console.WriteLine("Checkout information page displayed.");

            // 13. Enter checkout information
            await page.FillAsync("#first-name", "John");
            await page.FillAsync("#last-name", "Doe");
            await page.FillAsync("#postal-code", "12345");
            await page.ClickAsync("#continue");

            // 14. Verify order summary page
            await page.WaitForSelectorAsync(".summary_info");
            Console.WriteLine("Order summary page displayed.");

            // 15. Click "Finish"
            await page.ClickAsync("button:has-text('Finish')");

            // 16. Verify order confirmation page
            await page.WaitForSelectorAsync(".complete-header");
            Console.WriteLine("Order confirmation page displayed. Purchase successful.");

            // 17. Logout
            await page.ClickAsync("#react-burger-menu-btn");
            await page.ClickAsync("#logout_sidebar_link");

            // 18. Verify logout
            await page.WaitForSelectorAsync("#login-button");
            Console.WriteLine("Logout successful. Redirected to login page.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test failed: {ex.Message}");
        }
        finally
        {
            // Close browser
            await browser.CloseAsync();
        }
    }
}

