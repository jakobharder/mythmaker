using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MythMaker.Utils
{
    public interface IDataChanged
    {
        event PropertyChangedEventHandler DataChanged;
    }

    public class ObservableDataList<T> : ObservableCollection<T> where T : IDataChanged
    {
        public new void Add(T item)
        {
            if (item == null || Contains(item))
                return;

            base.Add(item);
            item.DataChanged += Item_DataChanged;
        }

        public new void Remove(T item)
        {
            if (item == null || !Contains(item))
                return;

            item.DataChanged -= Item_DataChanged;
            base.Remove(item);
        }

        private void Item_DataChanged(object sender, PropertyChangedEventArgs e)
        {
            // use replaced as their is no changed
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender);
            OnCollectionChanged(args);
        }
    }
}
