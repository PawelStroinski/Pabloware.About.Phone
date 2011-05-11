using System;

namespace Dietphone.ViewModels
{
    public class ViewModelBuffer<TModel> : ViewModelBase where TModel : class, new()
    {
        public TModel Model { get; protected set; }
        protected bool IsBuffered { get; private set; }
        private TModel buffer;

        protected TModel BufferOrModel
        {
            get
            {
                if (IsBuffered)
                {
                    return buffer;
                }
                else
                {
                    return Model;
                }
            }
        }

        public void MakeBuffer()
        {
            if (!IsBuffered)
            {
                IsBuffered = true;
                buffer = Model.GetCopy();
            }
        }

        public void FlushBuffer()
        {
            if (IsBuffered)
            {
                Model.CopyFrom(buffer);
            }
        }
    }
}