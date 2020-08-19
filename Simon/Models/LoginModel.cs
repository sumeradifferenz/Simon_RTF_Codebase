using System;
namespace Simon.Models
{
    public class LoginModel
    {
        public string token { get; set; }
        public string userId { get; set; }
        public string deviceType { get; set; }
    }

    public class LoginModelResponse
    {
        public string token { get; set; }
        public string sesionId { get; set; }
    }
}
