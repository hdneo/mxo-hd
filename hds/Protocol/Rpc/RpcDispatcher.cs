using System;
using System.Collections.Generic;
using System.Reflection;
using hds.Core.Wire;

namespace hds.Protocol.Rpc
{
    /// <summary>Everything a RPC handler needs: the sending client and the message.</summary>
    public sealed class RpcContext
    {
        public RpcContext(WorldClient client, RpcMessage message)
        {
            Client = client;
            Message = message;
        }

        public WorldClient Client { get; }
        public RpcMessage Message { get; }
        public ushort Opcode => Message.Opcode;

        /// <summary>Copy of the payload (legacy handlers take it by ref and may modify it).</summary>
        public byte[] Payload => Message.Payload;

        public WireReader CreateReader() => new WireReader(Message.Payload);
    }

    /// <summary>Marks a method <c>void Method(RpcContext context)</c> as handler for a client RPC.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class RpcHandlerAttribute : System.Attribute
    {
        public RpcHandlerAttribute(ushort opcode)
        {
            Opcode = opcode;
        }

        public RpcHandlerAttribute(RPCRequestHeader opcode) : this((ushort) opcode)
        {
        }

        public ushort Opcode { get; }
    }

    /// <summary>Routes client RPC messages to their handlers.</summary>
    public sealed class RpcDispatcher
    {
        private readonly Dictionary<ushort, (string Name, Action<RpcContext> Handler)> handlers =
            new Dictionary<ushort, (string, Action<RpcContext>)>();

        public IReadOnlyCollection<ushort> Opcodes => handlers.Keys;

        public void Register(ushort opcode, string name, Action<RpcContext> handler)
        {
            if (handlers.TryGetValue(opcode, out var existing))
            {
                throw new InvalidOperationException($"RPC 0x{opcode:x} is already handled by {existing.Name}, cannot add {name}");
            }
            handlers[opcode] = (name, handler);
        }

        public void Register(RPCRequestHeader opcode, Action<RpcContext> handler) =>
            Register((ushort) opcode, opcode.ToString(), handler);

        /// <summary>Registers all [RpcHandler] methods of an object.</summary>
        public void RegisterHandlers(object instance)
        {
            foreach (var method in instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                foreach (var attribute in method.GetCustomAttributes<RpcHandlerAttribute>())
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1 || parameters[0].ParameterType != typeof(RpcContext))
                    {
                        throw new InvalidOperationException($"{instance.GetType().Name}.{method.Name} must take exactly one RpcContext");
                    }
                    var handler = (Action<RpcContext>) method.CreateDelegate(typeof(Action<RpcContext>), instance);
                    Register(attribute.Opcode, instance.GetType().Name + "." + method.Name, handler);
                }
            }
        }

        public bool IsHandled(ushort opcode) => handlers.ContainsKey(opcode);

        /// <summary>Invokes the handler; returns false for unknown opcodes.</summary>
        public bool Dispatch(WorldClient client, RpcMessage message)
        {
            if (!handlers.TryGetValue(message.Opcode, out var entry))
            {
                return false;
            }
            entry.Handler(new RpcContext(client, message));
            return true;
        }
    }
}
