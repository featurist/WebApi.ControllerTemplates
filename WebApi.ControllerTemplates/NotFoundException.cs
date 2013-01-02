using System;

namespace WebApi.ControllerTemplates
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
