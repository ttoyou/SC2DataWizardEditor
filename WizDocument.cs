using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.Specialized;

namespace 银河编辑器数据向导产生器新版
{

    public partial class WizardElementStandard
    {
        public XmlElement Wiz;
        public void SetAttribute(string attribute, string value)
        {
            if (value == null || value.Length == 0)
                Wiz.RemoveAttribute(attribute);
            else
                Wiz.SetAttribute(attribute, value);
        }

        public void ElementInnerSet(string name, string innerText)
        {
            if (innerText == null || innerText.Length == 0)
            {
                foreach (XmlNode nd in Wiz.SelectNodes(name))
                    Wiz.RemoveChild(nd);
            }
            else
            {
                var list1 = Wiz.SelectNodes(name);
                if (list1.Count != 0)
                {
                    foreach (XmlNode nd in list1)
                    {
                        if (nd is XmlElement elem)
                        {
                            elem.InnerText = innerText;
                        }
                    }
                }
                else
                {
                    XmlElement newElement = Wiz.OwnerDocument.CreateElement(name);
                    Wiz.AppendChild(newElement);
                    newElement.InnerText = innerText;
                }
            }
        }

        public string ElementInnerGet(string name)
        {
            string temp = "";
            foreach (XmlNode nd in Wiz.SelectNodes(name))
            {
                if (nd is XmlElement elem)
                {
                    temp = elem.InnerText;
                }
            }
            return temp;
        }
        public void ElementElementAttributeSet(string name, string attribute, string value)
        {
            if (value == null || value.Length == 0)
            {
                foreach (XmlNode nd in Wiz.SelectNodes(name))
                {
                    if (nd is XmlElement elem)
                    {
                        elem.RemoveAttribute(attribute);
                        if (!elem.HasChildNodes && !elem.HasAttributes)
                        {
                            Wiz.RemoveChild(nd);
                        }
                    }
                }
            }
            else
            {
                var list1 = Wiz.SelectNodes(name);
                if (list1.Count != 0)
                {
                    foreach (XmlNode nd in Wiz.SelectNodes(name))
                    {
                        if (nd is XmlElement elem)
                        {
                            elem.SetAttribute(attribute, value);
                        }
                    }
                }
                else
                {
                    XmlElement newElement = Wiz.OwnerDocument.CreateElement(name);
                    Wiz.AppendChild(newElement);
                    newElement.SetAttribute(attribute, value);
                }
            }
        }

        public string ElementElementAttributeGet(string name, string attribute)
        {
            string temp = "";
            foreach (XmlNode nd in Wiz.SelectNodes(name))
            {
                if (nd is XmlElement elem)
                    temp = elem.GetAttribute(attribute);
            }
            return temp;
        }

        public bool ElementExist(string name)
        {
            bool temp = false;
            foreach (XmlNode nd in Wiz.SelectNodes(name))
            {
                if (nd is XmlElement elem) { temp = true; }
            }
            return temp;
        }

        public void ElementExistSet(string name, bool exi)
        {
            if (exi)
            {
                if (Wiz.SelectSingleNode(name) == null)
                {
                    XmlElement newElement = Wiz.OwnerDocument.CreateElement(name);
                    Wiz.AppendChild(newElement);
                }

            }
            else
            {
                foreach (XmlNode nd in Wiz.SelectNodes(name))
                {
                    if (nd is XmlElement elem) { Wiz.RemoveChild(nd); }
                }

            }
        }

        public string Comment
        {
            get
            {
                if (Wiz.PreviousSibling is XmlComment)
                    return Wiz.PreviousSibling.Value;
                return "";
            }
            set
            {
                if (Wiz.PreviousSibling is XmlComment)
                    Wiz.PreviousSibling.Value = value;
                else
                    Wiz.ParentNode.InsertBefore(Wiz.OwnerDocument.CreateComment(value), Wiz);
                Notify("Comment");
            }
        }

    }

    public partial class WizDocument
    {
        XmlDocument wdoc = new XmlDocument();

