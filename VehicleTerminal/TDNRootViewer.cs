using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Team.HobbyRobot.TDN.Base;
using Team.HobbyRobot.TDN.Core;

namespace VehicleTerminal
{
    public partial class TDNRootViewer : UserControl
    {
        private TDNRoot root;
        public TDNRoot Root
        {
            get => root;
            set { root = value; Refresh(); }
        }

        public TDNRootViewer()
        {
            root = new TDNRoot();
            InitializeComponent();
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateRootView();
        }

        private void UpdateRootView()
        {
            RootView.Nodes.Clear();
            AddRootToNode(root, RootView.Nodes.Add("root"));
            RootView.ExpandAll();
        }

        private void AddRootToNode(TDNRoot root, TreeNode node)
        {
            foreach (KeyValuePair<string, TDNValue> keyValue in root)
            {
                if (keyValue.Value.Parser.TypeKey == TDNParsers.ROOT.TypeKey)
                {
                    AddRootToNode(keyValue.Value.Value as TDNRoot, node.Nodes.Add(keyValue.Key));
                    continue;
                }
                else if(keyValue.Value.Parser.TypeKey == TDNParsers.ARRAY.TypeKey)
                {
                    TDNArray arr = keyValue.Value.As<TDNArray>();
                    TreeNode arrNode = node.Nodes.Add($"[{keyValue.Value.Parser.TypeKey} - {arr.ItemParser.TypeKey}] {keyValue.Key}: {keyValue.Value.Value}");
                    int i = 0;
                    foreach (object ii in arr)
                    {
                        arrNode.Nodes.Add($"[{i++}] {keyValue.Value.Value}");
                    }
                    continue;
                }
                node.Nodes.Add($"[{keyValue.Value.Parser.TypeKey}] {keyValue.Key}: {keyValue.Value.Value}");
            }
        }

        public void SetRootValue(string path, TDNValue value)
        {
            root[path] = value;
            Refresh();
        }
    }
}
