namespace app
{
    public enum WebResultCode
    {
        Success = 1,
        NetError,
        Fail
    }
    public class WebResult
    {
        public WebResultCode code { get; set; }
        public object data;
        public string url;
    }
}