        public ObservableCollection<WizEl> Wizs { get; set; } = new ObservableCollection<WizEl>();
        public bool IsSaved { get; set; } = true;
        public XmlElement WizRoot { get; set; }
        public string DefaultPath { get; set; } = "";

        public WizDocument(string path)
        {
            wdoc.Load(path);
            DefaultPath = path;
            WizRoot = wdoc.DocumentElement;
            foreach (XmlNode node in WizRoot.ChildNodes)
            {
                if (node is XmlElement && node.Name == "wizard") { Wizs.Add(new WizEl((XmlElement)node, this)); }
            }
            wdoc.NodeChanged += DocumentChanged;
            wdoc.NodeInserted += DocumentChanged;
            wdoc.NodeRemoved += DocumentChanged;
            InitN();
        }

        private void DocumentChanged(object sender, XmlNodeChangedEventArgs e) { IsSaved = false; }

        public WizEl NewWizard()
        {
            WizEl a1 = new WizEl(this);
            Wizs.Add(a1);
            return a1;
        }

        public WizDocument()
        {
            DefaultPath = null;
            wdoc.AppendChild(wdoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            WizRoot = wdoc.AppendChild(wdoc.CreateElement("wizardfile")) as XmlElement;
            Wizs.Add(new WizEl(WizRoot.AppendChild(wdoc.CreateElement("wizard")) as XmlElement, this));
            wdoc.NodeChanged += DocumentChanged;
            wdoc.NodeInserted += DocumentChanged;
            wdoc.NodeRemoved += DocumentChanged;
            InitN();
        }
        private void InitN()
        {
            Wizs.CollectionChanged += ItemDeleteTrigger;
        }

        public void Save(string path)
        {
            DefaultPath = path;
            wdoc.Save(path);
            IsSaved = true;
            Notify("IsSaved");
            Notify("DefaultPath");
        }
        public void Save() { wdoc.Save(DefaultPath); IsSaved = true; Notify("IsSaved"); }
    }

    public partial class WizEl : WizardElementStandard
    {
        public partial class WInstruction : WizardElementStandard
        {
            WizEl Parent;
            public WInstruction(XmlElement wiz, WizEl el)
            {
                Wiz = wiz;
                Parent = el;
            }
            public WInstruction(WizEl el)
            {
                Wiz = el.Wiz.OwnerDocument.CreateElement("instructions");
                el.Wiz.AppendChild(Wiz);
                Parent = el;
            }
            public string Page { set { SetAttribute("page", value); Notify("Page"); } get { return Wiz.GetAttribute("page"); } }
            public string Inner { set { Wiz.InnerText = value; Notify("Inner"); } get { return Wiz.InnerText; } }
        }
        public ObservableCollection<WCondition> Conditions { get; set; } = new ObservableCollection<WCondition>();
        public ObservableCollection<WInstruction> Instructions { get; set; } = new ObservableCollection<WInstruction>();
        public ObservableCollection<WEntry> Entries { get; set; } = new ObservableCollection<WEntry>();
        public ObservableCollection<WInput> Inputs { get; set; } = new ObservableCollection<WInput>();
        public ObservableCollection<WMacro> Macros { get; set; } = new ObservableCollection<WMacro>();
        public ObservableCollection<WValidate> Validates { get; set; } = new ObservableCollection<WValidate>();

        WizDocument Parent;
        public WizEl(XmlElement wiz, WizDocument root)
        {
            Wiz = wiz;
            foreach (XmlNode child in Wiz.ChildNodes)
            {
                if (child is XmlElement)
                {
                    switch (child.Name)
                    {
                        case "condition":
                            Conditions.Add(new WCondition((XmlElement)child));
                            break;
                        case "entry":
                            Entries.Add(new WEntry((XmlElement)child, this));
                            break;
                        case "input":
                            Inputs.Add(new WInput((XmlElement)child, this));
                            break;
                        case "macro":
                            Macros.Add(new WMacro((XmlElement)child, this));
                            break;
                        case "validate":
                            Validates.Add(new WValidate((XmlElement)child, this));
                            break;
                        case "instructions":
                            Instructions.Add(new WInstruction((XmlElement)child, this));
                            break;
                    }
                }
            }
            Parent = root;
            InitN();
        }

