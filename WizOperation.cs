using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace 银河编辑器数据向导产生器新版
{
    public partial class WizardElementStandard
    {
        public virtual void Copy() { }
    }

    public partial class WizEl
    {
        private void InitN()
        {
            Conditions.CollectionChanged += ItemDeleteTrigger;
            Macros.CollectionChanged += ItemDeleteTrigger;
            Inputs.CollectionChanged += ItemDeleteTrigger;
            Entries.CollectionChanged += ItemDeleteTrigger;
            Validates.CollectionChanged += ItemDeleteTrigger;
            Instructions.CollectionChanged += ItemDeleteTrigger;
        }
        public WCondition NewCondition()
        {
            WCondition a1 = new WCondition(this);
            Conditions.Add(a1);
            Notify("Conditions");
            return a1;
        }
        public WInstruction NewInstruction()
        {
            WInstruction a1 = new WInstruction(this);
            Instructions.Add(a1);
            Notify("Instructions");
            return a1;
        }
        public WEntry NewEntry()
        {
            WEntry a1 = new WEntry(this);
            Entries.Add(a1);
            Notify("Entries");
            return a1;
        }
        public WInput NewInput()
        {
            WInput a1 = new WInput(this);
            Inputs.Add(a1);
            Notify("Inputs");
            return a1;
        }
        public WMacro NewMacro()
        {
            WMacro a1 = new WMacro(this);
            Macros.Add(a1);
            Notify("Macros");
            return a1;
        }
        public WValidate NewValidate()
        {
            WValidate a1 = new WValidate(this);
            Validates.Add(a1);
            Notify("Validates");
            return a1;
        }
        public override void Copy()
        {
            XmlElement cp1 = (XmlElement)Wiz.CloneNode(true);
            WizEl el1 = new WizEl(cp1,Parent);
            Parent.Wizs.Add(el1);
        }
    }
    public partial class WInput
    {
        public WCondition NewCondition()
        {
            WCondition a1 = new WCondition(this);
            Conditions.Add(a1);
            Notify("Conditions");
            return a1;
        }
        public Item NewItem()
        {
            Item a1 = new Item(this);
            Items.Add(a1);
            Notify("Items");
            return a1;
        }
    }
}
