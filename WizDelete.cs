using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;

namespace 银河编辑器数据向导产生器新版
{
    public partial class WizardElementStandard
    {
        public virtual void RemoveItself()
        {

        }
        protected void ItemDeleteTrigger(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (WizardElementStandard c1 in e.OldItems)
                {
                    if (c1.Wiz.PreviousSibling is XmlComment)
                        Wiz.RemoveChild(c1.Wiz.PreviousSibling);
                    c1.RemovePage();
                    c1.RemoveItself();
                    Wiz.RemoveChild(c1.Wiz);
                }
        }
        public virtual void RemovePage()
        {
            if (this.Binding != null)
            {
                ((TabControl)
                    VisualTreeHelper.GetParent(
                        VisualTreeHelper.GetParent(
                            VisualTreeHelper.GetParent(
                                this.Binding)))).Items.Remove(this.Binding);
            }
        }
    }

    public partial class WizDocument
    {
        protected void ItemDeleteTrigger(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (WizardElementStandard c1 in e.OldItems)
                {
                    if (c1.Wiz.PreviousSibling is XmlComment)
                        WizRoot.RemoveChild(c1.Wiz.PreviousSibling);
                    c1.RemovePage();
                    c1.RemoveItself();
                    WizRoot.RemoveChild(c1.Wiz);
                }
        }
        public virtual void RemovePage()
        {
            if (this.Binding != null)
            {
                ((TabControl)
                    VisualTreeHelper.GetParent(
                        VisualTreeHelper.GetParent(
                            VisualTreeHelper.GetParent(
                                this.Binding)))).Items.Remove(this.Binding);
            }
        }
        public void RemoveItself()
        {
            foreach (WizEl c1 in this.Wizs)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
        }
    }

    public partial class WizEl : WizardElementStandard
    {
        public override void RemoveItself()
        {
            foreach (WCondition c1 in Conditions)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (WInput c1 in Inputs)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (WInstruction c1 in Instructions)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (WEntry c1 in Entries)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (WMacro c1 in Macros)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (WValidate c1 in Validates)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
        }
    }

    public partial class WCondition : WizardElementStandard
    {
        public override void RemoveItself()
        {
            foreach (WCondition c1 in Conditions)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
        }
    }

    public partial class WInput : WizardElementStandard
    {
        public override void RemoveItself()
        {
            foreach (WCondition c1 in Conditions)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (Item c1 in Items)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
        }
    }

    public partial class WValidate : WizardElementStandard
    {
        public override void RemoveItself()
        {
            foreach (WCondition c1 in Conditions)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
        }
    }

    public partial class WEntry : WizardElementStandard
    {
        public override void RemoveItself()
        {
            foreach (WCondition c1 in Conditions)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (Field c1 in Fields)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
            foreach (Token c1 in Tokens)
            {
                c1.RemovePage();
                c1.RemoveItself();
            }
        }
        public partial class Field : WizardElementStandard
        {
            public override void RemoveItself()
            {
                foreach (WCondition c1 in Conditions)
                {
                    c1.RemovePage();
                    c1.RemoveItself();
                }
            }
        }
        public partial class Token : WizardElementStandard
        {
            public override void RemoveItself()
            {
                foreach (WCondition c1 in Conditions)
                {
                    c1.RemovePage();
                    c1.RemoveItself();
                }
            }
        }
    }
}