        public WizEl(WizDocument root)
        {
            Wiz = root.WizRoot.OwnerDocument.CreateElement("wizard");
            root.WizRoot.AppendChild(Wiz);
            Parent = root;
            InitN();
        }

        public string Id { set { SetAttribute("id", value); Notify("Id"); } get { return Wiz.GetAttribute("id"); } }
        public string Description
        {
            get { return ElementInnerGet("description"); }
            set { ElementInnerSet("description", value); Notify("Description"); }
        }
        public string Category
        {
            get { return ElementInnerGet("category"); }
            set { ElementInnerSet("category", value); Notify("Category"); }
        }
        public string Name
        {
            get { return ElementInnerGet("name"); }
            set { ElementInnerSet("name", value); Notify("Name"); }
        }
        public string ObjecttypesCreate { get { return GetObjectTypes("create"); } set { SetObjectTypes("create", value); Notify("ObjecttypesCreate"); } }
        public string ObjecttypesLoad { get { return GetObjectTypes("load"); } set { SetObjectTypes("load", value); Notify("ObjecttypesLoad"); } }
        public string ObjecttypesView { get { return GetObjectTypes("view"); } set { SetObjectTypes("view", value); Notify("ObjecttypesView"); } }
        private string GetObjectTypes(string man) { return ElementElementAttributeGet("objecttypes", man); }
        private void SetObjectTypes(string man, string value) { ElementElementAttributeSet("objecttypes", man, value); }

    }

    public partial class WCondition : WizardElementStandard
    {
        public WCondition(XmlElement wiz)
        {
            Wiz = wiz;
            foreach (XmlNode child in Wiz.ChildNodes)
            {
                if (child is XmlElement && child.Name == "condition")
                    Conditions.Add(new WCondition((XmlElement)child));
            }
            InitN();
        }
        public WCondition(WizardElementStandard el)
        {
            Wiz = el.Wiz.OwnerDocument.CreateElement("condition");
            el.Wiz.AppendChild(Wiz);
            InitN();
        }
        private void InitN()
        {
            Conditions.CollectionChanged += ItemDeleteTrigger;
        }
        public ObservableCollection<WCondition> Conditions { get; set; } = new ObservableCollection<WCondition>();
        public string Id { set { SetAttribute("id", value); Notify("Id"); } get { return Wiz.GetAttribute("id"); } }
        public string Input { set { SetAttribute("input", value); Notify("Input"); } get { return Wiz.GetAttribute("input"); } }
        public string Value { set { SetAttribute("value", value); Notify("Value"); } get { return Wiz.GetAttribute("value"); } }
        public string Match { set { SetAttribute("match", value); Notify("Match"); } get { return Wiz.GetAttribute("match"); } }
        public string Operator { set { SetAttribute("operator", value); Notify("Operator"); } get { return Wiz.GetAttribute("operator"); } }
        public string Nocase { set { SetAttribute("nocase", value); Notify("Nocase"); } get { return Wiz.GetAttribute("nocase"); } }
        public string Empty { set { SetAttribute("empty", value); Notify("Empty"); } get { return Wiz.GetAttribute("empty"); } }
        public string Visible { set { SetAttribute("visible", value); Notify("Visible"); } get { return Wiz.GetAttribute("visible"); } }
        public string Logic { set { SetAttribute("logic", value); Notify("Logic"); } get { return Wiz.GetAttribute("logic"); } }

        public WCondition NewCondition()
        {
            WCondition a1 = new WCondition(this);
            Conditions.Add(a1);
            Notify("Conditions");
            return a1;
        }
    }

