using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading;
using Common.Logging;

namespace Helios.Wcf.WebHost.Inspectors
{
    public class IncomingMessageLoggerInspector : IDispatchMessageInspector
    {
        private static readonly ILog Logger = LogManager.GetLogger<IncomingMessageLoggerInspector>();

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var context = OperationContext.Current;
            if (context == null)
                return null;

            var operationName = ParseOperationName(context.IncomingMessageHeaders.Action);

            return MarkStartOfOperation(context.EndpointDispatcher.ContractName, operationName, context.SessionId);
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var context = OperationContext.Current;
            if (context == null)
                return;

            var operationName = ParseOperationName(context.IncomingMessageHeaders.Action);

            MarkEndOfOperation(context.EndpointDispatcher.ContractName, operationName, context.SessionId,
                correlationState);
        }

        #endregion

        #region Private Methods

        private static string ParseOperationName(string action)
        {
            if (string.IsNullOrEmpty(action))
                return action;

            string actionName = action;

            int index = action.LastIndexOf('/');
            if (index >= 0)
            {
                actionName = action.Substring(index + 1);
            }

            return actionName;
        }

        private static object MarkStartOfOperation(string inspectorType, string operationName, string sessionId)
        {
            var message = string.Format("接收到服务调用请求[{0}.{1}], SessionId={2}, ThreadId={3}",
                inspectorType, operationName, sessionId, Thread.CurrentThread.ManagedThreadId);

            Logger.Debug(message);

            return Stopwatch.StartNew();
        }

        private static void MarkEndOfOperation(
            string inspectorType, string operationName,
            string sessionId, object correlationState)
        {
            var watch = (Stopwatch) correlationState;
            watch.Stop();

            // TODO: 目前先将该数据输出到日志中，后期将此数据通过消息队列推送给后端性能分析的数据库进行分析
            var message = string.Format("服务调用请求[{0}.{1}]执行完毕, 耗时 {2} 毫秒, SessionId={3}, ThreadId={4}",
                inspectorType, operationName, watch.ElapsedMilliseconds, sessionId, Thread.CurrentThread.ManagedThreadId);

            Logger.Debug(message);
        }

        #endregion
    }
}