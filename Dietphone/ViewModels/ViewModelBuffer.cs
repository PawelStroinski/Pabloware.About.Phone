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
            if (IsBuffered)
            {
                throw new InvalidOperationException("Buffer can be made only once.");
            }
            IsBuffered = true;
            buffer = new TModel();
            Model.CopyToSameType(buffer);
        }

        public void FlushBuffer()
        {
            if (!IsBuffered)
            {
                throw new InvalidOperationException("Buffer was not made.");
            }
            buffer.CopyToSameType(Model);
        }
    }
}