    public partial class WInput : WizardElementStandard
    {
        public partial class Item : WizardElementStandard
        {
            WInput Parent;
            public Item(XmlElement wiz, WInput el)
            {
                Wiz = wiz;
                Parent = el;
            }
            public Item(WInput el)
            {
                Wiz = el.Wiz.OwnerDocument.CreateElement("item");
                el.Wiz.AppendChild(Wiz);
                Parent = el;
            }
            public string Value { set { SetAttribute("value", value); Notify("Value"); } get { return Wiz.GetAttribute("value"); } }
            public string Text { set { SetAttribute("text", value); Notify("Text"); } get { return Wiz.GetAttribute("text"); } }
            public string Count { set { SetAttribute("count", value); Notify("Count"); } get { return Wiz.GetAttribute("count"); } }
        }


        WizEl Parent;
        public WInput(XmlElement wiz, WizEl el)
        {
            Wiz = wiz;
            foreach (XmlNode child in Wiz.ChildNodes)
            {
                if (child is XmlElement && child.Name == "condition")
                    Conditions.Add(new WCondition((XmlElement)child));
                else if (child is XmlElement && child.Name == "item")
                    Items.Add(new Item((XmlElement)child, this));
            }
            Parent = el;
            InitN();
        }
        public WInput(WizEl el)
        {
            Wiz = el.Wiz.OwnerDocument.CreateElement("input");
            el.Wiz.AppendChild(Wiz);
            Parent = el;
            InitN();
        }
        private void InitN()
        {
            Conditions.CollectionChanged += ItemDeleteTrigger;
            Items.CollectionChanged += ItemDeleteTrigger;
        }
        public ObservableCollection<WCondition> Conditions { get; set; } = new ObservableCollection<WCondition>();
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
        public string Id { set { SetAttribute("id", value); Notify("Id"); } get { return Wiz.GetAttribute("id"); } }
        public string Type { set { SetAttribute("type", value); Notify("Type"); } get { return Wiz.GetAttribute("type"); } }
        public string Name
        {
            get { return ElementInnerGet("name"); }
            set { ElementInnerSet("name", value); Notify("Name"); }
        }
        public string Tooltip
        {
            get { return ElementInnerGet("tooltip"); }
            set { ElementInnerSet("tooltip", value); Notify("Tooltip"); }
        }
        public string Minimum
        {
            get { return ElementElementAttributeGet("minimum", "value"); }
            set { ElementElementAttributeSet("minimum", "value", value); Notify("Minimum"); }
        }
        public string Maximum
        {
            get { return ElementElementAttributeGet("maximum", "value"); }
            set { ElementElementAttributeSet("maximum", "value", value); Notify("Maximum"); }
        }
        public string Default
        {
            get { return ElementElementAttributeGet("default", "value"); }
            set { ElementElementAttributeSet("default", "value", value); Notify("Default"); }
        }
        public string Increment
        {
            get { return ElementElementAttributeGet("increment", "value"); }
            set { ElementElementAttributeSet("increment", "value", value); Notify("Increment"); }
        }
        public string Color
        {
            get { return ElementElementAttributeGet("color", "value"); }
            set { ElementElementAttributeSet("color", "value", value); Notify("Color"); }
        }
        public bool Nosave
        {
            get { return ElementExist("nosave"); }
            set { ElementExistSet("nosave", value); Notify("Nosave"); }
        }
        public bool Nopreview
        {
            get { return ElementExist("nopreview"); }
            set { ElementExistSet("nopreview", value); Notify("Nopreview"); }
        }


    }

    public partial class WValidate : WizardElementStandard
    {
        WizEl Parent;
        public WValidate(XmlElement wiz, WizEl el)
        {
            Wiz = wiz; foreach (XmlNode child in Wiz.ChildNodes)
            {
                if (child is XmlElement && child.Name == "condition")
                    Conditions.Add(new WCondition((XmlElement)child));
            }
            Parent = el;
            InitN();
        }

