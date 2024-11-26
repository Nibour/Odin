namespace IPStackCommunicationLibrary
{
    public class IPServiceNotAvailableException : Exception
    {
        public IPServiceNotAvailableException(string message, Exception? innerException = null) 
            : base(message, innerException) { }
    }
}
