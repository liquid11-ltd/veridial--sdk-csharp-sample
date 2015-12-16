using System;
using System.Net;
using System.Web.Mvc;
using VerIDial.SDK.Sample.Models;

namespace VerIDial.SDK.Sample.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Show a menu of items
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        #region Verify
        /// <summary>
        /// Show the screen
        /// </summary>
        /// <returns></returns>
        public ActionResult VerifyStart()
        {
            return View();
        }

        /// <summary>
        /// Will process a demo request
        /// </summary>        
        /// <param name="model">The view model continaing the number and method to use</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VerifyStart(VerifyModel model)
        {
            if (model == null || String.IsNullOrEmpty(model.PhoneNumber))
            {
                // No model or blank number, then display an error                
                ModelState.AddModelError("", "Sorry, you must enter a valid phone number to procees.");
            }
            else
            {
                // Examine the submit button the user pressed and pick the corresponding API request method.
                RequestMethod method;
                switch (model.submit)
                {
                    case "CALL":
                        method = RequestMethod.VOICE;
                        break;

                    case "APP":
                        method = RequestMethod.APP;
                        break;

                    default:
                        method = RequestMethod.SMS;
                        break;
                }
                // Create an Api instance using our helper method
                var api = CreateApi();
                try
                {
                    // Attempt to create a new request using our Api instance passing in the 
                    // phone number we want to send the pin to and the method the user selected.
                    var token = api.Create(method, model.PhoneNumber);
                    return RedirectToAction("VerifyPinEntry", new { token = token });
                }
                catch (ApiException ex)
                {
                    // Check the status code and deal with appropriately, for example if
                    // the method was App and the status is Handset Not Registered, then fall back
                    // to SMS or encourage the end user to download the app and register.
                    ModelState.AddModelError("", ex.ApiStatus.StatusDescription);
                }
                catch (WebException ex)
                {
                    // Can throw HTTP 401 Unauthorized due to invalid credentials   
                    // Replace in production code with your own generic error message
                    // and handle with your own unexpected exception handling logic
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(model);
        }

        /// <summary>
        /// Shows the demo page with pin plugin ready for data entry
        /// </summary>
        /// <param name="token">the unique identifier for the request</param>
        /// <returns></returns>        
        public ActionResult VerifyPinEntry(Guid token)
        {
            // Since we don't know which port or address you may be running this site under. 
            // Get the base URL dynamically from the current page url. 
            string baseUrl = Request.Url.GetComponents(UriComponents.Scheme | UriComponents.HostAndPort, UriFormat.Unescaped);
            // Convert the token from a guid to a string that we can concatiate into our iframe url
            string tokenString = token.ToString();
            // Create the iframe url using the plugin provided by VerIDial passing through some parameters we wish to use.
            // If we had a longer PIN that the default of 4, or wished to change the default colours, we would more 
            // parameters.
            ViewBag.IFrame =
                String.Format("https://www.veridial.co.uk/plugin?token={0}&pinSuccessUrl={1}/Home/VerifySuccess?token={0}&pinFailureUrl={1}/Home/VerifyFailure?token={0}", tokenString, baseUrl);
            return View();
        }

        /// <summary>
        /// GET: /Home/Failure
        /// Get the status of the request and show it on the screen
        /// </summary>
        /// <param name="token">the unique identifier for the request</param>
        /// <returns></returns>        
        public ActionResult VerifyFailure(Guid token)
        {
            var api = CreateApi();
            var status = api.Status(token);
            return View(new MessageModel() { Message = status.StatusDescription });
        }

        /// <summary>
        /// GET: /Home/Sucess
        /// Get the status of the request and show it on the screen
        /// </summary>
        /// <param name="token">the unique identifier for the request</param>
        /// <returns></returns>        
        public ActionResult VerifySuccess(Guid token)
        {
            var api = CreateApi();
            var status = api.Status(token);
            return View(new MessageModel() { Message = status.StatusDescription });
        }
        #endregion

        #region Balance
        /// <summary>
        /// GET: /Home/Balance
        /// Get your current balance and show on screen.
        /// </summary>     
        /// <returns></returns>        
        public ActionResult Balance()
        {
            var api = CreateApi();
            string message;
            try
            {
                var balance = api.Balance();
                message = String.Format("Your balance is currently {0}", balance);

            }
            catch (WebException ex)
            {             
                // Can throw HTTP 401 Unauthorized due to invalid credentials   
                message =  ex.Message;
            }
            return View(new MessageModel() { Message = message });
        }
        #endregion
        
        #region Helpers
        /// <summary>
        /// Creates an api instance with the stored credentials from web.config
        /// </summary>
        /// <returns></returns>
        private Api CreateApi()
        {
            // Get the authentication credentials. 
            // Ensure you have changed these in the web.config or project settings to your 
            // credentials before use.           
            int installationId = Properties.Settings.Default.InstallationId;
            string apiKey = Properties.Settings.Default.ApiKey;

            // Create a new instance passing in the keys, note there is no check to
            // ensure the credentials are correct until you make (or try to make) your
            // first api call.
            var api = new Api(installationId, apiKey);
            return api;
        }
        #endregion
    }
}