        public ObservableCollection<WCondition> Conditions { get; set; } = new ObservableCollection<WCondition>();
        public WValidate(WizEl el)
        {
            Wiz = el.Wiz.OwnerDocument.CreateElement("validate");
            el.Wiz.AppendChild(Wiz);
            Parent = el;
            InitN();
        }
        private void InitN()
        {
            Conditions.CollectionChanged += ItemDeleteTrigger;
        }
        public string Type { set { SetAttribute("type", value); Notify("Type"); } get { return Wiz.GetAttribute("type"); } }
        public string Text
        {
            get { return ElementInnerGet("text"); }
            set { ElementInnerSet("text", value); Notify("Text"); }
        }

        public WCondition NewCondition()
        {
            WCondition a1 = new WCondition(this);
            Conditions.Add(a1);
            Notify("Conditions");
            return a1;
        }
    }

    public partial class WMacro : WizardElementStandard
    {
        WizEl EL;
        public WMacro(XmlElement wiz, WizEl el)
        {
            Wiz = wiz;
            EL = el;
        }
        public WMacro(WizEl el)
        {
            Wiz = el.Wiz.OwnerDocument.CreateElement("macro");
            EL = el;
            el.Wiz.AppendChild(Wiz);
        }

        public string Id
        {
            set
            {
                SetAttribute("id", value);
                Notify("Id");
            }
            get { return Wiz.GetAttribute("id"); }
        }
        public string ReplaceSrc { set { SetAttribute("replaceSrc", value); Notify("ReplaceSrc"); } get { return Wiz.GetAttribute("replaceSrc"); } }
        public string ReplaceDst { set { SetAttribute("replaceDst", value); Notify("ReplaceDst"); } get { return Wiz.GetAttribute("replaceDst"); } }
        public string Truncate { set { SetAttribute("truncate", value); Notify("Truncate"); } get { return Wiz.GetAttribute("truncate"); } }
        public string Inner { set { Wiz.InnerText = value; Notify("Inner"); } get { return Wiz.InnerText; } }
    }

    public partial class WEntry : WizardElementStandard
    {
        WizEl Parent;
        public WEntry(XmlElement wiz, WizEl el)
        {
            Wiz = wiz;
            foreach (XmlNode child in Wiz.ChildNodes)
            {
                if (child is XmlElement)
                {
                    switch (child.Name)
                    {
                        case "condition":
                            Conditions.Add(new WCondition((XmlElement)child));
                            break;
                        case "field":
                            Fields.Add(new Field((XmlElement)child, this));
                            break;
                        case "token":
                            Tokens.Add(new Token((XmlElement)child, this));
                            break;
                    }
                }
            }
            Parent = el;
            InitN();
        }
        public WEntry(WizEl el)
        {
            Wiz = el.Wiz.OwnerDocument.CreateElement("entry");
            el.Wiz.AppendChild(Wiz);
            Parent = el;
            InitN();
        }
        private void InitN()
        {
            Conditions.CollectionChanged += ItemDeleteTrigger;
            Fields.CollectionChanged += ItemDeleteTrigger;
            Tokens.CollectionChanged += ItemDeleteTrigger;
        }

        public string Type { set { SetAttribute("type", value); Notify("Type"); } get { return Wiz.GetAttribute("type"); } }
        public string Catalog { set { SetAttribute("catalog", value); Notify("Catalog"); } get { return Wiz.GetAttribute("catalog"); } }
        public ObservableCollection<WCondition> Conditions { get; set; } = new ObservableCollection<WCondition>();
        public ObservableCollection<Field> Fields { get; set; } = new ObservableCollection<Field>();
        public ObservableCollection<Token> Tokens { get; set; } = new ObservableCollection<Token>();
        public string Id
        {
            get { return ElementInnerGet("id"); }
            set { ElementInnerSet("id", value); Notify("Id"); }
        }
        public string Useloaded
        {
            get { return ElementInnerGet("useloaded"); }
            set { ElementInnerSet("useloaded", value); Notify("Useloaded"); }
        }
        public string Parentid
        {
            get { return ElementInnerGet("parentid"); }
            set { ElementInnerSet("parentid", value); Notify("Parentid"); }
        }
        public string Count
        {
            get { return ElementInnerGet("count"); }
            set { ElementInnerSet("count", value); Notify("Count"); }
        }
        public bool Allowmodify
        {
            get { return ElementExist("allowmodify"); }
            set { ElementExistSet("allowmodify", value); Notify("Allowmodify"); }
        }
        public WCondition NewCondition()
        {
            WCondition a1 = new WCondition(this);
            Conditions.Add(a1);
            Notify("Conditions");
            return a1;
        }
        public Field NewField()
        {
            Field a1 = new Field(this);
            Fields.Add(a1);
            Notify("Fields");
            return a1;
        }
        public Token NewToken()
        {
            Token a1 = new Token(this);
            Tokens.Add(a1);
            Notify("Tokens");
            return a1;
        }


