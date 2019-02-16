using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace 银河编辑器数据向导产生器新版
{
    public partial class WizardElementStandard : INotifyPropertyChanged
    {
        public TabItem Binding { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string PropName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropName));
        }
    }
    public partial class WizDocument : INotifyPropertyChanged
    {
        public TabItem Binding { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string PropName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropName));
        }
    }
}
