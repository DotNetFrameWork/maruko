using Maruko.Dependency;

namespace Maruko.Events.Bus.Handlers
{
    /// <summary>
    /// �����¼������������ӿڣ����е��¼�����Ҫʵ�ָýӿ�
    /// Implement <see cref="IEventHandler{TEventData}"/>
    /// </summary>
    public interface IEventHandler : IDependencyTransient
    {

    }
}