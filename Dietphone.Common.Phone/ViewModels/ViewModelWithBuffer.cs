using Dietphone.Tools;
using Dietphone.Models;
using System.Collections.Generic;

namespace Dietphone.ViewModels
{
    public class ViewModelWithBuffer<TModel> : ViewModelBase where TModel : Entity, new()
    {
        public TModel Model { get; private set; }
        protected bool IsBuffered { get; private set; }
        protected readonly Factories factories;
        private TModel buffer;

        public ViewModelWithBuffer(TModel model, Factories factories)
        {
            Model = model;
            this.factories = factories;
        }

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
                buffer.SetOwner(factories);
            }
        }

        public void FlushBuffer()
        {
            if (IsBuffered)
            {
                Model.CopyFrom(buffer);
            }
        }

        public void ClearBuffer()
        {
            IsBuffered = false;
        }
    }
}