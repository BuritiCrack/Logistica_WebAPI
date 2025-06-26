namespace LogisticoWebAPI.Shared.Responses
{
    public class ActionResponses<T> where T : class
    {
        public bool WassSuccess { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }
    }
}