        public partial class Field : WizardElementStandard
        {
            WEntry Parent;
            public Field(XmlElement wiz, WEntry el)
            {
                Wiz = wiz;
                foreach (XmlNode child in Wiz.ChildNodes)
                {
                    if (child is XmlElement)
                    {
                        switch (child.Name)
                        {
                            case "condition":
                                Conditions.Add(new WCondition((XmlElement)child));
                                break;
                        }
                    }
                }
                Parent = el;
                InitN();
            }
            public Field(WEntry el)
            {
                Wiz = el.Wiz.OwnerDocument.CreateElement("field");
                el.Wiz.AppendChild(Wiz);
                Parent = el;
                InitN();

            }
            private void InitN()
            {
                Conditions.CollectionChanged += ItemDeleteTrigger;
            }

            public ObservableCollection<WCondition> Conditions { get; set; } = new ObservableCollection<WCondition>();
            public string Id { set { SetAttribute("id", value); Notify("Id"); } get { return Wiz.GetAttribute("id"); } }

            public string FIndex
            {
                get { return ElementInnerGet("index"); }
                set { ElementInnerSet("index", value); Notify("FIndex"); }
            }
            public string Count
            {
                get { return ElementInnerGet("count"); }
                set { ElementInnerSet("count", value); Notify("Count"); }
            }
            public string Value
            {
                get { return ElementInnerGet("value"); }
                set { ElementInnerSet("value", value); Notify("Value"); }
            }
            public string StringId
            {
                get { return ElementInnerGet("stringid"); }
                set { ElementInnerSet("stringid", value); Notify("StringId"); }
            }
            public string LoadInput
            {
                get { return ElementInnerGet("loadinput"); }
                set { ElementInnerSet("loadinput", value); Notify("LoadInput"); }
            }

            public WCondition NewCondition()
            {
                WCondition a1 = new WCondition(this);
                Conditions.Add(a1);
                Notify("Conditions");
                return a1;
            }
        }
        public partial class Token : WizardElementStandard
        {
            WEntry Parent;
            public Token(XmlElement wiz, WEntry el)
            {
                Wiz = wiz;
                foreach (XmlNode child in Wiz.ChildNodes)
                {
                    if (child is XmlElement && child.Name == "condition")
                        Conditions.Add(new WCondition((XmlElement)child));

                }
                Parent = el;
                InitN();
            }
            public Token(WEntry el)
            {
                Wiz = el.Wiz.OwnerDocument.CreateElement("token");
                el.Wiz.AppendChild(Wiz);
                Parent = el;
                InitN();
            }
            private void InitN()
            {
                Conditions.CollectionChanged += ItemDeleteTrigger;
            }

            public ObservableCollection<WCondition> Conditions { get; set; } = new ObservableCollection<WCondition>();
            public string Id { set { SetAttribute("id", value); Notify("Id"); } get { return Wiz.GetAttribute("id"); } }
            public string Value
            {
                get { return ElementInnerGet("value"); }
                set { ElementInnerSet("value", value); Notify("Value"); }
            }
            public string LoadInput
            {
                get { return ElementInnerGet("loadinput"); }
                set { ElementInnerSet("loadinput", value); Notify("LoadInput"); }
            }

            public WCondition NewCondition()
            {
                WCondition a1 = new WCondition(this);
                Conditions.Add(a1);
                Notify("Conditions");
                return a1;
            }
        }
    }

}
