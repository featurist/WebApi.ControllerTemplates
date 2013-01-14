using RestfulRoles;

namespace WebApi.ControllerTemplates.Roles
{
    public interface Repository<T> : Inserter<T>, Upserter<T>, Retriever<T>, Deleter
    {
    }
}
