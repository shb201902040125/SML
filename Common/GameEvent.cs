using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace SML.Common
{
    public class GameEvent<TEvent, TUpdateParamters>
        : ModType
        where TEvent : GameEvent<TEvent, TUpdateParamters>
    {
        public virtual bool IsCompleted { get; protected set; }
        public event Action<TEvent, TUpdateParamters> OnUpdate;
        public event Action<TEvent> OnCompleted;
        protected ref Action<TEvent, TUpdateParamters> onUpdate => ref OnUpdate;
        protected ref Action<TEvent> onComplete => ref OnCompleted;
        public virtual bool Update(TUpdateParamters paramters)
        {
            return false;
        }
        public void BindHandler(GameEventHandler<TEvent, TUpdateParamters> handler)
        {
            onUpdate += delegate (TEvent @event, TUpdateParamters paramters)
            {
                handler.Update(@event);
            };
        }
        protected override void Register() { }
    }
    public class GameEventHandler<TEvent, TUpdateParamters>
        : GameEvent<GameEventHandler<TEvent, TUpdateParamters>, GameEvent<TEvent, TUpdateParamters>>
        where TEvent : GameEvent<TEvent, TUpdateParamters>
    {
        public override bool Update(GameEvent<TEvent, TUpdateParamters> @event)
        {
            return Handle(@event);
        }
        public virtual bool Handle(GameEvent<TEvent, TUpdateParamters> @event)
        {
            return false;
        }
    }
    public class GameEvent<TEvent, TUpdateParamters, THandler>
        : GameEvent<TEvent, TUpdateParamters>
        where TEvent : GameEvent<TEvent, TUpdateParamters, THandler>
        where THandler : GameEventHandler<TEvent, TUpdateParamters, THandler>
    {
    }
    public class GameEventHandler<TEvent, TUpdateParamters, THandler>
        : GameEventHandler<TEvent, TUpdateParamters>
        where TEvent : GameEvent<TEvent, TUpdateParamters, THandler>
        where THandler : GameEventHandler<TEvent, TUpdateParamters, THandler>
    {
        public sealed override bool Update(GameEvent<TEvent, TUpdateParamters> @event)
        {
            return Update((TEvent)@event);
        }
        public virtual bool Update(TEvent @event)
        {
            return false;
        }
        public sealed override bool Handle(GameEvent<TEvent, TUpdateParamters> @event)
        {
            return Handle((TEvent)@event);
        }
        public virtual bool Handle(TEvent @event)
        {
            return false;
        }
    }
}