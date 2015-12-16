using System.ComponentModel.DataAnnotations;

namespace VerIDial.SDK.Sample.Models
{
    /// <summary>
    /// Submit number to initiate a request
    /// </summary>
    public class VerifyModel
    {
        /// <summary>
        /// The phone number to initiate the request, you should validate this according to your countries numbering plan(s).
        /// </summary>
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// This is automatically populated with the "value" property of the submit button when submitted.
        /// </summary>            
        public string submit { get; set; }
    }

     /// <summary>
    /// Model to return a simple message
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }
    }
}
