namespace WebApi.ControllerTemplates
{
    public class RetrievedOrNotModified<TInstance>
    {
        private static readonly RetrievedOrNotModified<TInstance> NotModifiedInstance = new RetrievedOrNotModified<TInstance>();

        public static RetrievedOrNotModified<TInstance> Retrieved(TInstance instance)
        {
            return new RetrievedOrNotModified<TInstance> { WasRetrieved = true, Value = instance };
        }
        
        public static RetrievedOrNotModified<TInstance> NotModified
        {
            get { return NotModifiedInstance; }
        }

        public bool WasRetrieved { get; set; }
        public TInstance Value { get; set; }
    }
}