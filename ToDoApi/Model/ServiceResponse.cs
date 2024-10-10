public class ServiceResponse<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public int ErrorNumber { get; set; }
    public string ErrorDescription { get; set; }
}