using System;

namespace Maruko.Event.Bus
{
    /// <summary>
    /// Implements <see cref="IEventData"/>
    /// �¼�Դ���ࣺ�����¼���Ϣ�����ڲ�������
    /// </summary>
    [Serializable]
    public abstract class EventData : IEventData
    {
        /// <summary>
        /// �¼�������ʱ��
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// �����¼��Ķ���
        /// </summary>
        public object EventSource { get; set; }

        /// <summary>
        /// ���캯��
        /// </summary>
        protected EventData()
        {
            EventTime = DateTime.Now;
        }
    }
}