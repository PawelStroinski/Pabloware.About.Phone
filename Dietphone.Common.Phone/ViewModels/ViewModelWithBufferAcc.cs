using Dietphone.Models;
using System.Collections.Generic;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class ViewModelWithBufferAcc<TModel> : ViewModelWithBuffer<TModel> where TModel : Entity, new()
    {
        public ViewModelWithBufferAcc(TModel model, Factories factories)
            : base(model, factories)
        {
        }

        public void AddModelTo(List<TModel> target)
        {
            target.Add(BufferOrModel);
        }

        public void CopyFromModel(TModel source)
        {
            BufferOrModel.CopyFrom(source);
        }
    }
}
